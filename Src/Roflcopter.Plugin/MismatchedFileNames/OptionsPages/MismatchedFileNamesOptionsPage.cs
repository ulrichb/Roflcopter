using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Feature.Services.Daemon.OptionPages;
#if RS20171
using JetBrains.UI.Resources;
using JetBrains.UI.Options;
using JetBrains.UI.Options.OptionsDialog2.SimpleOptions;

#else
using JetBrains.Application.UI.Icons.CommonThemedIcons;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;

#endif

namespace Roflcopter.Plugin.MismatchedFileNames.OptionsPages
{
    [OptionsPage(
        nameof(MismatchedFileNamesOptionsPage), "Mismatched file names", typeof(CommonThemedIcons.Bulb), ParentId = CodeInspectionPage.PID)]
    [ExcludeFromCodeCoverage /* manually tested UI code */]
    public class MismatchedFileNamesOptionsPage : SimpleOptionsPage
    {
        public MismatchedFileNamesOptionsPage([NotNull] Lifetime lifetime, [NotNull] OptionsSettingsSmartContext optionsSettingsSmartContext) :
            base(lifetime, optionsSettingsSmartContext)
        {
            AddStringOption((MismatchedFileNamesSettings s) => s.AllowedFileNamePostfixRegex, "Allowed file name postfix regex: ");

            FinishPage();
        }
    }
}
