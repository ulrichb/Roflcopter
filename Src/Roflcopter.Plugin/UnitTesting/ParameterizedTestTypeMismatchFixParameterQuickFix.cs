using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.TextControl;
using JetBrains.Util;

namespace Roflcopter.Plugin.UnitTesting
{
    [QuickFix]
    public class ParameterizedTestTypeMismatchFixParameterQuickFix : QuickFixBase
    {
        private readonly ParameterizedTestTypeMismatchHighlighting _highlighting;

        public ParameterizedTestTypeMismatchFixParameterQuickFix(ParameterizedTestTypeMismatchHighlighting highlighting)
        {
            _highlighting = highlighting;
        }

        public override string Text => $"Change parameter type to '{_highlighting.ArgumentExpression.Type()}'";

        public override bool IsAvailable(IUserDataHolder _) => true;

        [CanBeNull]
        protected override Action<ITextControl> ExecutePsiTransaction(ISolution _, IProgressIndicator __)
        {
            _highlighting.ParameterDeclaration.SetType(_highlighting.ArgumentExpression.Type());

            return BulbActionUtils.SetSelection(_highlighting.ParameterDeclaration);
        }
    }
}
