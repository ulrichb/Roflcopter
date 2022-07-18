using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.Resolve;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve.ExtensionMethods;
using JetBrains.Util;

namespace Roflcopter.Plugin.AssertionMessages
{
    [ElementProblemAnalyzer(
        typeof(IInvocationExpression),
        HighlightingTypes = new[] { typeof(InvalidAssertionMessageHighlighting) })]
    public class InvalidAssertionMessageProblemAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
    {
        private readonly ContractAnnotationProvider _contractAnnotationProvider;
        private readonly AssertionMethodAnnotationProvider _assertionMethodAnnotationProvider;
        private readonly AssertionConditionAnnotationProvider _assertionConditionAnnotationProvider;

        public InvalidAssertionMessageProblemAnalyzer(CodeAnnotationsCache annotationsCache)
        {
            _contractAnnotationProvider = annotationsCache.GetProvider<ContractAnnotationProvider>();
            _assertionMethodAnnotationProvider = annotationsCache.GetProvider<AssertionMethodAnnotationProvider>();
            _assertionConditionAnnotationProvider = annotationsCache.GetProvider<AssertionConditionAnnotationProvider>();
        }

        protected override void Run(
            IInvocationExpression invocationExpression,
            ElementProblemAnalyzerData data,
            IHighlightingConsumer consumer)
        {
            var arguments = invocationExpression.Arguments;

            // Short circuit exit:
            if (arguments.Count < 1)
                return;

            var resolveResult = invocationExpression.InvocationExpressionReference.Resolve();

            if (resolveResult.DeclaredElement is IMethod method)
            {
                // Following 'FDTApplicator..ctor' and 'CSharpControlFlowGraphInspector.PatchContextByObsoleteAnnotatedMethodCall'

                var functionDefinitionTable = _contractAnnotationProvider.GetInfo(method);

                if (functionDefinitionTable != null || _assertionMethodAnnotationProvider.GetInfo(method))
                {
                    if (resolveResult.Result.IsExtensionMethodInvocation())
                    {
                        var thisArgumentInfo = (ExtensionArgumentInfo) invocationExpression
                            .ExtensionQualifier.NotNull("seems to be escaped by the positive resolution");

                        var conditionType = GetConditionTypeForParameter(thisArgumentInfo, functionDefinitionTable);

                        if (conditionType != null)
                        {
                            var conditionExpression = thisArgumentInfo.Expression;
                            Assertion.Assert(arguments.Count > 0, "arguments.Count > 0");
                            var nextToConditionArgument = arguments[0];

                            CheckMessageArgument(conditionExpression, conditionType.Value, nextToConditionArgument, consumer);
                        }
                    }
                    else
                    {
                        for (var i = 0; i < arguments.Count - 1; i++)
                        {
                            var argument = arguments[i];
                            var nextArgument = arguments[i + 1];

                            var conditionType = GetConditionTypeForParameter(argument, functionDefinitionTable);

                            if (conditionType != null)
                            {
                                var conditionExpression = argument.Value;
                                CheckMessageArgument(conditionExpression, conditionType.Value, nextArgument, consumer);
                            }
                        }
                    }
                }
            }
        }

        private ConditionType? GetConditionTypeForParameter(
            ICSharpArgumentInfo argumentInfo,
            [CanBeNull] FunctionDefinitionTable functionDefinitionTable)
        {
            var matchingParameter = argumentInfo.MatchingParameter;
            if (matchingParameter != null)
            {
                var parameter = matchingParameter.Element;

                if (functionDefinitionTable != null)
                {
                    var conditionType = GetContractAnnotationConditionTypeForParameter(functionDefinitionTable, parameter);

                    if (conditionType != null)
                        return conditionType;
                }

                return GetAssertionConditionAnnotationConditionType(parameter);
            }

            return null;
        }

