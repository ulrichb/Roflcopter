using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Roflcopter.Plugin.UnitTesting;

namespace Roflcopter.Plugin.Tests.UnitTesting
{
    [TestNetFramework4]
    [UseNUnitPackage]
    public class ConvertToParameterizedTestContextActionTest : CSharpContextActionExecuteTestBase<ConvertToParameterizedTestContextAction>
    {
        protected override string RelativeTestDataPath => this.CalculateRelativeTestDataPath();

        [ExcludeFromCodeCoverage]
        protected override string ExtraPath => throw new NotSupportedException();

        [Test]
        public void TestMethodWithoutParameters() => DoNamedTest();

        [TestNetFramework4]
        [UseNUnitPackage]
        public class AvailabilityTest : CSharpContextActionAvailabilityTestBase<ConvertToParameterizedTestContextAction>
        {
            protected override string RelativeTestDataPath => this.CalculateRelativeTestDataPath();

            [ExcludeFromCodeCoverage]
            protected override string ExtraPath => throw new NotSupportedException();

            [Test]
            public void Availability() => DoNamedTest();
        }
    }
}
