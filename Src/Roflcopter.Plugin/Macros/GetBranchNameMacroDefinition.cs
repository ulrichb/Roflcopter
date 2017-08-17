using System.Diagnostics.CodeAnalysis;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;

namespace Roflcopter.Plugin.Macros
{
    [MacroDefinition(Name, ShortDescription = ShortDescription, LongDescription = LongDescription)]
    public class GetBranchNameMacroDefinition : SimpleMacroDefinition
    {
        private const string Name = "getBranchName";
        private const string ShortDescription = "Current branch name, stripped by {#0:an optional regex}";
        private const string LongDescription = "Returns the current Git branch name (using Git's HEAD file)";

        [ExcludeFromCodeCoverage /* just a declaration, tested manually */]
        public override ParameterInfo[] Parameters => new[] { new ParameterInfo(ParameterType.String) };
    }
}
