using System.IO;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Roflcopter.Plugin.UnitTesting;

namespace Roflcopter.Plugin.Tests.UnitTesting
{
    [TestFixture]
    [TestNetFramework4]
    public abstract class ParameterizedTestHighlightingTests : CSharpHighlightingTestBase
    {
        protected override string RelativeTestDataPath =>
            Path.Combine(base.RelativeTestDataPath, "..", nameof(ParameterizedTestHighlightingTests));

        [Test]
        public void TestCaseSamples() => DoNamedTest();

        [Test]
        public void TestValueAttributeSamples() => DoNamedTest();

        [Test]
        public void DerivedAttributesSamples() => DoNamedTest();

        [Test]
        public void InheritanceSamples() => DoNamedTest();

        [Test]
        [HighlightOnly(typeof(ParameterizedTestMissingArgumentHighlighting), typeof(ParameterizedTestTypeMismatchHighlighting))]
        public void ErrorSamples() => DoNamedTest();

        [UseNUnitPackage]
        public class Default : ParameterizedTestHighlightingTests
        {
            [Test]
            public void TestBuilderInterfaceSample() => DoNamedTest();

            [Test]
            public void ParameterDataSourceAttributeSamples() => DoNamedTest();
        }

        [UseNUnitPackage("2.6.4")]
        public class WithNUnit2 : ParameterizedTestHighlightingTests
        {
        }
    }
}
