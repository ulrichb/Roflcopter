using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TextControl;
using NUnit.Framework;
using Roflcopter.Plugin.MismatchedFileNames;

namespace Roflcopter.Plugin.Tests.MismatchedFileNames
{
    [TestFixture]
    [TestNetFramework4]
    public class MismatchedFileNameHighlightingQuickFixTest : CSharpQuickFixTestBase<MismatchedFileNameHighlightingQuickFixTest.TestQuickFix>
    {
        private TestQuickFix _quickFix;

        [CanBeNull]
        protected override IQuickFix CreateQuickFix(IProject project, ITextControl textControl)
        {
            _quickFix = (TestQuickFix) base.CreateQuickFix(project, textControl);
            return _quickFix;
        }

        [Test]
        public void SomeClassWithWrongName()
        {
            DoNamedTest();

            Assert.That(_quickFix.RenameFileActions, Is.EqualTo(new[] { "SomeClassWithWrongName.cs => SomeClass.cs" }));
            Assert.That(_quickFix.Errors, Is.Empty);
        }

        [Test]
        public void SomeClassWithWrongName_ButExistingFile()
        {
            DoTestFiles(nameof(SomeClassWithWrongName) + ".cs", "SomeClass.cs");

            Assert.That(_quickFix.Errors, Is.EqualTo(new[] { "Can't rename 'SomeClassWithWrongName.cs': A file 'SomeClass.cs' already exists." }));
            Assert.That(_quickFix.RenameFileActions, Is.Empty);
        }

        [Test]
        public void SingleClassWithWrongCaSiNg()
        {
            DoNamedTest();

            Assert.That(_quickFix.RenameFileActions, Is.EqualTo(new[] { "SingleClassWithWrongCaSiNg.cs => SingleClassWithWrongCasing.cs" }));
            Assert.That(_quickFix.Errors, Is.Empty);
        }

        [QuickFix]
        public class TestQuickFix : MismatchedFileNameHighlightingQuickFixBase
        {
            public TestQuickFix(MismatchedFileNameHighlighting highlighting) : base(highlighting)
            {
            }

            public IList<string> RenameFileActions { get; } = new List<string>();
            public List<string> Errors { get; } = new List<string>();

            protected override void RenameFile(ISolution solution, IProjectFile projectFile, string newFileName)
            {
                RenameFileActions.Add($"{projectFile.Name} => {newFileName}");
            }

            protected override void ShowError(string text, string caption)
            {
                Errors.Add($"{caption}: {text}");
            }
        }
    }
}
