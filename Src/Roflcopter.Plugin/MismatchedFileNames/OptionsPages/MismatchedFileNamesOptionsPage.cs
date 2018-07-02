using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.DataFlow;
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
#pragma warning disable 618
    // TODO after dropping 20181 support: Refactor to BeSimpleOptionsPage
    public class MismatchedFileNamesOptionsPage : SimpleOptionsPage
#pragma warning restore 618
    {
        private const string OptionsPageId = nameof(MismatchedFileNamesOptionsPage);

        public MismatchedFileNamesOptionsPage([NotNull] Lifetime lifetime, [NotNull] OptionsSettingsSmartContext optionsSettingsSmartContext) :
            base(lifetime, optionsSettingsSmartContext)
        {
            AddStringOption((MismatchedFileNamesSettings s) => s.AllowedFileNamePostfixRegex, "Allowed file name postfix regex: ");

            FinishPage();
        }
    }
}
