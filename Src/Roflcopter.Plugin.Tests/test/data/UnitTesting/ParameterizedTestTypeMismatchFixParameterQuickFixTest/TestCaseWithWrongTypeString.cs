using NUnit.Framework;

namespace Roflcopter.Sample.UnitTesting.ParameterizedTestTypeMismatchFixParameterQuickFixTest
{
    public class TestCaseWithWrongTypeString
    {
        [TestCase("Arg"{caret})]
        public void Test(int param)
        {
        }
    }
}
