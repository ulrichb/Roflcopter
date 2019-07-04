using System.Diagnostics.CodeAnalysis;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.IDE.UI.Extensions;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.Daemon.OptionPages;
using JetBrains.ReSharper.Feature.Services.Resources;

namespace Roflcopter.Plugin.MismatchedFileNames.OptionsPages
{
    [OptionsPage(
        OptionsPageId,
        "Mismatched file names",
        typeofIcon: typeof(AlteringFeatuThemedIcons.FileHeaderText),
        ParentId = CodeInspectionPage.PID)]
    [ExcludeFromCodeCoverage /* manually tested UI code */]
    public class MismatchedFileNamesOptionsPage : BeSimpleOptionsPage
    {
        private const string OptionsPageId = nameof(MismatchedFileNamesOptionsPage);

        public MismatchedFileNamesOptionsPage(
            Lifetime lifetime,
            OptionsPageContext optionsPageContext,
            OptionsSettingsSmartContext optionsSettingsSmartContext) : base(lifetime, optionsPageContext, optionsSettingsSmartContext)
        {
            var allowedFileNamePostfixRegex =
                OptionsSettingsSmartContext.GetValueProperty(lifetime, (MismatchedFileNamesSettings s) => s.AllowedFileNamePostfixRegex);

            AddControl(allowedFileNamePostfixRegex.GetBeTextBox(lifetime).WithDescription("Allowed file name postfix regex: ", lifetime));
        }
    }
}
