using System.Diagnostics.CodeAnalysis;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace Roflcopter.Plugin
{
    [ShellComponent]
    public class SolutionPathProvider
    {
        [ExcludeFromCodeCoverage /* this is stubbed in the tests */]
        public virtual FileSystemPath GetSolutionDirectory(ISolution solution) => solution.SolutionFilePath.Directory;
    }
}
