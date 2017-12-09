using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharperExtensionsShared.QuickFixes;
using Roflcopter.Plugin.Utilities;

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
        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator _)
        {
            var methodDeclaration = Highlighting.MethodDeclaration;

            var elementFactory = CSharpElementFactory.GetInstance(methodDeclaration);

            var parameterDeclaration = methodDeclaration.AddParameterDeclarationBefore(
                elementFactory.CreateParameterDeclaration(
                    ParameterKind.VALUE,
                    isParametric: false,
                    isVarArg: false,
                    type: Highlighting.ArgumentExpression.Type(),
                    name: "newParameter",
                    defaultValue: null), anchor: null);

            return textControl =>
            {
                var endSelectionRange = DocumentRange.InvalidRange;
                var hotspotNode = parameterDeclaration.NameIdentifier;

                var hotspotSession = textControl.CreateHotspotSessionAtopExistingText(solution, endSelectionRange, hotspotNode);

                hotspotSession.Execute();
            };
        }
    }
}
