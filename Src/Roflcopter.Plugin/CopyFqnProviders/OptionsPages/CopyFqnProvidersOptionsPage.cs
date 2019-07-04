#if RESHARPER
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.Options.ThemedIcons;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Features.Navigation.Options;

namespace Roflcopter.Plugin.CopyFqnProviders.OptionsPages
{
    [OptionsPage(
        PageId,
        "Copy names to clipboard",
        typeof(OptionsThemedIcons.CopySettings),
        ParentId = SearchAndNavigationOptionPage.PID)]
    [ExcludeFromCodeCoverage /* manually tested UI code */]
#pragma warning disable 618
    // TODO: Refactor to BeSimpleOptionsPage
    public class CopyFqnProvidersOptionsPage : SimpleOptionsPage
#pragma warning restore 618
    {
        private const string PageId = nameof(CopyFqnProvidersOptionsPage);

        public CopyFqnProvidersOptionsPage(
            Lifetime lifetime, [NotNull] OptionsSettingsSmartContext optionsSettingsSmartContext) :
            base(lifetime, optionsSettingsSmartContext)
        {
            AddText("\"Copy Fully-qualified name/ Source browser URI to Clipboard\" will be extended by the following entries.");
            AddText("");

            AddBoolOption((CopyFqnProvidersSettings s) => s.EnableShortNames, "Short names");

            AddText("");

            AddText("URL templates:");
            AddStringOption((CopyFqnProvidersSettings s) => s.UrlTemplates, "", acceptsReturn: true);

            FinishPage();
        }
    }
}
#endif
