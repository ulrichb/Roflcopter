using JetBrains.ProjectModel;
using JetBrains.Util;

namespace Roflcopter.Plugin.Tests
{
    internal class FixedSolutionPathProvider : SolutionPathProvider
    {
        public FixedSolutionPathProvider(FileSystemPath solutionDirectory)
        {
            SolutionDirectory = solutionDirectory;
        }

        private FileSystemPath SolutionDirectory { get; }

        public override FileSystemPath GetSolutionDirectory(ISolution solution) => SolutionDirectory;
    }
}
