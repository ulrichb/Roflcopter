using System.Collections;
using NUnit.Framework;

namespace Roflcopter.Sample.UnitTesting.ParameterizedTestHighlightingTests
{
    [TestFixture]
    public class DerivedAttributesSamples
    {
        [DerivedTest]
        public void DerivedTest()
        {
        }

        [DerivedTest]
        public void DerivedTestWithWarning(string param)
        {
        }

        [DerivedTestCase("Arg")]
        public void TestCase(string param)
        {
        }

        [DerivedTestCase("Arg")]
        public void TestCaseWithWarning(int param)
        {
        }

        [DerivedTestCase("ArgA", "ArgB", "ArgC", "Invalid")]
        public void TestCaseWithArrayContructor(string paramA, string paramB, string paramC)
        {
        }

        [Test]
        [DerivedTestCaseSource(nameof(TestCaseSourceCases))]
        // ReSharper disable once NUnit.MethodWithParametersAndTestAttribute (it's just wrong here)
        public void TestCaseSource(string paramA, string paramB)
        {
        }

        private static IEnumerable TestCaseSourceCases => new[] { new TestCaseData("Arg1A", "Arg1B"), new TestCaseData("Arg2A", "Arg2B") };

        [Test]
        public void TestDataValues([DerivedValues("Arg")] string paramA, [DerivedValues("Arg")] int paramB)
        {
        }

        //

        public class DerivedTestAttribute : TestAttribute
        {
        }

        private class DerivedTestCaseAttribute : TestCaseAttribute
        {
            public DerivedTestCaseAttribute(object arg) : base(arg)
            {
            }

            public DerivedTestCaseAttribute(params object[] args) : base(args)
            {
            }
        }

        private class DerivedTestCaseSourceAttribute : TestCaseSourceAttribute
        {
            public DerivedTestCaseSourceAttribute(string sourceName) : base(sourceName)
            {
            }
        }

        private class DerivedValuesAttribute : ValuesAttribute
        {
            public DerivedValuesAttribute(object arg1) : base(arg1)
            {
            }
        }
    }
}
