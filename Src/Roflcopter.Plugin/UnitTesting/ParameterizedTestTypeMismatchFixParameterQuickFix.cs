using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharperExtensionsShared.QuickFixes;

namespace Roflcopter.Plugin.UnitTesting
{
    [QuickFix]
    public class ParameterizedTestTypeMismatchFixParameterQuickFix :
        SimpleQuickFixBase<ParameterizedTestTypeMismatchHighlighting, ITreeNode>
    {
        public ParameterizedTestTypeMismatchFixParameterQuickFix(ParameterizedTestTypeMismatchHighlighting highlighting) : base(highlighting)
        {
        }

        public override string Text =>
            $"Change parameter type to '{Highlighting.ArgumentExpression.Type().GetPresentableName(CSharpLanguage.Instance)}'";

        protected override bool IsAvailableForTreeNode(IUserDataHolder _) => true;

        [CanBeNull]
        protected override Action<ITextControl> ExecutePsiTransaction(ISolution _, IProgressIndicator __)
        {
            Highlighting.ParameterDeclaration.SetType(Highlighting.ArgumentExpression.Type());

            return BulbActionUtils.SetSelection(Highlighting.ParameterDeclaration);
        }
    }
}
