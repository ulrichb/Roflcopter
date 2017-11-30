using System.Collections;
using NUnit.Framework;

// ReSharper disable RedundantExplicitParamsArrayCreation

namespace Roflcopter.Sample.UnitTesting.ParameterizedTestHighlightingTests
{
    [TestFixture]
    public class TestCaseSamples
    {
        [TestCase("Arg")]
        public void TestCase(string param)
        {
        }

        [Test]
        [TestCase("Arg")]
        public void TestCaseWithTestAttribute(string param)
        {
        }

        [TestCase("Arg1A", "Arg1B")]
        [TestCase("Arg2A", "Arg2B")]
        public void TestCaseMultiple(string paramA, string paramB)
        {
        }

        [TestCaseSource(nameof(TestCaseSourceCases))]
        public void TestCaseSource(string paramA, string paramB)
        {
        }

        [Test]
        [TestCaseSource(nameof(TestCaseSourceCases))]
        public void TestCaseSourceWithTestAttribute(string paramA, string paramB)
        {
        }

        private static IEnumerable TestCaseSourceCases => new[] { new TestCaseData("Arg1A", "Arg1B"), new TestCaseData("Arg2A", "Arg2B") };

        [TestCase("Arg")]
        public void TestCaseWithWrongType(int param)
        {
        }

        [TestCase("ArgA", "ArgB", "ArgC")]
        [TestCase("ArgA", 42, "ArgC")]
        [TestCase(1, "ArgB", "ArgC")]
        public void TestCaseWithWrongTypeMultiple(string paramA, int paramB, double paramC)
        {
        }

        [TestCase("Arg")]
        public void TestCaseWithoutParameter()
        {
        }

        [TestCase("ArgA", "ArgB", "ArgC")]
        public void TestCaseWithMissingParameter(string paramA)
        {
        }

        [TestCase("Arg1A", "Arg1B")]
        [TestCase("Arg2A")]
        [TestCase]
        public void TestCaseWithMissingArgument(string paramA, string paramB)
        {
        }

        [TestCase("Arg1A")]
        [TestCase("Arg2A")]
        public void TestCaseWithMissingArgumentInEveryTestCase(string paramA, string paramB)
        {
        }

        [TestCase("Arg1A", "Arg1B")]
        [TestCase("Arg1A", "Arg1B", "Arg1C")]
        [TestCase("Arg1A", "Arg1B", "Arg1C", "Arg1D")]
        public void TestCaseWithExtraArgument(string paramA, string paramB)
        {
        }

        [Test]
        public void ParameterWithoutTestCase(string param)
        {
        }

        [Test]
        public void ParameterWithoutTestCaseMultiple(string paramA, string paramB)
        {
        }

        [TestCase(new object[] { "ArgA", "ArgB", "ArgC", "ArgD", "ArgE", "ArgF", "ArgG", "ArgH" })]
        [TestCase("ArgA", "ArgB", "ArgC", "ArgD", "ArgE", "ArgF", "ArgG", "ArgH")] // "params" array
        //
        [TestCase(new object[] { "ArgA", "ArgB", "ArgC", "ArgD", "ArgE", "ArgF", "ArgG", "ArgH", "Invalid" })]
        [TestCase("ArgA", "ArgB", "ArgC", "ArgD", "ArgE", "ArgF", "ArgG", "ArgH", "Invalid")] // "params" array
        public void TestCaseWithArrayContructor(string pA, string pB, string pC, string pD, string pE, string pF, string pG, string pH)
        {
        }

        [TestCase(arg1: "Arg1A", arg2: "Arg1B")]
        [TestCase(arg: "Arg2A")]
        public void TestCaseWithArgumentNames(string paramA, string paramB)
        {
        }
    }
}
