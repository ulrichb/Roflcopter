using System.Diagnostics.CodeAnalysis;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.Util;

namespace Roflcopter.Plugin.MismatchedFileNames
{
    [QuickFix]
    [ExcludeFromCodeCoverage]
    public class MismatchedFileNameHighlightingQuickFix : MismatchedFileNameHighlightingQuickFixBase
    {
        public MismatchedFileNameHighlightingQuickFix(MismatchedFileNameHighlighting highlighting) :
            base(highlighting)
        {
        }

        protected override void RenameFile(ISolution solution, IProjectFile projectFile, string newFileName)
        {
            using (var transactionCookie = solution.CreateTransactionCookie(DefaultAction.Commit, commandName: Text))
                transactionCookie.Rename(projectFile, newFileName);
        }

        protected override void ShowError(string text, string caption)
        {
            MessageBox.ShowError(text, caption);
        }
    }
}
