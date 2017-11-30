using System;
using NUnit.Framework;

namespace Roflcopter.Sample.UnitTesting.ParameterizedTestMissingParameterAddParameterQuickFixTest
{
    public class TestCaseWithMissingEnumParameter
    {
        [TestCase("Arg", DateTimeKind.{caret}Utc)]
        public void Test(string param)
        {
        }
    }
}
