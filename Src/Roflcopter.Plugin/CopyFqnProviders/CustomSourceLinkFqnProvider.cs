using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Features.Environment.CopyFqn;
using JetBrains.Util;
using Roflcopter.Plugin.Git;

namespace Roflcopter.Plugin.CopyFqnProviders
{
    [SolutionComponent]
    public class CustomSourceLinkFqnProvider : IFqnProvider
    {
        private readonly ISolution _solution;
        private readonly SolutionPathProvider _solutionPathProvider;
        private readonly GitRepositoryProvider _gitRepositoryProvider;

        public CustomSourceLinkFqnProvider(ISolution solution, SolutionPathProvider solutionPathProvider, GitRepositoryProvider gitRepositoryProvider)
        {
            _solution = solution;
            _solutionPathProvider = solutionPathProvider;
            _gitRepositoryProvider = gitRepositoryProvider;
        }

        public bool IsApplicable([NotNull] IDataContext _)
        {
            // Will be called in the CopyFqnAction to determine if _any_ provider has sth. to provide.

            return GetUrlTemplates().Any();
        }

        public IEnumerable<PresentableFqn> GetSortedFqns([NotNull] IDataContext dataContext)
        {
            var projectItem = dataContext.GetData(ProjectModelDataConstants.PROJECT_MODEL_ELEMENT) as IProjectItem;

            var uriTemplates = GetUrlTemplates();

            foreach (var urlTemplate in uriTemplates)
            {
                var projectItemPath = projectItem.NotNull("projectItem != null").Location.GetLongPath();

                var url = urlTemplate.Value;
                url = ExecuteSolutionPathReplacements(projectItemPath, url);
                url = ExecuteGitRepositoryReplacements(projectItemPath, url);

                yield return new PresentableFqn(urlTemplate.Name ?? url, url);
            }
        }

        private IEnumerable<UrlTemplate> GetUrlTemplates()
        {
            var urlTemplatesText = _solution.GetSettingsStore().GetValue((CopyFqnProvidersSettings s) => s.UrlTemplates);

            return
                from urlTemplateText in urlTemplatesText.SplitByNewLine(options: StringSplitOptions.RemoveEmptyEntries)
                let match = Regex.Match(urlTemplateText, @"^\s*((?<Name>.+?)\s*:\s+)?(?<UrlTemplate>\S.+?)\s*$")
                where match.Success
                select new UrlTemplate(
                    match.Groups["Name"].Success ? match.Groups["Name"].Value : null,
                    match.Groups["UrlTemplate"].Value);
        }

        private string ExecuteSolutionPathReplacements(FileSystemPath projectItemPath, string url)
        {
            var solutionDirectory = _solutionPathProvider.GetSolutionDirectory(_solution);
            var pathRelativeToSolution = projectItemPath.TryMakeRelativeTo(solutionDirectory);

            return url.Replace("{PathRelativeToSolutionSlashSeparated}", GetSlashSeparated(pathRelativeToSolution));
        }

        private string ExecuteGitRepositoryReplacements(FileSystemPath projectItemPath, string url)
        {
            var gitRepositoryInfo = _gitRepositoryProvider.FetchGitRepositoryInfo(projectItemPath.Directory);

            var gitRepositoryOrigin = gitRepositoryInfo != null ? GetGitRepositoryOrigin(gitRepositoryInfo) : null;
            url = url.Replace("{GitRepoOriginUrl}", gitRepositoryOrigin ?? "<cannot find Git repository origin>");

            var gitRepoBranch = gitRepositoryInfo?.ReadHeadFileReference();
            url = url.Replace("{GitRepoBranch}", gitRepoBranch ?? "<cannot find Git branch>");

            var pathRelativeToGitRepoSlashSeparated =
                gitRepositoryInfo == null ? null : GetSlashSeparated(projectItemPath.TryMakeRelativeTo(gitRepositoryInfo.RepositoryDirectory));

            url = url.Replace("{PathRelativeToGitRepoSlashSeparated}", pathRelativeToGitRepoSlashSeparated ?? "<cannot find Git repository>");

            return url;
        }

        [CanBeNull]
        private string GetGitRepositoryOrigin(GitRepositoryInfo gitRepositoryInfo)
        {
            var configFileText = gitRepositoryInfo.ReadConfigFile();

            // IDEA: Parse .INI and use '[remote "origin"]'
            return Regex.Matches(configFileText, @"\bhttps?://.+\b").Cast<Match>().Select(x => x.Value).FirstOrDefault();
        }

        private static string GetSlashSeparated(IPath path) => path.AssertRelative().FullPath.Replace('\\', '/');

        public int Priority => +10; // The higher the value, the _lower_ it is ranked. Yes, really.

        private struct UrlTemplate
        {
            public UrlTemplate([CanBeNull] string name, string value)
            {
                Name = name;
                Value = value;
            }

            [CanBeNull]
            public string Name { get; }

            public string Value { get; }
        }
    }
}
