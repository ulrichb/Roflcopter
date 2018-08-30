using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;
using Roflcopter.Plugin.Tests;

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
