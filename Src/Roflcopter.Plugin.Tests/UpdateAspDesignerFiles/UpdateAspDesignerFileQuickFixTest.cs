#if RESHARPER
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.Paths;
using JetBrains.ReSharper.TestFramework.Web;
using JetBrains.Util;
using NUnit.Framework;
using Roflcopter.Plugin.UpdateAspDesignerFiles;

namespace Roflcopter.Plugin.Tests.UpdateAspDesignerFiles
{
    [TestFixture]
    [TestAsp40]
    public class UpdateAspDesignerFileQuickFixTest : QuickFixTestBase<UpdateAspDesignerFileQuickFix>
    {
        private readonly AspCodeBehindTestHelper _aspCodeBehindTestHelper;

        public UpdateAspDesignerFileQuickFixTest() => _aspCodeBehindTestHelper = new AspCodeBehindTestHelper(this);

        [Test]
        public void MissingReference() => DoNamedTest();

        //

        protected override string[] ModifyTestFiles(string[] testFiles) => base.ModifyTestFiles(_aspCodeBehindTestHelper.ExtendTestFiles(testFiles));

        protected override void DoTest(Lifetime lifetime, IProject testProject)
        {
            base.DoTest(lifetime, testProject);

            var designerFile = testProject.GetPsiSourceFileInProject(_aspCodeBehindTestHelper.DesignerFileName.ToFileSystemPath());

            ExecuteWithGold(_aspCodeBehindTestHelper.DesignerFileName, writer => { writer.Write(designerFile.Document.GetText()); });
        }
    }
}
#endif
