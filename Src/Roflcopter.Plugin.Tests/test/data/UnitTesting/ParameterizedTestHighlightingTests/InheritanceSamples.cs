using NUnit.Framework;

// ReSharper disable NUnit.RedundantArgumentInTestCaseAttribute (added in R# 2018.3)

namespace Roflcopter.Sample.UnitTesting.ParameterizedTestHighlightingTests
{
    public static class InheritanceSamples
    {
        public abstract class Base
        {
            [Test]
            // ReSharper disable once NUnit.MethodWithParametersAndTestAttribute (it's just wrong here)
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
