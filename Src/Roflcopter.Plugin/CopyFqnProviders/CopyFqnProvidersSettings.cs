using System.Diagnostics.CodeAnalysis;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Navigation.Options;

namespace Roflcopter.Plugin.CopyFqnProviders
{
    [SettingsKey(typeof(SearchAndNavigationSettings), "Copy names to clipboard")]
    [SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized")]
    public class CopyFqnProvidersSettings
    {
        [SettingsEntry(true, "Enable short names")]
        public readonly bool EnableShortNames;

        [SettingsEntry(
            "",
            "URL templates\n\nReplacements:\n" +
            "{PathRelativeToSolutionSlashSeparated}" + "\n" +
            "{GitRepoOriginUrl}" + "\n" +
            "{GitRepoBranch}" + "\n" +
            "{PathRelativeToGitRepoSlashSeparated}"
        )]
        public readonly string UrlTemplates;
    }
}
