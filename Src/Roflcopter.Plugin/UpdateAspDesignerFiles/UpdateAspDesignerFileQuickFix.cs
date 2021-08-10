#if RESHARPER
using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.Asp.Highlightings;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace Roflcopter.Plugin.UpdateAspDesignerFiles
{
    [QuickFix]
    public class UpdateAspDesignerFileQuickFix : QuickFixBase
    {
        [NotNull]
        private readonly IReference _reference;

        public UpdateAspDesignerFileQuickFix([NotNull] AspNotResolvedErrorHighlighting<IReference> error)
        {
            _reference = error.Reference;
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
            var treeNode = _reference.GetTreeNode();

            var updateAspDesignerFileService = treeNode.GetSolution().GetComponent<UpdateAspDesignerFileService>();

            return updateAspDesignerFileService.GetGenerator(treeNode.GetSourceFile());
        }
    }
}
#endif
