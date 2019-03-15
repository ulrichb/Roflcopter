using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.ReSharper.Feature.Services.Daemon.OptionPages;
using JetBrains.ReSharper.Feature.Services.Resources;
#if RS20183
using JetBrains.DataFlow;
#else
using JetBrains.Lifetimes;

#endif

namespace Roflcopter.Plugin.MismatchedFileNames.OptionsPages
{
    [OptionsPage(
        OptionsPageId,
        "Mismatched file names",
        typeofIcon: typeof(AlteringFeatuThemedIcons.FileHeaderText),
        ParentId = CodeInspectionPage.PID)]
    [ExcludeFromCodeCoverage /* manually tested UI code */]
#pragma warning disable 618
    // TODO: Refactor to BeSimpleOptionsPage
    public class MismatchedFileNamesOptionsPage : SimpleOptionsPage
#pragma warning restore 618
    {
        private const string OptionsPageId = nameof(MismatchedFileNamesOptionsPage);

        public MismatchedFileNamesOptionsPage(
#if RS20183
            [NotNull] 
#endif
            Lifetime lifetime, [NotNull] OptionsSettingsSmartContext optionsSettingsSmartContext) :
            base(lifetime, optionsSettingsSmartContext)
        {
            AddStringOption((MismatchedFileNamesSettings s) => s.AllowedFileNamePostfixRegex, "Allowed file name postfix regex: ");

            FinishPage();
        }
    }
}
