using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;
using ReSharperExtensionsShared.Highlighting;
using Roflcopter.Plugin.AssertionMessages;

[assembly: RegisterConfigurableSeverity(
    InvalidAssertionMessageHighlighting.SeverityId,
    CompoundItemName: null,
    Group: HighlightingGroupIds.CodeSmell,
    Title: InvalidAssertionMessageHighlighting.Title,
    Description: InvalidAssertionMessageHighlighting.Description,
    DefaultSeverity: Severity.WARNING)]

namespace Roflcopter.Plugin.AssertionMessages
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        OverlapResolve = OverlapResolveKind.NONE,
        ToolTipFormatString = Message)]
    public class InvalidAssertionMessageHighlighting : SimpleTreeNodeHighlightingBase<ICSharpLiteralExpression>
    {
        public const string SeverityId = "InvalidAssertionMessage";
        public const string Title = "Assertion message is invalid";
        private const string Message = "Assertion message is invalid. Expected '{0}'.";

        public const string Description = "Assertion message is invalid (e.g. wrong/outdated null-check " +
                                          "condition is part of the assertion message).";

        private readonly int _messagePostfixLength;

        public InvalidAssertionMessageHighlighting(
            ICSharpLiteralExpression messageExpression,
            int messagePostfixLength,
            string expectedMessage)
            : base(messageExpression, string.Format(Message, expectedMessage))
        {
            Assertion.Assert(
                messageExpression.GetTextLength() > 1 + _messagePostfixLength + 1,
                "messageExpression.GetTextLength() > 1 + _messagePostfixLength + 1");

            _messagePostfixLength = messagePostfixLength;
        }

        public override DocumentRange CalculateRange() =>
            base.CalculateRange().TrimLeft(1).TrimRight(_messagePostfixLength + 1);
    }
}
