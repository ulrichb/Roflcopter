#if RESHARPER
using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Asp.ContextActions;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.Html.Bulbs;
using JetBrains.ReSharper.Psi.Asp.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace Roflcopter.Plugin.UpdateAspDesignerFiles
{
    [ContextAction(
        GroupType = typeof(AspContextActions),
        Name = UpdateAspDesignerFileService.ActionDescription,
        Description = UpdateAspDesignerFileService.ActionDescription,
        Priority = -5)]
    public class UpdateAspDesignerFileContextAction : ContextActionBase
    {
        private readonly IContextActionDataProvider _contextActionDataProvider;

        public UpdateAspDesignerFileContextAction(IWebContextActionDataProvider<IAspFile> contextActionDataProvider)
        {
            _contextActionDataProvider = contextActionDataProvider;
        }

        public override string Text => UpdateAspDesignerFileService.ActionDescription;

        public override bool IsAvailable(IUserDataHolder _) => GetGenerator() != null;

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) => null;

        protected override Action<ITextControl> ExecuteAfterPsiTransaction(
            ISolution solution,
            IProjectModelTransactionCookie cookie,
            IProgressIndicator progress)
        {
            var generator = GetGenerator().NotNull("Generator is not available");

            return generator.GenerateAndUpdateDesignerDocument(cookie);
        }

        [CanBeNull]
        private UpdateAspDesignerFileService.Generator GetGenerator()
        {
            var updateAspDesignerFileService = _contextActionDataProvider.PsiServices.GetComponent<UpdateAspDesignerFileService>();

            return updateAspDesignerFileService.GetGenerator(_contextActionDataProvider.SourceFile);
        }
    }
}
#endif
