using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Templates;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

namespace Roflcopter.Plugin.Utilities
{
    public static class TextControlExtensions
    {
        public static HotspotSession CreateHotspotSessionAtopExistingText(
            this ITextControl textControl,
            ISolution solution,
            DocumentRange endSelectionRange,
            params ITreeNode[] hotspotNodes)
        {
            var hotspotDocumentRanges = hotspotNodes.Select(x => x.GetDocumentRange()).ToArray();

            return CreateHotspotSessionAtopExistingText(textControl, solution, endSelectionRange, hotspotDocumentRanges);
        }

        private static HotspotSession CreateHotspotSessionAtopExistingText(
            this ITextControl textControl,
            ISolution solution,
            DocumentRange endSelectionRange,
            params DocumentRange[] hotspotDocumentRanges)
        {
            var escapeAction = LiveTemplatesManager.EscapeAction.LeaveTextAndCaret;
            var hotspotInfos = hotspotDocumentRanges.Select(x => new HotspotInfo(new TemplateField(x.GetText(), 0), x)).ToArray();

            return LiveTemplatesManager.Instance.CreateHotspotSessionAtopExistingText(
                solution, endSelectionRange, textControl, escapeAction, hotspotInfos);
        }
    }
}
