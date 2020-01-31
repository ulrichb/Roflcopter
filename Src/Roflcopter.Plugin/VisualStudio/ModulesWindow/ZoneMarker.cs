#if RESHARPER
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Platform.VisualStudio.SinceVs10.Shell.Zones;
using JetBrains.ReSharper.Feature.Services.DebuggerVs;

namespace Roflcopter.Plugin.VisualStudio.ModulesWindow
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<ISinceVs10EnvZone>, IRequire<IVsDebuggerZone>
    {
    }
}
#endif
