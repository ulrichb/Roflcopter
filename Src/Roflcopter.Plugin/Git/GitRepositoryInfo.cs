using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.Util;

namespace Roflcopter.Plugin.Git
{
    public class GitRepositoryInfo
    {
        public GitRepositoryInfo(VirtualFileSystemPath gitDirectory)
        {
            GitDirectory = gitDirectory;
        }

        /// <summary>The ".git" directory.</summary>
        private VirtualFileSystemPath GitDirectory { get; }

        public VirtualFileSystemPath RepositoryDirectory => GitDirectory.Directory;

        private VirtualFileSystemPath HeadFile => GitDirectory.Combine("HEAD");

        private VirtualFileSystemPath ConfigFile => GitDirectory.Combine("config");

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
