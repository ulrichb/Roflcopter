using JetBrains.Application.BuildScript.Application.Zones;
#if RS20181
using IUIInteractiveEnvZone = JetBrains.Application.BuildScript.Application.Zones.IUIInteractiveZone;
#endif

namespace Roflcopter.Plugin.TodoItems.Presentation
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<IUIInteractiveEnvZone>
    {
    }
}
