using System;
using NUnit.Framework;

namespace Roflcopter.Sample.UnitTesting.ParameterizedTestHighlightingsQuickFixAvailabilityTest
{
    public class ParameterizedTestMissingParameterHighlighting
    {
        [TestCase("Arg", "first missing parameter", "next missing parameter")]
        public void MissingStringParameter(string param)
        {
        }

        [TestCase("Arg", DateTimeKind.Utc)]
        public void MissingEnumParameter(string param)
        {
        }
    }
}
