﻿using System;
using NUnit.Framework;

namespace Roflcopter.Sample.UnitTesting.ParameterizedTestHighlightingsQuickFixAvailabilityTest
{
    public class ParameterizedTestTypeMismatchFixParameterQuickFix
    {
        [TestCase("Arg")]
        public void WrongTypeString(int param)
        {
        }

        [TestCase(DateTimeKind.Utc)]
        public void WrongTypeEnum(int param)
        {
        }
    }
}
