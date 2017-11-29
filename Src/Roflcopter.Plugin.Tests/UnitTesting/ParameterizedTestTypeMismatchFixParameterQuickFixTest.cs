using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Roflcopter.Plugin.UnitTesting;

namespace Roflcopter.Plugin.Tests.UnitTesting
{
    [TestFixture]
    [TestNetFramework4]
    [TestPackages("NUnit")]
    public class ParameterizedTestTypeMismatchFixParameterQuickFixTest : CSharpQuickFixTestBase<ParameterizedTestTypeMismatchFixParameterQuickFix>
    {
        [Test]
        public void TestCaseWithWrongTypeString() => DoNamedTest();

        [Test]
        public void TestCaseWithWrongTypeEnum() => DoNamedTest();
    }
}
