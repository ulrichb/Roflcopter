using System;
using System.Collections;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Roflcopter.Sample.UnitTesting.ParameterizedTestHighlightingTests
{
    [TestFixture]
    public class TestValueAttributeSamples
    {
        [Test]
        public void TestDataValues([Values("Arg1", "Arg2")] string param)
        {
        }

        [Test]
        public void TestDataValueSource([ValueSource(nameof(ValueSource))] string param)
        {
        }

        private static IEnumerable ValueSource => new[] { "Arg1", "Arg2" };

        [Test]
        public void TestDataValuesWithAdditionalAttribute([Values("Arg")] [In] string param)
        {
        }

        [Test]
        public void TestDataValuesMultiple([Values("Arg1", "Arg2")] string paramA, [Values("Arg1", "Arg2", "Arg3")] string paramB)
        {
        }

        [Test]
        public void TestDataValuesWithWrongType([Values("Arg", 42, 42.0)] int param)
        {
        }

        //

        [Test]
        public void TestDataValuesWithObjectArrayParameter([Values(new object[] { "a", 2, 3.0 }, new[] { "a", "b", "c" })] object[] param)
        {
        }

        [Test]
        public void TestDataValuesWithStringArrayParameter([Values(new object[] { "a", 2, 3.0 }, new[] { "a", "b", "c" })] string[] param)
        {
        }

        [Test]
        public void TestDataValuesWithTypeArrayParameter([Values(new[] { "a", "b", "c" }, new[] { typeof(int) })] Type[] param)
        {
        }

        //

        [Test]
        public void TestDataValuesWithEnumParameter([Values("str", DateTimeKind.Utc)] DateTimeKind param)
        {
        }
    }
}
