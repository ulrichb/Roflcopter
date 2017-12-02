using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharperExtensionsShared.QuickFixes;

namespace Roflcopter.Plugin.UnitTesting
{
    [QuickFix]
    public class ParameterizedTestMissingParameterRemoveArgumentQuickFix :
        SimpleQuickFixBase<ParameterizedTestMissingParameterHighlighting, ITreeNode>
    {
        public ParameterizedTestMissingParameterRemoveArgumentQuickFix(ParameterizedTestMissingParameterHighlighting highlighting) :
            base(highlighting)
        {
        }

        public override string Text => $"Remove argument value '{Highlighting.Argument.NotNull().GetText()}'";

        protected override bool IsAvailableForTreeNode(IUserDataHolder _) => Highlighting.Argument != null;

        [CanBeNull]
        protected override Action<ITextControl> ExecutePsiTransaction(ISolution _, IProgressIndicator __)
        {
            var argument = Highlighting.Argument.NotNull();

            Highlighting.Attribute.RemoveArgument(argument);

            return null;
        }
    }
}
