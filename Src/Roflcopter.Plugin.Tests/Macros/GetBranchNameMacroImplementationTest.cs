using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;
using JetBrains.ReSharper.FeaturesTestFramework.LiveTemplates;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NUnit.Framework;
using Roflcopter.Plugin.Macros;

namespace Roflcopter.Plugin.Tests.Macros
{
    [TestFixture]
    [TestNetFramework4]
    public class GetBranchNameMacroImplementationTest : MacroImplTestBase
    {
        private readonly PathProvider _pathProvider = new PathProvider();

        [SetUp]
        public new void SetUp()
        {
            Directory.CreateDirectory(_pathProvider.GitDirectoryName);
        }

        [TearDown]
        public new void TearDown()
        {
            if (Directory.Exists(_pathProvider.GitDirectoryName))
                Directory.Delete(_pathProvider.GitDirectoryName, recursive: true);
        }

        protected override IMacroImplementation GetMacro([NotNull] IEnumerable<IMacroParameterValueNew> parameters)
        {
            return new GetBranchNameMacroImplementation(_pathProvider, parameters.ToParameters());
        }

        [Test]
        public void Default()
        {
            File.WriteAllText(Path.Combine(_pathProvider.GitDirectoryName, "HEAD"), "ref: refs/heads/my_branch");

            DoNamedTest();
        }

        [Test]
        public void StripingRegexParameter()
        {
            File.WriteAllText(Path.Combine(_pathProvider.GitDirectoryName, "HEAD"), "ref: refs/heads/spike/my_branch");

            DoNamedTest();
        }

        [Test]
        public void EmptyHeadFile()
        {
            File.WriteAllText(Path.Combine(_pathProvider.GitDirectoryName, "HEAD"), "");

            DoNamedTest();
        }

        [Test]
        public void NoHeadFile()
        {
            DoNamedTest();
        }

        [Test]
        public void NoGitDirectory()
        {
            Directory.Delete(_pathProvider.GitDirectoryName);

            DoNamedTest();
        }

        private class PathProvider : IGetBranchNameMacroPathProvider
        {
            public string GitDirectoryName => "__git__";
            public FileSystemPath GetSolutionDirectory(ISolution solution) => FileSystemPath.Parse(Environment.CurrentDirectory);
        }
    }
}
