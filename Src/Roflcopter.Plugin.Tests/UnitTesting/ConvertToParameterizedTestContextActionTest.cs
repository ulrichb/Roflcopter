using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using Roflcopter.Plugin.UnitTesting;
using NUnit.Framework;

namespace Roflcopter.Plugin.Tests.UnitTesting
{
    [TestNetFramework4]
    [UseNUnitPackage]
    public class ConvertToParameterizedTestContextActionTest : CSharpContextActionExecuteTestBase<ConvertToParameterizedTestContextAction>
    {
        private static readonly string CommonTestDataPath = Path.Combine(nameof(UnitTesting), nameof(ConvertToParameterizedTestContextActionTest));

        protected override string RelativeTestDataPath => CommonTestDataPath;

        [ExcludeFromCodeCoverage]
        protected override string ExtraPath => throw new NotSupportedException();

        [Test]
        public void TestMethodWithoutParameters() => DoNamedTest();

        [TestNetFramework4]
        [UseNUnitPackage]
        public class AvailabilityTest : CSharpContextActionAvailabilityTestBase<ConvertToParameterizedTestContextAction>
        {
            protected override string RelativeTestDataPath => CommonTestDataPath;

            [ExcludeFromCodeCoverage]
            protected override string ExtraPath => throw new NotSupportedException();

            [Test]
            public void Availability() => DoNamedTest();
        }
    }
}
