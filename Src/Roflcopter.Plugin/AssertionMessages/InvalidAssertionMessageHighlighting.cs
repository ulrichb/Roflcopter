using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharperExtensionsShared.Highlighting;

namespace Roflcopter.Plugin.AssertionMessages
{
    [RegisterConfigurableSeverity(
        SeverityId,
        CompoundItemName: null,
        Group: HighlightingGroupIds.CodeSmell,
        Title: Title,
        Description: Description,
        DefaultSeverity: Severity.WARNING)]
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        OverlapResolve = OverlapResolveKind.NONE,
        ToolTipFormatString = Message)]
    public class InvalidAssertionMessageHighlighting : SimpleTreeNodeHighlightingBase<ICSharpLiteralExpression>
    {
        private const string SeverityId = "InvalidAssertionMessage";
        private const string Title = "Assertion message is invalid";
        private const string Message = "Assertion message is invalid. Expected '{0}'.";

        private const string Description = "Assertion message is invalid (e.g. wrong/outdated null-check " +
                                           "condition is part of the assertion message).";

        private readonly int _messagePostfixLength;

        public InvalidAssertionMessageHighlighting(
            ICSharpLiteralExpression messageExpression,
            int messagePostfixLength,
            string expectedMessage)
            : base(messageExpression, string.Format(Message, expectedMessage))
        {
            Assertion.Assert(
                messageExpression.GetTextLength() > 1 + _messagePostfixLength + 1);

            _messagePostfixLength = messagePostfixLength;
        }

        public override DocumentRange CalculateRange() =>
            base.CalculateRange().TrimLeft(1).TrimRight(_messagePostfixLength + 1);
    }
}
