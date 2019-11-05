﻿using JetBrains.ReSharper.TestFramework;

namespace Roflcopter.Plugin.Tests.UnitTesting
{
    public class UseNUnitPackageAttribute : TestPackagesAttribute
    {
        public UseNUnitPackageAttribute() : base("NUnit")
        {
        }
    }
}
