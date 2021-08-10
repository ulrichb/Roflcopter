using NUnit.Framework;

namespace Roflcopter.Sample.UnitTesting.ConvertToParameterizedTestContextActionTest
{
    public class Availability
    {
        {off}

        public void NoTestMethod()
        {
            {off}
        }

        {on}[Test]
        public void TestMethod()
        {
            {on}
        }{on}
    
        [Test]
        public void TestMethodWithParameter(string param)
        {
            {off}
        }
    }
}