        [Pure]
        private static ConditionType? GetContractAnnotationConditionTypeForParameter(
            FunctionDefinitionTable functionDefinitionTable,
            IParameter parameter)
        {
            foreach (var row in functionDefinitionTable.Rows)
            {
                if (row.FunctionReturn == ContractAnnotationValue.HALT)
                {
                    if (row.Input.Count == 1)
                    {
                        var contractAnnotationValue =
                            row.Input[0].ParameterName == parameter.ShortName ? row.Input[0].Value : (ContractAnnotationValue?) null;

                        switch (contractAnnotationValue)
                        {
                            case ContractAnnotationValue.TRUE:
                            case ContractAnnotationValue.FALSE: return ConditionType.TrueOrFalseCheck;
                            case ContractAnnotationValue.NULL:
                            case ContractAnnotationValue.NOT_NULL: return ConditionType.NullEqualityCheck;
                        }
                    }
                }
            }

            return null;
        }

        private ConditionType? GetAssertionConditionAnnotationConditionType(IParameter parameter)
        {
            var assertionConditionType = _assertionConditionAnnotationProvider.GetInfo(parameter);

            switch (assertionConditionType)
            {
                case AssertionConditionType.IS_TRUE:
                case AssertionConditionType.IS_FALSE: return ConditionType.TrueOrFalseCheck;
                case AssertionConditionType.IS_NULL:
                case AssertionConditionType.IS_NOT_NULL: return ConditionType.NullEqualityCheck;
            }

            return null;
        }

        private static void CheckMessageArgument(
            ICSharpExpression conditionExpression,
            ConditionType conditionType,
            ICSharpArgument nextToConditionArgument,
            IHighlightingConsumer consumer)
        {
            if (nextToConditionArgument.Value is ICSharpLiteralExpression literalExpression)
            {
                if (literalExpression.ConstantValue.AsString() is { } message)
                {
                    var nullabilityEqualityPostfixLength = MatchNullabilityEqualityPostfix(message);

                    if (nullabilityEqualityPostfixLength.HasValue)
                    {
                        int messagePostfixLength = 0;

                        if (conditionType == ConditionType.NullEqualityCheck)
                        {
                            // For not null/null assertions we strip the postfix away. 

                            messagePostfixLength = nullabilityEqualityPostfixLength.Value;
                        }

                        CheckMessageExpression(conditionExpression, literalExpression, message, messagePostfixLength, consumer);
                    }
                }
            }
        }

        private static readonly Regex EndsWithNotNull = new Regex(" *!= *null$");
        private static readonly Regex EndsWithIsNull = new Regex(" *== *null$");

        [Pure]
        private static int? MatchNullabilityEqualityPostfix(string message)
        {
            var endsWithNotNullMatch = EndsWithNotNull.Match(message);
            if (endsWithNotNullMatch.Success)
                return endsWithNotNullMatch.Length;

            var endsWithIsNullMatch = EndsWithIsNull.Match(message);
            if (endsWithIsNullMatch.Success)
                return endsWithIsNullMatch.Length;

            return null;
        }

        private static void CheckMessageExpression(
            ICSharpExpression conditionExpression,
            ICSharpLiteralExpression messageExpression,
            string message,
            int messagePostfixLength,
            IHighlightingConsumer consumer)
        {
            var conditionExpressionText = conditionExpression.GetText();

            var messageConditionPart = message.Substring(0, message.Length - messagePostfixLength);

            if (!EqualsIgnoringWhitespace(messageConditionPart, conditionExpressionText))
            {
                consumer.AddHighlighting(new InvalidAssertionMessageHighlighting(
                    messageExpression,
                    messagePostfixLength,
                    conditionExpressionText));
            }
        }

        [Pure]
        private static bool EqualsIgnoringWhitespace(string a, string b) =>
            StringUtil.CutSpaces(a).Equals(StringUtil.CutSpaces(b), StringComparison.Ordinal);

        private enum ConditionType
        {
            TrueOrFalseCheck,
            NullEqualityCheck,
        }
    }
}
