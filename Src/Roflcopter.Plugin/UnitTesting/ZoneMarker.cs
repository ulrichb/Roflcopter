using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.UnitTestFramework;

namespace Roflcopter.Plugin.UnitTesting
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<IUnitTestingZone>
    {
    }
}
