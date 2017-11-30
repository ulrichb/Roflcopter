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

namespace Roflcopter.Plugin.UnitTesting
{
    [QuickFix]
    public class ParameterizedTestMissingParameterAddParameterQuickFix : QuickFixBase
    {
        private readonly ParameterizedTestMissingParameterHighlighting _highlighting;

        public ParameterizedTestMissingParameterAddParameterQuickFix(ParameterizedTestMissingParameterHighlighting highlighting)
        {
            _highlighting = highlighting;
        }

        public override string Text =>
            $"Add '{_highlighting.ArgumentExpression.Type().GetPresentableName(CSharpLanguage.Instance)}' parameter";

        public override bool IsAvailable(IUserDataHolder _) => _highlighting.IsFirstMissingParameter;

        [CanBeNull]
        protected override Action<ITextControl> ExecutePsiTransaction(ISolution _, IProgressIndicator __)
        {
            var methodDeclaration = _highlighting.MethodDeclaration;

            var parameterDeclaration = methodDeclaration.AddParameterDeclarationBefore(
                ParameterKind.VALUE,
                parameterType: _highlighting.ArgumentExpression.Type(),
                parameterName: "newParameter",
                anchor: null);

            return BulbActionUtils.SetSelection(parameterDeclaration.GetNameDocumentRange());
        }
    }
}
