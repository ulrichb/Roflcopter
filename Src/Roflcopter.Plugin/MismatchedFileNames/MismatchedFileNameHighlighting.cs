using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;

namespace Roflcopter.Plugin.MismatchedFileNames
{
    [RegisterConfigurableSeverity(
        SeverityId,
        CompoundItemName: null,
        Group: HighlightingGroupIds.CodeSmell,
        Title: Title,
        Description: Description,
        DefaultSeverity: Severity.WARNING)]
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        OverlapResolve = OverlapResolveKind.NONE,
        ToolTipFormatString = Message)]
    public class MismatchedFileNameHighlighting : SimpleTreeNodeHighlightingBase<ITypeDeclaration>
    {
        private const string SeverityId = "MismatchedFileName";
        private const string Title = "Mismatch between type and file name";
        private const string Message = "Type doesn't match file name '{0}'";

        private const string Description = Title;

        public MismatchedFileNameHighlighting(ITypeDeclaration declaration, string currentFileName, string expectedFileName)
            : base(declaration, string.Format(Message, currentFileName))
        {
            CurrentFileName = currentFileName;
            ExpectedFileName = expectedFileName;
        }

        public string CurrentFileName { get; }
        public string ExpectedFileName { get; }

        public override DocumentRange CalculateRange() => HighlightingNode.GetNameDocumentRange();
    }
}
