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
                    var argumentExpression = testCaseAttributeInfo.GetArgumentExpressionForParameter(iParameter);

                    if (argumentExpression == null)
                    {
                        var highlightingNode = testCaseAttributeInfo.Attribute.Name;
                        consumer.AddHighlighting(new ParameterizedTestMissingArgumentHighlighting(highlightingNode, parameterDeclaration));
                    }
                    else
                    {
                        parameterHasSomeSource = true;

                        AnalyzeArgumentType(argumentExpression, parameterDeclaration, consumer);
                    }
                }

                parameterHasSomeSource |= AnalyzeValuesAttribute(parameterDeclaration, consumer);

                if (!parameterHasSomeSource)
                {
                    consumer.AddHighlighting(new ParameterizedTestMissingArgumentHighlighting(parameterDeclaration, parameterDeclaration));
                }
            }

            foreach (var testCaseAttributeInfo in sourceInfo.TestCaseAttributes)
            {
                foreach (var argumentsExpression in testCaseAttributeInfo.ArgumentExpressions.Skip(parametersCount))
                {
                    consumer.AddHighlighting(new ParameterizedTestMissingParameterHighlighting(argumentsExpression));
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

                    var argumentsExpressions = GetArgumentsOfTestDataAttribute(attribute);

                    foreach (var argumentsExpression in argumentsExpressions)
                    {
                        AnalyzeArgumentType(argumentsExpression, parameterDeclaration, consumer);
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
                    var argumentsExpressions = GetArgumentsOfTestDataAttribute(attribute);

                    testCaseAttributeInfos.Add(new TestCaseAttributeInfo(attribute, argumentsExpressions.ToList()));
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
        private static IEnumerable<ICSharpExpression> GetArgumentsOfTestDataAttribute(IAttribute testCaseAttribute)
        {
            if (testCaseAttribute.ConstructorReference.Resolve().DeclaredElement is IConstructor constructor)
            {
                if (constructor.Parameters.Count == 1 && constructor.Parameters[0].IsParameterArray)
                {
                    var paramsExpression = testCaseAttribute.ConstructorArgumentExpressions.SingleItem;

                    if (paramsExpression is IArrayCreationExpression arrayCreationExpression)
                    {
                        var initializers = arrayCreationExpression.ArrayInitializer.ElementInitializersEnumerable;

                        return initializers.Select(x => ((IExpressionInitializer) x).Value);
                    }
                }
            }

            return testCaseAttribute.ConstructorArgumentExpressions;
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
            public TestCaseAttributeInfo(IAttribute attribute, IReadOnlyList<ICSharpExpression> argumentExpressions)
            {
                Attribute = attribute;
                ArgumentExpressions = argumentExpressions;
            }

            public IAttribute Attribute { get; }
            public IReadOnlyList<ICSharpExpression> ArgumentExpressions { get; }

            [CanBeNull]
            public ICSharpExpression GetArgumentExpressionForParameter(int index) => ArgumentExpressions.ElementAtOrDefault(index);
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
