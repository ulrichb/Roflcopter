using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NUnit.Framework;
using Roflcopter.Plugin.CopyFqnProviders;
using Roflcopter.Plugin.Git;
using Roflcopter.Plugin.Tests.Git;

namespace Roflcopter.Plugin.Tests.CopyFqnProviders
{
    [TestNetFramework4]
    public class CustomSourceLinkFqnProviderTest : FqnProviderTestBase<CustomSourceLinkFqnProvider>
    {
        private FixedSolutionPathProvider _solutionPathProvider;
        private GitRepositoryProvider _gitRepositoryProvider;
        private FileSystemPath _gitDirectoryPath;

        [SetUp]
        public new void SetUp()
        {
            _solutionPathProvider = new FixedSolutionPathProvider(TestDataPath.Parent.Parent.Parent);

            _gitRepositoryProvider = new TestGitRepositoryProvider();
            _gitDirectoryPath = TestDataPath.Parent.Combine(_gitRepositoryProvider.GitDirectoryName);

            _gitDirectoryPath.CreateDirectory();
            _gitDirectoryPath.Combine("config").WriteAllText("https://repository.url/");
            _gitDirectoryPath.Combine("HEAD").WriteAllText("my_branch");
        }

        [TearDown]
        public new void TearDown()
        {
            _gitDirectoryPath.Delete();
        }

        [Test]
        public void GetSortedFqns_WithoutTemplates()
        {
            Test(info =>
            {
                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result, Is.Empty);
            });
        }

        [Test]
        public void GetSortedFqns_WithSpecialTemplateConfiguration()
        {
            Test(info =>
            {
                info.Settings.SetValue((CopyFqnProvidersSettings s) => s.UrlTemplates,
                    "    \n" +
                    "Name: http://url\n" +
                    "   Na   m e   :     http://url  \n" +
                    " Na me : http://url/{Te mp la te}/path?x={Te mp la te}  \n" +
                    "  http://url  \n" +
                    "");

                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result.Select(x => (x.PresentableText, x.TextToCopy)), Is.EqualTo(new[]
                {
                    ("Name", "http://url"),
                    ("Na   m e", "http://url"),
                    ("Na me", "http://url/{Te mp la te}/path?x={Te mp la te}"),
                    ("http://url", "http://url"),
                }));
            });
        }

        [Test]
        public void GetSortedFqns_WithGitRepoUrl()
        {
            Test(info =>
            {
                SetDummyGitUrlTemplate(info.Settings);

                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result.Select(x => x.TextToCopy), Is.EqualTo(new[]
                {
                    "https://repository.url/branch/my_branch/CopyFqnProviders/SomeClass.cs",
                }));
            });
        }

        [Test]
        public void GetSortedFqns_WithoutGitDirectory()
        {
            Test(info =>
            {
                SetDummyGitUrlTemplate(info.Settings);
                _gitDirectoryPath.Delete();

                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result.Select(x => x.TextToCopy), Is.EqualTo(new[]
                {
                    "<cannot find Git repository origin>/branch/<cannot find Git branch>/<cannot find Git repository>",
                }));
            });
        }

        [Test]
        public void GetSortedFqns_WithoutFoundOrigin()
        {
            Test(info =>
            {
                SetDummyGitUrlTemplate(info.Settings);
                _gitDirectoryPath.Combine("config").WriteAllText("");

                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result.Select(x => x.TextToCopy), Is.EqualTo(new[]
                {
                    "<cannot find Git repository origin>/branch/my_branch/CopyFqnProviders/SomeClass.cs",
                }));
            });
        }

        [Test]
        public void GetSortedFqns_WithEmptyHeadFile()
        {
            Test(info =>
            {
                SetDummyGitUrlTemplate(info.Settings);
                _gitDirectoryPath.Combine("HEAD").WriteAllText("");

                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result.Select(x => x.TextToCopy), Is.EqualTo(new[]
                {
                    "https://repository.url/branch/<cannot find Git branch>/CopyFqnProviders/SomeClass.cs",
                }));
            });
        }

        private static void SetDummyGitUrlTemplate(IContextBoundSettingsStore contextBoundSettingsStore)
        {
            contextBoundSettingsStore.SetValue((CopyFqnProvidersSettings s) => s.UrlTemplates,
                "{GitRepoOriginUrl}/branch/{GitRepoBranch}/{PathRelativeToGitRepoSlashSeparated}");
        }

        [Test]
        public void GetSortedFqns_WithPathRelativeToSolutionSlashSeparatedTemplate()
        {
            Test(info =>
            {
                info.Settings.SetValue((CopyFqnProvidersSettings s) => s.UrlTemplates, "http://example.com/{PathRelativeToSolutionSlashSeparated}");
                _gitDirectoryPath.Delete(); // proves that this works without a Git directory

                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result.Select(x => x.TextToCopy), Is.EqualTo(new[] { "http://example.com/test/data/CopyFqnProviders/SomeClass.cs" }));
            });
        }

        //

        [Test]
        public void IsApplicable_WithTemplates()
        {
            Test(info =>
            {
                info.Settings.SetValue((CopyFqnProvidersSettings s) => s.UrlTemplates, "some entry");

                var result = info.Sut.IsApplicable(info.DataContext);

                Assert.That(result, Is.True);
            });
        }

        [Test]
        public void IsApplicable_WithDefault()
        {
            Test(info =>
            {
                var result = info.Sut.IsApplicable(info.DataContext);

                Assert.That(result, Is.False);
            });
        }

        //

        [Test]
        public void Priority()
        {
            Test(info =>
            {
                var result = info.Sut.Priority;

                Assert.That(result, Is.EqualTo(+10));
            });
        }

        protected override CustomSourceLinkFqnProvider CreateFqnProvider(ISolution solution) =>
            new CustomSourceLinkFqnProvider(solution, _solutionPathProvider, _gitRepositoryProvider);
    }
}
