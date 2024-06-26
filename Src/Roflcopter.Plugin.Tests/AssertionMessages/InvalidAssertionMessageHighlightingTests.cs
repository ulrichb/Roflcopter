﻿using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using NUnit.Framework;
using Roflcopter.Plugin.AssertionMessages;

namespace Roflcopter.Plugin.Tests.AssertionMessages
{
    [TestFixture]
    public class InvalidAssertionMessageHighlightingTests : CSharpHighlightingTestBase
    {
        [Test]
        public void AssertionMessageContractAnnotationSamples() => DoNamedTest();

        [Test]
        public void AssertionMessageExtensionMethodSamples() => DoNamedTest();

        [Test]
        [HighlightOnly(typeof(InvalidAssertionMessageHighlighting))]
        public void AssertionMessageErrorSamples() => DoNamedTest();

        [Test]
        public void AssertionMessageLegacyAnnotationSamples() => DoNamedTest();
    }
}
