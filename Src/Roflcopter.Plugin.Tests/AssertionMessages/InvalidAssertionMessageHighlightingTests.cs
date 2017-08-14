using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Roflcopter.Plugin.AssertionMessages;

namespace Roflcopter.Plugin.Tests.AssertionMessages
{
    [TestFixture]
    [TestNetFramework4]
    public class InvalidAssertionMessageHighlightingTests : CSharpHighlightingTestBase
    {
        protected override bool HighlightingPredicate([NotNull] IHighlighting highlighting, [CanBeNull] IPsiSourceFile _)
        {
            return highlighting is InvalidAssertionMessageHighlighting ||
                   highlighting is ConditionIsAlwaysTrueOrFalseWarning ||
                   highlighting is HeuristicUnreachableCodeWarning;
        }

        [Test]
        public void AssertionMessageContractAnnotationSamples() => DoNamedTest();

        [Test]
        public void AssertionMessageExtensionMethodSamples() => DoNamedTest();

        [Test]
        public void AssertionMessageErrorSamples() => DoNamedTest();

        [Test]
        public void AssertionMessageLegacyAnnotationSamples() => DoNamedTest();
    }
}
