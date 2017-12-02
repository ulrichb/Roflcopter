using NUnit.Framework;

namespace Roflcopter.Sample.UnitTesting.ParameterizedTestHighlightingsQuickFixAvailabilityTest
{
    public class ParameterizedTestMissingArgumentRemoveParameterQuickFix
    {
        [TestCase("ArgA")]
        public void Test(string paramA, string paramB, string paramC)
        {
        }
    }
}
