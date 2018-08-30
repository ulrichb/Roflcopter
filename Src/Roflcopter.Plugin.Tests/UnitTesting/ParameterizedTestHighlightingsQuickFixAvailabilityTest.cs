using JetBrains.Annotations;
using JetBrains.Application.Settings;
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
    [UseNUnitPackage]
    public class ParameterizedTestHighlightingsQuickFixAvailabilityTest : QuickFixAvailabilityTestBase
    {
        protected override bool HighlightingPredicate(
            IHighlighting highlighting,
            [CanBeNull] IPsiSourceFile _,
            [CanBeNull] IContextBoundSettingsStore __)
        {
            return
                highlighting is ParameterizedTestMissingArgumentHighlighting ||
                highlighting is ParameterizedTestMissingParameterHighlighting ||
                highlighting is ParameterizedTestTypeMismatchHighlighting;
        }

        [Test]
        public void ParameterizedTestMissingArgumentHighlighting() => DoNamedTest();

        [Test]
        public void ParameterizedTestMissingParameterHighlighting() => DoNamedTest();

        [Test]
        public void ParameterizedTestTypeMismatchHighlighting() => DoNamedTest();
    }
}
