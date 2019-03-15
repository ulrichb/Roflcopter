using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharperExtensionsShared.QuickFixes;
#if !RS20183
using JetBrains.Diagnostics;

#endif

namespace Roflcopter.Plugin.MismatchedFileNames
{
    public abstract class MismatchedFileNameHighlightingQuickFixBase : SimpleQuickFixBase<MismatchedFileNameHighlighting, ITypeDeclaration>
    {
        protected MismatchedFileNameHighlightingQuickFixBase(MismatchedFileNameHighlighting highlighting) : base(highlighting)
        {
        }

        public override string Text => $"Rename file to '{Highlighting.ExpectedFileName}'";

        protected override bool IsAvailableForTreeNode(IUserDataHolder cache) => true;

        [CanBeNull]
        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var newFileName = Highlighting.ExpectedFileName;
            var isOnlyACasingRenaming = FileSystemDefinition.PathStringEquality.Equals(newFileName, Highlighting.CurrentFileName);

            var psiSourceFile = Highlighting.HighlightingNode.GetSourceFile();
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
