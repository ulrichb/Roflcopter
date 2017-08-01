using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperExtensionsShared.Highlighting;
using Roflcopter.Plugin.MismatchedFileNames;

[assembly: RegisterConfigurableSeverity(
    MismatchedFileNameHighlighting.SeverityId,
    CompoundItemName: null,
    Group: HighlightingGroupIds.CodeSmell,
    Title: MismatchedFileNameHighlighting.Title,
    Description: MismatchedFileNameHighlighting.Description,
    DefaultSeverity: Severity.WARNING)]

namespace Roflcopter.Plugin.MismatchedFileNames
{
    /// <summary>
    /// Xml Doc highlighting for types / type members with specific accessibility.
    /// </summary>
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        OverlapResolve = OverlapResolveKind.NONE,
        ToolTipFormatString = Message)]
        public class MismatchedFileNameHighlighting : SimpleTreeNodeHighlightingBase<ITypeDeclaration>
    {
        public const string SeverityId = "MismatchedFileName";
        public const string Title = "Mismatch between type and file name";
        private const string Message = "Type doesn't match file name '{0}'";

        public const string Description = Title;    

        public MismatchedFileNameHighlighting(ITypeDeclaration declaration, string fileName)
            : base(declaration, string.Format(Message, fileName))
        {
        }

        public override DocumentRange CalculateRange()
        {
            return TreeNode.GetNameDocumentRange();
        }
    }
}
