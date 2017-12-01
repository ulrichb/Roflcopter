using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharperExtensionsShared.QuickFixes;

namespace Roflcopter.Plugin.UnitTesting
{
    [QuickFix]
    public class ParameterizedTestMissingParameterAddParameterQuickFix :
        SimpleQuickFixBase<ParameterizedTestMissingParameterHighlighting, ITreeNode>
    {
        public ParameterizedTestMissingParameterAddParameterQuickFix(ParameterizedTestMissingParameterHighlighting highlighting) : base(highlighting)
        {
        }

        public override string Text => $"Add '{Highlighting.ArgumentExpression.Type().GetPresentableName(CSharpLanguage.Instance)}' parameter";

        protected override bool IsAvailableForTreeNode(IUserDataHolder _) => Highlighting.IsFirstMissingParameter;

        [CanBeNull]
        protected override Action<ITextControl> ExecutePsiTransaction(ISolution _, IProgressIndicator __)
        {
            var methodDeclaration = Highlighting.MethodDeclaration;

            var parameterDeclaration = methodDeclaration.AddParameterDeclarationBefore(
                ParameterKind.VALUE,
                parameterType: Highlighting.ArgumentExpression.Type(),
                parameterName: "newParameter",
                anchor: null);

            return BulbActionUtils.SetSelection(parameterDeclaration.GetNameDocumentRange());
        }
    }
}
