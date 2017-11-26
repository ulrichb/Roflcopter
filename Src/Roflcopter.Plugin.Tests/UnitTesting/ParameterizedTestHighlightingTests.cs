using System.IO;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
#if !RS20171
using Roflcopter.Plugin.UnitTesting;

#endif

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

#if !RS20171
        [Test]
        [HighlightOnly(typeof(ParameterizedTestMissingArgumentHighlighting), typeof(ParameterizedTestTypeMismatchHighlighting))]
        public void ErrorSamples() => DoNamedTest();
#endif

        [TestPackages("NUnit")]
        public class Default : ParameterizedTestHighlightingTests
        {
            [Test]
            public void TestBuilderInterfaceSample() => DoNamedTest();

            [Test]
            public void ParameterDataSourceAttributeSamples() => DoNamedTest();
        }

        [TestPackages("NUnit/2.6.4")]
        public class WithNUnit2 : ParameterizedTestHighlightingTests
        {
        }
    }
}
