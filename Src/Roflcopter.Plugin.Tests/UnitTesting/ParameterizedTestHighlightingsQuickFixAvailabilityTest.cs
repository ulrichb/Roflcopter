using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Roflcopter.Plugin.UnitTesting;

namespace Roflcopter.Plugin.Tests.UnitTesting
{
    [TestFixture]
    [TestNetFramework4]
    [TestPackages("NUnit")]
    public class ParameterizedTestHighlightingsQuickFixAvailabilityTest : QuickFixAvailabilityTestBase
    {
        protected override bool HighlightingPredicate(IHighlighting highlighting, [CanBeNull] IPsiSourceFile _)
        {
            return
                highlighting is ParameterizedTestMissingArgumentHighlighting ||
                highlighting is ParameterizedTestMissingParameterHighlighting ||
                highlighting is ParameterizedTestTypeMismatchHighlighting;
        }

        [Test]
        public void ParameterizedTestMissingArgumentRemoveParameterQuickFix() => DoNamedTest();

        [Test]
        public void ParameterizedTestMissingParameterAddParameterQuickFix() => DoNamedTest();

        [Test]
        public void ParameterizedTestTypeMismatchFixParameterQuickFix() => DoNamedTest();
    }
}
