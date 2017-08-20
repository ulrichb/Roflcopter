using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services.Navigation;

namespace Roflcopter.Plugin.CopyFqnProviders
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<NavigationZone>
    {
    }
}
