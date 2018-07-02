#if RESHARPER
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Resources.Shell;
#if RS20181
using ISinceVs10EnvZone = JetBrains.Platform.VisualStudio.SinceVs10.Shell.Zones.ISinceVs10Zone;
#else
using JetBrains.Platform.VisualStudio.SinceVs10.Shell.Zones;
#endif

namespace Roflcopter.Plugin.VisualStudio.ModulesWindow
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<ISinceVs10EnvZone>, IRequire<PsiFeaturesImplZone>
    {
    }
}
#endif
