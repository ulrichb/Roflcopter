using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Util;

namespace Roflcopter.Plugin.Git
{
    [ShellComponent]
    public class GitRepositoryProvider
    {
        [CanBeNull]
        public GitRepositoryInfo FetchGitRepositoryInfo(FileSystemPath directory)
        {
            var gitDirectory = FindGitDirectory(directory);

            if (gitDirectory == null)
                return null;

            return new GitRepositoryInfo(gitDirectory);
        }

        [CanBeNull]
        private FileSystemPath FindGitDirectory(FileSystemPath directory)
        {
            do
            {
                var gitDirectory = directory.Combine(GitDirectoryName);

                // IDEA: Add support for ".git"-file redirect

                if (gitDirectory.ExistsDirectory)
                    return gitDirectory;

                directory = directory.Parent;
            } while (!directory.IsEmpty);

            return null;
        }

        [ExcludeFromCodeCoverage /* this is stubbed in the tests */]
        public virtual string GitDirectoryName => ".git";
    }
}
