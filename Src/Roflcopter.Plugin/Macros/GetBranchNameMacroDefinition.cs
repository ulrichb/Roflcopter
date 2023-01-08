using System.Diagnostics.CodeAnalysis;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;

namespace Roflcopter.Plugin.Macros
{
    [MacroDefinition(Name, ResourceType = typeof(Strings),
        DescriptionResourceName = nameof(Strings.GetBranchNameMacro_Description),
        LongDescriptionResourceName = nameof(Strings.GetBranchNameMacro_LongDescription))]
    public class GetBranchNameMacroDefinition : SimpleMacroDefinition
    {
        private const string Name = "getBranchName";

        [ExcludeFromCodeCoverage /* just a declaration, tested manually */]
        public override ParameterInfo[] Parameters => new[] { new ParameterInfo(ParameterType.String) };
    }
}
