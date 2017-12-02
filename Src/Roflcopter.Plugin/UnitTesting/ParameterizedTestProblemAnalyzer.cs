using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReSharperExtensionsShared.ProblemAnalyzers;
#if RS20171
using JetBrains.ReSharper.Psi.CSharp.Impl;

#else
using JetBrains.ReSharper.Psi.CSharp.Conversions;

#endif

namespace Roflcopter.Plugin.UnitTesting
{
    [ElementProblemAnalyzer(
        typeof(IMethodDeclaration),
        HighlightingTypes = new[]
        {
            typeof(ParameterizedTestMissingArgumentHighlighting),
            typeof(ParameterizedTestMissingParameterHighlighting),
            typeof(ParameterizedTestTypeMismatchHighlighting),
        })]
    public class ParameterizedTestProblemAnalyzer : SimpleElementProblemAnalyzer<IMethodDeclaration, IMethod>
    {
        private static readonly IClrTypeName TestAttribute = new ClrTypeName("NUnit.Framework.TestAttribute");

        private static readonly IClrTypeName TestBuilderAttribute = new ClrTypeName("NUnit.Framework.Interfaces.ITestBuilder");
        private static readonly IClrTypeName TestCaseAttribute = new ClrTypeName("NUnit.Framework.TestCaseAttribute");
        private static readonly IClrTypeName TestCaseSourceAttribute = new ClrTypeName("NUnit.Framework.TestCaseSourceAttribute");

        private static readonly IClrTypeName ParameterDataSourceInterface = new ClrTypeName("NUnit.Framework.Interfaces.IParameterDataSource");
        private static readonly IClrTypeName ValuesAttribute = new ClrTypeName("NUnit.Framework.ValuesAttribute");
        private static readonly IClrTypeName ValueSourceAttribute = new ClrTypeName("NUnit.Framework.ValueSourceAttribute");

        protected override void Run(
            IMethodDeclaration methodDeclaration,
            IMethod method,
            ElementProblemAnalyzerData _,
            IHighlightingConsumer consumer)
        {
            // IDEA: Check for NUnit assembly reference for performance (using INUnitVersionDetector.GetVersion)?

            if (!method.IsAbstract)
            {
                if (methodDeclaration.AttributesEnumerable.Any(x => IsAttributeOrDerivedFrom(x, TestAttribute, TestCaseAttribute)))
                {
                    AnalyzeTestMethod(methodDeclaration, consumer);
                }
            }
        }

        private void AnalyzeTestMethod(IMethodDeclaration methodDeclaration, IHighlightingConsumer consumer)
        {
            var parametersCount = methodDeclaration.ParameterDeclarations.Count;

            var sourceInfo = GetParameterizedTestSourceInfo(methodDeclaration);

            for (var iParameter = 0; iParameter < parametersCount; iParameter++)
            {
                // Test method attributes can only be applied to regular parameter methods:
                var parameterDeclaration = (IRegularParameterDeclaration) methodDeclaration.ParameterDeclarations[iParameter];

                var parameterHasSomeSource = sourceInfo.HasNonTestCaseTestBuilder;

                foreach (var testCaseAttributeInfo in sourceInfo.TestCaseAttributes)
                {
                    var argumentInfo = testCaseAttributeInfo.GetArgumentForParameter(iParameter);

                    if (argumentInfo == null)
                    {
                        var highlightingNode = testCaseAttributeInfo.Attribute.Name;
                        consumer.AddHighlighting(
                            new ParameterizedTestMissingArgumentHighlighting(highlightingNode, methodDeclaration, parameterDeclaration));
                    }
                    else
                    {
                        parameterHasSomeSource = true;

                        AnalyzeArgumentType(argumentInfo.Value.Expression, parameterDeclaration, consumer);
                    }
                }

                parameterHasSomeSource |= AnalyzeValuesAttribute(parameterDeclaration, consumer);

                if (!parameterHasSomeSource)
                {
                    consumer.AddHighlighting(
                        new ParameterizedTestMissingArgumentHighlighting(parameterDeclaration, methodDeclaration, parameterDeclaration));
                }
            }

            foreach (var testCaseAttributeInfo in sourceInfo.TestCaseAttributes)
            {
                var isFirstMissingParameter = true;

                foreach (var argument in testCaseAttributeInfo.Arguments.Skip(parametersCount))
                {
                    consumer.AddHighlighting(
                        new ParameterizedTestMissingParameterHighlighting(methodDeclaration, argument.Expression, isFirstMissingParameter));

                    isFirstMissingParameter = false;
                }
            }
        }

        private bool AnalyzeValuesAttribute(IRegularParameterDeclaration parameterDeclaration, IHighlightingConsumer consumer)
        {
            var parameterHasAnyDataSource = false;

            foreach (var attribute in parameterDeclaration.AttributesEnumerable)
            {
                if (IsAttributeOrDerivedFrom(attribute, ParameterDataSourceInterface,
                    // NUnit 2 support:
                    ValueSourceAttribute))
                {
                    parameterHasAnyDataSource = true;
                }

                if (IsAttributeOrDerivedFrom(attribute, ValuesAttribute))
                {
                    parameterHasAnyDataSource = true;

                    var arguments = GetArgumentsOfTestDataAttribute(attribute);

                    foreach (var argument in arguments)
                    {
                        AnalyzeArgumentType(argument.Expression, parameterDeclaration, consumer);
                    }
                }
            }

            return parameterHasAnyDataSource;
        }

