using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Parts;
using JetBrains.Util;

namespace Roflcopter.Plugin.Git
{
    [ShellComponent(Instantiation.DemandAnyThreadSafe)]
    public class GitRepositoryProvider
    {
        [CanBeNull]
        public GitRepositoryInfo FetchGitRepositoryInfo(VirtualFileSystemPath directory)
        {
            var gitDirectory = FindGitDirectory(directory);

            if (gitDirectory == null)
                return null;

            return new GitRepositoryInfo(gitDirectory);
        }

        [CanBeNull]
        private VirtualFileSystemPath FindGitDirectory(VirtualFileSystemPath directory)
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
