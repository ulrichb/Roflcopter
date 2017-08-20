using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;

namespace Roflcopter.Plugin.Macros
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<ICodeEditingZone>
    {
    }
}