        private static void AnalyzeArgumentType(
            IExpression argumentExpression,
            IParameterDeclaration parameterDeclaration,
            IHighlightingConsumer consumer)
        {
            var typeConversionRule = argumentExpression.GetTypeConversionRule();

            if (!argumentExpression.Type().IsImplicitlyConvertibleTo(parameterDeclaration.Type, typeConversionRule))
            {
                consumer.AddHighlighting(new ParameterizedTestTypeMismatchHighlighting(argumentExpression, parameterDeclaration));
            }
        }

        private static ParameterizedTestSourceInfo GetParameterizedTestSourceInfo(IMethodDeclaration methodDeclaration)
        {
            var testCaseAttributeInfos = new List<TestCaseAttributeInfo>();
            var hasNonTestCaseTestBuilder = false;

            foreach (var attribute in methodDeclaration.AttributesEnumerable)
            {
                if (IsAttributeOrDerivedFrom(attribute, TestCaseAttribute))
                {
                    var arguments = GetArgumentsOfTestDataAttribute(attribute);

                    testCaseAttributeInfos.Add(new TestCaseAttributeInfo(attribute, arguments.ToList()));
                }
                else
                {
                    if (IsAttributeOrDerivedFrom(attribute, TestBuilderAttribute,
                        // NUnit 2 support:
                        TestCaseSourceAttribute))
                    {
                        hasNonTestCaseTestBuilder = true;
                    }
                }
            }

            return new ParameterizedTestSourceInfo(testCaseAttributeInfos, hasNonTestCaseTestBuilder);
        }

        /// <summary>
        /// Returns the list of arguments for <see cref="TestCaseAttribute"/> or <see cref="ValuesAttribute"/> declarations.
        /// </summary>
        private static IEnumerable<TestCaseArgumentInfo> GetArgumentsOfTestDataAttribute(IAttribute testCaseAttribute)
        {
            var arguments = testCaseAttribute.Arguments;

            var singleArgument = arguments.SingleItem;

            if (singleArgument != null && singleArgument.MatchingParameter?.Element.IsParameterArray == true)
            {
                if (singleArgument.Value is IArrayCreationExpression arrayCreationExpression)
                {
                    var initializers = arrayCreationExpression.ArrayInitializer.ElementInitializersEnumerable;

                    return initializers.Select(x => new TestCaseArgumentInfo(arrayExpressionInitializer: (IExpressionInitializer) x));
                }
            }

            return arguments.Select(x => new TestCaseArgumentInfo(x));
        }

        private struct ParameterizedTestSourceInfo
        {
            public ParameterizedTestSourceInfo(IReadOnlyCollection<TestCaseAttributeInfo> testCaseAttributes, bool hasNonTestCaseTestBuilder)
            {
                TestCaseAttributes = testCaseAttributes;
                HasNonTestCaseTestBuilder = hasNonTestCaseTestBuilder;
            }

            public IReadOnlyCollection<TestCaseAttributeInfo> TestCaseAttributes { get; }
            public bool HasNonTestCaseTestBuilder { get; }
        }

        private struct TestCaseAttributeInfo
        {
            public TestCaseAttributeInfo(IAttribute attribute, IReadOnlyList<TestCaseArgumentInfo> arguments)
            {
                Attribute = attribute;
                Arguments = arguments;
            }

            public IAttribute Attribute { get; }
            public IReadOnlyList<TestCaseArgumentInfo> Arguments { get; }

            public TestCaseArgumentInfo? GetArgumentForParameter(int index) =>
                index < Arguments.Count ? Arguments[index] : (TestCaseArgumentInfo?) null;
        }

        /// <summary>
        /// Represents a test case argument (which can be an attribute argument or an array initializer expression).
        /// </summary>
        private struct TestCaseArgumentInfo
        {
            public TestCaseArgumentInfo(ICSharpArgument argument = null, IExpressionInitializer arrayExpressionInitializer = null)
            {
                Assertion.Assert((argument == null) ^ (arrayExpressionInitializer == null), "argument XOR arrayExpressionInitializer");

                Argument = argument;
                ArrayExpressionInitializer = arrayExpressionInitializer;
            }

            [CanBeNull]
            public ICSharpArgument Argument { get; }

            [CanBeNull]
            public IExpressionInitializer ArrayExpressionInitializer { get; }

            public ICSharpExpression Expression => Argument?.Value ?? ArrayExpressionInitializer.NotNull().Value;
        }

        private static bool IsAttributeOrDerivedFrom(IAttribute attribute, [ItemNotNull] params IClrTypeName[] typeNamesToTest)
        {
            if (attribute.TypeReference != null)
            {
                if (attribute.TypeReference.Resolve().DeclaredElement is ITypeElement typeElement)
                {
                    return typeNamesToTest.Any(typeNameToTest =>
                    {
                        var interfaceTypeToTest = TypeFactory.CreateTypeByCLRName(typeNameToTest, attribute.PsiModule);

                        return typeElement.IsDescendantOf(interfaceTypeToTest.GetTypeElement());
                    });
                }
            }

            return false;
        }
    }
}
