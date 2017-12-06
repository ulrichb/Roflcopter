using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.Util;

namespace Roflcopter.Plugin.Git
{
    public class GitRepositoryInfo
    {
        public GitRepositoryInfo(FileSystemPath gitDirectory)
        {
            GitDirectory = gitDirectory;
        }

        /// <summary>The ".git" directory.</summary>
        private FileSystemPath GitDirectory { get; }

        public FileSystemPath RepositoryDirectory => GitDirectory.Directory;

        private FileSystemPath HeadFile => GitDirectory.Combine("HEAD");

        private FileSystemPath ConfigFile => GitDirectory.Combine("config");

        [CanBeNull]
        public string ReadHeadFileReference()
        {
            var headFile = HeadFile;

            if (!headFile.ExistsFile)
                return null;

            var headFileContent = File.ReadAllLines(headFile.FullPath).DefaultIfEmpty("").First();

            if (string.IsNullOrWhiteSpace(headFileContent))
                return null;

            return Regex.Replace(headFileContent, @"^ref:\s*refs/heads/", "");
        }

        public string ReadConfigFile()
        {
            return ConfigFile.ReadAllText2().Text;
        }
    }
}
