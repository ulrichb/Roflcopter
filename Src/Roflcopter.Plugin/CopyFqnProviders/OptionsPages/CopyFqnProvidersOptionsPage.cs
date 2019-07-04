#if RESHARPER
using System.Diagnostics.CodeAnalysis;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.Options.ThemedIcons;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.IDE.UI.Extensions;
using JetBrains.IDE.UI.Options;
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
    public class CopyFqnProvidersOptionsPage : BeSimpleOptionsPage
    {
        private const string PageId = nameof(CopyFqnProvidersOptionsPage);

        public CopyFqnProvidersOptionsPage(
            Lifetime lifetime,
            OptionsPageContext optionsPageContext,
            OptionsSettingsSmartContext optionsSettingsSmartContext) : base(lifetime, optionsPageContext, optionsSettingsSmartContext)
        {
            AddText("\"Copy Fully-qualified name/ Source browser URI to Clipboard\" will be extended by the following entries.");

            AddSpacer();

            AddBoolOption((CopyFqnProvidersSettings s) => s.EnableShortNames, "Short names");

            AddSpacer();

            AddText("URL templates:");

            // Use BeControls.GetTextControl() + manual Bind() instead of GetBeTextBox() to support multi-line
            // editing (no handling of the "Enter" key). Just like the `ReSpellerSettingsPage`.

            var textControl = BeControls.GetTextControl(isReadonly: false);

            OptionsSettingsSmartContext
                .GetValueProperty(Lifetime, (CopyFqnProvidersSettings s) => s.UrlTemplates)
                .Bind(Lifetime, textControl.Text);

            AddControl(textControl);
        }
    }
}
#endif
