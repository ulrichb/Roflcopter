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
            "GitHub (master): {GitRepoOriginUrl}/blob/master/{PathRelativeToGitRepoSlashSeparated}\n" +
            "GitHub (current branch): {GitRepoOriginUrl}/blob/{GitRepoBranch}/{PathRelativeToGitRepoSlashSeparated}",
            "URL templates\n\nReplacements:\n" +
            "{PathRelativeToSolutionSlashSeparated}" + "\n" +
            "{GitRepoOriginUrl}" + "\n" +
            "{GitRepoBranch}" + "\n" +
            "{PathRelativeToGitRepoSlashSeparated}"
        )]
        public readonly string UrlTemplates;
    }
}
