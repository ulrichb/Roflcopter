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
    public class ParameterizedTestMissingArgumentRemoveParameterQuickFix :
        SimpleQuickFixBase<ParameterizedTestMissingArgumentHighlighting, ITreeNode>
    {
        public ParameterizedTestMissingArgumentRemoveParameterQuickFix(ParameterizedTestMissingArgumentHighlighting highlighting) : base(highlighting)
        {
        }

        public override string Text => $"Remove parameter '{Highlighting.ParameterDeclaration.DeclaredName}'";

        protected override bool IsAvailableForTreeNode(IUserDataHolder _) => true;

        [CanBeNull]
        protected override Action<ITextControl> ExecutePsiTransaction(ISolution _, IProgressIndicator __)
        {
            Highlighting.MethodDeclaration.RemoveParameterDeclaration(Highlighting.ParameterDeclaration);

            return null;
        }
    }
}
