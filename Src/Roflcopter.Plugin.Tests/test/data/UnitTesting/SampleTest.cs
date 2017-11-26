using NUnit.Framework;

namespace Roflcopter.Sample.UnitTesting
{
    [TestFixture]
    public class SampleTest
    {
        public class NestedTest1 : SampleTest
        {
            [Test]
            public void Test1()
            {
                // <run here>
            }

            // <or run here>

            [Test]
            public void Test2()
            {
            }
        }

        public class NestedTest2 : SampleTest
        {
            [Test]
            public void DerivedTest1()
            {
            }

            [Test]
            public void DerivedTest2()
            {
            }
        }
    }
}
