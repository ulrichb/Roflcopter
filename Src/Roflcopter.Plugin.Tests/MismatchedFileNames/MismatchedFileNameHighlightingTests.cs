using System.IO;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace Roflcopter.Plugin.Tests.MismatchedFileNames
{
    [TestFixture]
    [TestNetFramework4]
    public class MismatchedFileNameHighlightingTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => Path.Combine(base.RelativeTestDataPath, "..");

        [Test]
        public void SingleClass() => DoNamedTest();

        [Test]
        public void SingleClassWithWrongName() => DoNamedTest();

        [Test]
        public void MultipleClasses() => DoNamedTest();

        [Test]
        public void ClassAndEnum() => DoNamedTest();

        [Test]
        public void ClassAndInterfacePair() => DoNamedTest();

        [Test]
        public void ClassAndInterfacePairWithWrongName() => DoNamedTest();

        [Test]
        public void InterfaceAndClassPair() => DoNamedTest();

        [Test]
        public void InterfaceAndClassPairWithWrongName() => DoNamedTest();

        [Test]
        public void PartialClass_partial() => DoNamedTest("PartialClass.InvalidPostfix.cs");

        [Test]
        public void PartialClass_InvalidPostfix() => DoNamedTest("PartialClass.partial.cs");

        protected override void DoTestSolution([NotNull] params string[] fileSet) =>
            base.DoTestSolution(fileSet.Select(x => x.Replace('_', '.')).ToArray());
    }
}
