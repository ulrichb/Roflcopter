using JetBrains.ProjectModel;
using JetBrains.Util;

namespace Roflcopter.Plugin.Tests
{
    internal class FixedSolutionPathProvider : SolutionPathProvider
    {
        public FixedSolutionPathProvider(VirtualFileSystemPath solutionDirectory)
        {
            SolutionDirectory = solutionDirectory;
        }

        private VirtualFileSystemPath SolutionDirectory { get; }

        public override VirtualFileSystemPath GetSolutionDirectory(ISolution solution) => SolutionDirectory;
    }
}
