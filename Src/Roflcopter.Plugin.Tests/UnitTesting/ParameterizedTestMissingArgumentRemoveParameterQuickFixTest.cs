using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Roflcopter.Plugin.UnitTesting;

namespace Roflcopter.Plugin.Tests.UnitTesting
{
    [TestFixture]
    [TestNetFramework4]
    [UseNUnitPackage]
    public class ParameterizedTestMissingArgumentRemoveParameterQuickFixTest :
        CSharpQuickFixTestBase<ParameterizedTestMissingArgumentRemoveParameterQuickFix>
    {
        [Test]
        public void TestCaseWithParameterWithMissingArgument() => DoNamedTest();
    }
}
