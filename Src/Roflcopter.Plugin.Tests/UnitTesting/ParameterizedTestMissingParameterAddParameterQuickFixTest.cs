using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Roflcopter.Plugin.UnitTesting;

namespace Roflcopter.Plugin.Tests.UnitTesting
{
    [TestFixture]
    [TestNetFramework4]
    [TestPackages("NUnit")]
    public class ParameterizedTestMissingParameterAddParameterQuickFixTest :
        CSharpQuickFixTestBase<ParameterizedTestMissingParameterAddParameterQuickFix>
    {
        [Test]
        public void TestCaseWithMissingStringParameter() => DoNamedTest();

        [Test]
        public void TestCaseWithMissingEnumParameter() => DoNamedTest();
    }
}
