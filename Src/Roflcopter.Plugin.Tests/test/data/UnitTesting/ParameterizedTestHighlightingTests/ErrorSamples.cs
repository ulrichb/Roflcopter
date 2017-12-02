using NUnit.Framework;

namespace Roflcopter.Sample.UnitTesting.ParameterizedTestHighlightingTests
{
    [TestFixture]
    public class ErrorSamples
    {
        [Test]
        [TestCase(InvalidArgument)]
        public void TestCaseWithInvalidArguments1(string param)
        {
        }

        [Test]
        [TestCase(,, "")]
        public void TestCaseWithInvalidArguments2(string param)
        {
        }

        //

        [Test]
        [TestCase(arguments: "Arg4A")]
        public void TestArgumentParamterWithoutArrayInitializer(string param)
        {
        }

        //

        [Test]
        [InvalidAttributeReference]
        public void TestWithInvalidAttribute1(string param)
        {
        }

        [Test]
        [-]
        public void TestWithInvalidAttribute2(string param)
        {
        }

        //

        [Test]
        [TestCase("Arg")]
        public void TestWithInvalidParameter(invalidParam)
        {
        }

        //

        [Test]
        public void TestWithInvalidParameterAttribute([InvalidAttributeReference] string param)
        {
        }

        //

        [TestCase("Arg")]
        public void WarningSample(int param)
        {
        }
    }
}
