using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Conversions;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReSharperExtensionsShared.ProblemAnalyzers;
using Roflcopter.Plugin.Utilities;

#if RS20172
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;

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
        protected override void Run(
            IMethodDeclaration methodDeclaration,
            IMethod method,
            ElementProblemAnalyzerData _,
            IHighlightingConsumer consumer)
        {
            // IDEA: Check for NUnit assembly reference for performance (using INUnitVersionDetector.GetVersion)?

            if (!method.IsAbstract)
            {
                if (ParameterizedTests.IsTestOrTestCaseMethod(methodDeclaration))
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

                foreach (var argumentInfo in testCaseAttributeInfo.Arguments.Skip(parametersCount))
                {
                    if (argumentInfo.Expression != null)
                    {
                        consumer.AddHighlighting(new ParameterizedTestMissingParameterHighlighting(
                            methodDeclaration,
                            testCaseAttributeInfo.Attribute,
                            isFirstMissingParameter,
                            argumentInfo.Expression,
                            argumentInfo.Argument));
                    }

                    isFirstMissingParameter = false;
                }
            }
        }

        private bool AnalyzeValuesAttribute(IRegularParameterDeclaration parameterDeclaration, IHighlightingConsumer consumer)
        {
            var parameterHasAnyDataSource = false;

            foreach (var attribute in parameterDeclaration.AttributesEnumerable)
            {
                if (attribute.IsAttributeOrDerivedFrom(ParameterizedTests.ParameterDataSourceInterface,
                    // NUnit 2 support:
                    ParameterizedTests.ValueSourceAttribute))
                {
                    parameterHasAnyDataSource = true;
                }

                if (attribute.IsAttributeOrDerivedFrom(ParameterizedTests.ValuesAttribute))
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
            [CanBeNull] IExpression argumentExpression,
            IParameterDeclaration parameterDeclaration,
            IHighlightingConsumer consumer)
        {
            if (argumentExpression != null && !argumentExpression.ConstantValue.IsNull())
            {
                var typeConversionRule = argumentExpression.GetTypeConversionRule();

                if (!argumentExpression.Type().IsImplicitlyConvertibleTo(parameterDeclaration.Type, typeConversionRule))
                {
                    consumer.AddHighlighting(new ParameterizedTestTypeMismatchHighlighting(argumentExpression, parameterDeclaration));
                }
            }
        }

        private static ParameterizedTestSourceInfo GetParameterizedTestSourceInfo(IMethodDeclaration methodDeclaration)
        {
            var testCaseAttributeInfos = new List<TestCaseAttributeInfo>();
            var hasNonTestCaseTestBuilder = false;

            foreach (var attribute in methodDeclaration.AttributesEnumerable)
            {
                if (attribute.IsAttributeOrDerivedFrom(ParameterizedTests.TestCaseAttribute))
                {
                    var arguments = GetArgumentsOfTestDataAttribute(attribute);

                    testCaseAttributeInfos.Add(new TestCaseAttributeInfo(attribute, arguments.ToList()));
                }
                else
                {
                    if (attribute.IsAttributeOrDerivedFrom(ParameterizedTests.TestBuilderAttribute,
                        // NUnit 2 support:
                        ParameterizedTests.TestCaseSourceAttribute))
                    {
                        hasNonTestCaseTestBuilder = true;
                    }
                }
            }

            return new ParameterizedTestSourceInfo(testCaseAttributeInfos, hasNonTestCaseTestBuilder);
        }

        /// <summary>
        /// Returns the list of arguments for <see cref="ParameterizedTests.TestCaseAttribute"/> or
        /// <see cref="ParameterizedTests.ValuesAttribute"/> declarations.
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
            private IExpressionInitializer ArrayExpressionInitializer { get; }

            /// <summary>
            /// Can be null in incomplete argument-lists (e.g. <c>"arg a", "arg b",</c>).
            /// </summary>
            [CanBeNull]
            public ICSharpExpression Expression => Argument != null ? Argument.Value : ArrayExpressionInitializer.NotNull().Value;
        }
    }
}
