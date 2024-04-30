using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using NUnit.Framework;

namespace Roflcopter.Plugin.Tests.MismatchedFileNames
{
    [TestFixture]
    public class MismatchedFileNameHighlightingTests : CSharpHighlightingTestBase
    {
        [Test]
        public void SingleClass() => DoNamedTest();

        [Test]
        public void SingleClassWithWrongName() => DoNamedTest();

        [Test]
        public void SingleClassWithWrongCaSiNg() => DoNamedTest();

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
        public void PartialClass() => DoNamedTest(nameof(PartialClass_partial) + ".cs", nameof(PartialClass_InvalidPostfix) + ".cs");

        [Test]
        public void PartialClass_partial() => DoNamedTest(nameof(PartialClass) + ".cs", nameof(PartialClass_InvalidPostfix) + ".cs");

        [Test]
        public void PartialClass_InvalidPostfix() => DoNamedTest(nameof(PartialClass) + ".cs", nameof(PartialClass_partial) + ".cs");

        protected override void DoTestSolution([NotNull] params string[] fileSet) =>
            base.DoTestSolution(fileSet.Select(x => x.Replace('_', '.')).ToArray());
    }
}
