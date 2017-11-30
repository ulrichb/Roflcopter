using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;
using Roflcopter.Plugin.UnitTesting;

[assembly: RegisterConfigurableSeverity(
    ParameterizedTestMissingArgumentHighlighting.SeverityId,
    CompoundItemName: null,
    Group: HighlightingGroupIds.CodeSmell,
    Title: ParameterizedTestMissingArgumentHighlighting.Title,
    Description: ParameterizedTestMissingArgumentHighlighting.Description,
    DefaultSeverity: Severity.WARNING)]

[assembly: RegisterConfigurableSeverity(
    ParameterizedTestMissingParameterHighlighting.SeverityId,
    CompoundItemName: null,
    Group: HighlightingGroupIds.CodeSmell,
    Title: ParameterizedTestMissingParameterHighlighting.Title,
    Description: ParameterizedTestMissingParameterHighlighting.Description,
    DefaultSeverity: Severity.WARNING)]

[assembly: RegisterConfigurableSeverity(
    ParameterizedTestTypeMismatchHighlighting.SeverityId,
    CompoundItemName: null,
    Group: HighlightingGroupIds.CodeSmell,
    Title: ParameterizedTestTypeMismatchHighlighting.Title,
    Description: ParameterizedTestTypeMismatchHighlighting.Description,
    DefaultSeverity: Severity.WARNING)]

namespace Roflcopter.Plugin.UnitTesting
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        OverlapResolve = OverlapResolveKind.WARNING,
        ToolTipFormatString = Message)]
    public class ParameterizedTestMissingArgumentHighlighting : SimpleTreeNodeHighlightingBase<ITreeNode>
    {
        public const string SeverityId = "ParameterizedTestMissingArgument";
        public const string Title = "Missing argument in parameterized test";
        private const string Message = "Missing arguments for parameter '{0}'";

        public const string Description = Title;

        public ParameterizedTestMissingArgumentHighlighting(ITreeNode highlightingNode, IParameterDeclaration parameterDeclaration) :
            base(highlightingNode, string.Format(Message, parameterDeclaration.DeclaredName))
        {
        }
    }

    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        OverlapResolve = OverlapResolveKind.WARNING,
        ToolTipFormatString = Message)]
    public class ParameterizedTestMissingParameterHighlighting : SimpleTreeNodeHighlightingBase<ITreeNode>
    {
        public const string SeverityId = "ParameterizedTestMissingParameter";
        public const string Title = "Missing parameter in parameterized test";
        private const string Message = "Missing parameter for argument";

        public const string Description = Title;

        public ParameterizedTestMissingParameterHighlighting(
            IMethodDeclaration methodDeclaration,
            IExpression argumentExpression,
            bool isFirstMissingParameter) :
            base(argumentExpression, string.Format(Message))
        {
            MethodDeclaration = methodDeclaration;
            ArgumentExpression = argumentExpression;
            IsFirstMissingParameter = isFirstMissingParameter;
        }

        public IMethodDeclaration MethodDeclaration { get; }
        public IExpression ArgumentExpression { get; }
        public bool IsFirstMissingParameter { get; }
    }

    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        OverlapResolve = OverlapResolveKind.WARNING,
        ToolTipFormatString = Message)]
    public class ParameterizedTestTypeMismatchHighlighting : SimpleTreeNodeHighlightingBase<ITreeNode>
    {
        public const string SeverityId = "ParameterizedTestTypeMismatch";
        public const string Title = "Type mismatch in parameterized test";
        private const string Message = "Argument value not convertible to '{0}'";

        public const string Description = Title;

        public ParameterizedTestTypeMismatchHighlighting(IExpression argumentExpression, IParameterDeclaration parameterDeclaration) :
            base(argumentExpression, string.Format(Message, parameterDeclaration.Type))
        {
            ArgumentExpression = argumentExpression;
            ParameterDeclaration = parameterDeclaration;
        }

        public IExpression ArgumentExpression { get; }
        public IParameterDeclaration ParameterDeclaration { get; }
    }
}
