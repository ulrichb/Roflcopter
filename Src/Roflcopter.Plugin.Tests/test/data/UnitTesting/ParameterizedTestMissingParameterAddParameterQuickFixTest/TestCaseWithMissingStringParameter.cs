using NUnit.Framework;

namespace Roflcopter.Sample.UnitTesting.ParameterizedTestMissingParameterAddParameterQuickFixTest
{
    public class TestCaseWithMissingStringParameter
    {
        [TestCase("ArgA", "ArgB"{caret}, "ArgC")]
        public void Test(string param)
        {
        }
    }
}
