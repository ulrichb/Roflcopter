using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;

namespace Roflcopter.Plugin
{
    [MacroDefinition(Name, ShortDescription = ShortDescription, LongDescription = LongDescription)]
    public class GetBranchNameMacroDefinition : SimpleMacroDefinition
    {
        private const string Name = "getBranchName";
        private const string ShortDescription = "Current branch name, stripped by {#0:an optional regex}";
        private const string LongDescription = "Returns the current Git branch name (using Git's HEAD file)";

        public override ParameterInfo[] Parameters
        {
            get { return new[] {new ParameterInfo(ParameterType.String)}; }
        }
    }
}