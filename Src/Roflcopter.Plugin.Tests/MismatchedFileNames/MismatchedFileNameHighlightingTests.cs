using System.IO;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Roflcopter.Plugin.MismatchedFileNames;

namespace Roflcopter.Plugin.Tests.MismatchedFileNames
{
    [TestFixture]
    [TestNetFramework4]
    public class MismatchedFileNameHighlightingTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath => Path.Combine(base.RelativeTestDataPath, "..");

        protected override bool HighlightingPredicate([NotNull] IHighlighting highlighting, [CanBeNull] IPsiSourceFile _) =>
            highlighting is MismatchedFileNameHighlighting;

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
    }
}
