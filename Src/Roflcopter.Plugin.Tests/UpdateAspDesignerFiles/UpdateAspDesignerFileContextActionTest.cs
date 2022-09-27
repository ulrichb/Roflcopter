#if RESHARPER
using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Roflcopter.Plugin.UpdateAspDesignerFiles;

namespace Roflcopter.Plugin.Tests.UpdateAspDesignerFiles
{
    [TestNetFramework46]
    public class UpdateAspDesignerFileContextActionTest : AspContextActionExecuteTestBase<UpdateAspDesignerFileContextAction>
    {
        private readonly AspCodeBehindTestHelper _aspCodeBehindTestHelper;

        public UpdateAspDesignerFileContextActionTest() => _aspCodeBehindTestHelper = new AspCodeBehindTestHelper(this);

        [Test]
        public void MissingCodeBehindFile() => DoNamedTest();

        [Test]
        public void PartiallyMissingReference() => DoNamedTest();

        [Test]
        public void MissingDesignerFile() => DoNamedTest();

        //

        protected override string RelativeTestDataPath => this.CalculateRelativeTestDataPath();

        [ExcludeFromCodeCoverage]
        protected override string ExtraPath => throw new NotSupportedException();

        protected override string[] ModifyTestFiles(string[] testFiles) => base.ModifyTestFiles(_aspCodeBehindTestHelper.ExtendTestFiles(testFiles));
    }
}
#endif
