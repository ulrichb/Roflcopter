using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

#if !RS20171
using Roflcopter.Plugin.AssertionMessages;
#endif

namespace Roflcopter.Plugin.Tests.AssertionMessages
{
    [TestFixture]
    [TestNetFramework4]
    public class InvalidAssertionMessageHighlightingTests : CSharpHighlightingTestBase
    {
        [Test]
        public void AssertionMessageContractAnnotationSamples() => DoNamedTest();

        [Test]
        public void AssertionMessageExtensionMethodSamples() => DoNamedTest();

#if !RS20171
        [Test]
        [HighlightOnly(typeof(InvalidAssertionMessageHighlighting))]
        public void AssertionMessageErrorSamples() => DoNamedTest();
#endif

        [Test]
        public void AssertionMessageLegacyAnnotationSamples() => DoNamedTest();
    }
}
