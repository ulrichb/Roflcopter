using System.Collections.Generic;
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
            IEnumerable<ITreeNode> hotspotNodes,
            DocumentRange endSelectionRange)
        {
            var escapeAction = LiveTemplatesManager.EscapeAction.LeaveTextAndCaret;

            var hotspotInfos = hotspotNodes.Select(x => new HotspotInfo(new TemplateField(x.GetText(), 0), x.GetDocumentRange()));

            return LiveTemplatesManager.Instance.CreateHotspotSessionAtopExistingText(
                solution, endSelectionRange, textControl, escapeAction, hotspotInfos.ToArray());
        }
    }
}
