using System.Diagnostics.CodeAnalysis;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace Roflcopter.Plugin.Macros
{
    public interface IGetBranchNameMacroPathProvider
    {
        string GitDirectoryName { get; }
        FileSystemPath GetSolutionDirectory(ISolution solution);
    }

    [ShellComponent]
    [ExcludeFromCodeCoverage /* this provider is stubbed in the tests */]
    public class GetBranchNameMacroPathProvider : IGetBranchNameMacroPathProvider
    {
        public string GitDirectoryName => ".git";
        public FileSystemPath GetSolutionDirectory(ISolution solution) => solution.SolutionFilePath.Directory;
    }
}
