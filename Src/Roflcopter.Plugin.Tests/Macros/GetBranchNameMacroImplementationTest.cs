using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;
using JetBrains.ReSharper.FeaturesTestFramework.LiveTemplates;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NUnit.Framework;
using Roflcopter.Plugin.Git;
using Roflcopter.Plugin.Macros;
using Roflcopter.Plugin.Tests.Git;

namespace Roflcopter.Plugin.Tests.Macros
{
    [TestFixture]
    [TestNetFramework4]
    public class GetBranchNameMacroImplementationTest : MacroImplTestBase
    {
        private GitRepositoryProvider _gitRepositoryProvider;
        private FileSystemPath _gitDirectory;

        [SetUp]
        public new void SetUp()
        {
            const string gitDirectory = "__git__";
            _gitDirectory = SolutionItemsBasePath.Combine(gitDirectory);
            _gitRepositoryProvider = new TestGitRepositoryProvider(gitDirectory);

            _gitDirectory.CreateDirectory();
        }

        [TearDown]
        public new void TearDown()
        {
            _gitDirectory.Delete();
        }

        protected override IMacroImplementation GetMacro([NotNull] IEnumerable<IMacroParameterValueNew> parameters)
        {
            return new GetBranchNameMacroImplementation(_gitRepositoryProvider, parameters.ToParameters());
        }

        [Test]
        public void Default()
        {
            _gitDirectory.Combine("HEAD").WriteAllText("ref: refs/heads/my_branch");

            DoNamedTest();
        }

        [Test]
        public void StripingRegexParameter()
        {
            _gitDirectory.Combine("HEAD").WriteAllText("ref: refs/heads/spike/my_branch");

            DoNamedTest();
        }

        [Test]
        public void EmptyHeadFile()
        {
            _gitDirectory.Combine("HEAD").WriteAllText("");

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
            _gitDirectory.Delete();

            DoNamedTest();
        }
    }
}
