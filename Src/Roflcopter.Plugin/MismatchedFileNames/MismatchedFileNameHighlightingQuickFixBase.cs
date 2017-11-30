using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.Util;

namespace Roflcopter.Plugin.MismatchedFileNames
{
    public abstract class MismatchedFileNameHighlightingQuickFixBase : QuickFixBase
    {
        private readonly MismatchedFileNameHighlighting _highlighting;

        protected MismatchedFileNameHighlightingQuickFixBase(MismatchedFileNameHighlighting highlighting)
        {
            _highlighting = highlighting;
        }

        public override string Text => $"Rename file to '{_highlighting.ExpectedFileName}'";

        public override bool IsAvailable(IUserDataHolder cache) => true;

        [CanBeNull]
        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var newFileName = _highlighting.ExpectedFileName;
#if RS20171
            var isOnlyACasingRenaming = StringComparer.OrdinalIgnoreCase.Equals(newFileName, _highlighting.CurrentFileName);
#else
            var isOnlyACasingRenaming = FileSystemDefinition.PathStringEquality.Equals(newFileName, _highlighting.CurrentFileName);
#endif

            var psiSourceFile = _highlighting.HighlightingNode.GetSourceFile();
            var projectFile = psiSourceFile.ToProjectFile().NotNull("psiSourceFile.ToProjectFile() != null");

            return _ =>
            {
                if (!isOnlyACasingRenaming && projectFile.Location.Directory.Combine(newFileName).ExistsFile)
                {
                    ShowError($"A file '{newFileName}' already exists.", $"Can't rename '{projectFile.Location.Name}'");
                }
                else
                {
                    RenameFile(solution, projectFile, newFileName);
                }
            };
        }

        protected abstract void RenameFile(ISolution solution, IProjectFile projectFile, string newFileName);
        protected abstract void ShowError(string text, string caption);
    }
}
