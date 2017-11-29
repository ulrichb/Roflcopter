using System;
using NUnit.Framework;

namespace Roflcopter.Sample.UnitTesting.ParameterizedTestTypeMismatchFixParameterQuickFixTest
{
    public class TestCaseWithWrongTypeEnum
    {
        [TestCase(DateTimeKind.{caret}Utc)]
        public void Test(int param)
        {
        }
    }
}
