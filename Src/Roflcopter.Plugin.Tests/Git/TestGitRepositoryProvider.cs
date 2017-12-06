using System;
using Roflcopter.Plugin.Git;

namespace Roflcopter.Plugin.Tests.Git
{
    internal class TestGitRepositoryProvider : GitRepositoryProvider
    {
        public TestGitRepositoryProvider(string gitDirectoryName = null)
        {
            GitDirectoryName = gitDirectoryName ?? $"__git_{Guid.NewGuid()}__";
        }

        public override string GitDirectoryName { get; }
    }
}
