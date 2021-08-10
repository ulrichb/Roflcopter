#if RESHARPER
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.Asp;

namespace Roflcopter.Plugin.UpdateAspDesignerFiles
{
    [ZoneMarker]
    public class ZoneMarker :
        IRequire<ICodeEditingZone>,
        IRequire<ILanguageAspZone>
    {
    }
}
#endif
