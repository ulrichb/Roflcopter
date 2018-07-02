using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using NUnit.Framework;
using Roflcopter.Plugin.Tests;
#if RS20181
using ITestsEnvZone = JetBrains.TestFramework.Application.Zones.ITestsZone;
#else
using JetBrains.TestFramework.Application.Zones;
#endif

[assembly: RequiresSTA]

namespace Roflcopter.Plugin.Tests
{
    [ZoneDefinition]
    public interface IRoflcopterTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>
    {
    }

    [ZoneMarker]
    public class ZoneMarker : IRequire<IRoflcopterTestEnvironmentZone>
    {
    }
}

// Note: Global namespace to workaround (or hide) https://youtrack.jetbrains.com/issue/RSRP-464493.
[SetUpFixture]
public class TestEnvironmentSetUpFixture : ExtensionTestEnvironmentAssembly<IRoflcopterTestEnvironmentZone>
{
}
