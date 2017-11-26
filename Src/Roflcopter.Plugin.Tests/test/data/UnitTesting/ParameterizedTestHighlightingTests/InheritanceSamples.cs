using NUnit.Framework;

namespace Roflcopter.Sample.UnitTesting.ParameterizedTestHighlightingTests
{
    public static class InheritanceSamples
    {
        public abstract class Base
        {
            [Test]
            public abstract void TestCaseInOverride(string paramA, string paramB);
        }

        [TestFixture]
        public class Derived : Base
        {
            [TestCase("ArgA", "ArgB", "Invalid")]
            public override void TestCaseInOverride(string paramA, string paramB)
            {
            }
        }
    }
}
