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
    public interface IRoflcopterTestEnvironmentZone : ITestsZone, IRequire<PsiFeatureTestZone>
    {
    }

    [ZoneMarker]
    public class ZoneMarker : IRequire<IRoflcopterTestEnvironmentZone>
    {
    }
}

[SetUpFixture]
public class TestEnvironmentSetUpFixture : ExtensionTestEnvironmentAssembly<IRoflcopterTestEnvironmentZone>
{
}
