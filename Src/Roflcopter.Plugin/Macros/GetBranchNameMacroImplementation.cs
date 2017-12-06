using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;
using JetBrains.ReSharper.Psi;
using Roflcopter.Plugin.Git;

namespace Roflcopter.Plugin.Macros
{
    [MacroImplementation(Definition = typeof(GetBranchNameMacroDefinition))]
    public class GetBranchNameMacroImplementation : SimpleMacroImplementation
    {
        private readonly GitRepositoryProvider _gitRepositoryProvider;

        [CanBeNull]
        private readonly IMacroParameterValueNew _stripingRegexParameter;

        public GetBranchNameMacroImplementation(GitRepositoryProvider gitRepositoryProvider, MacroParameterValueCollection parameters = null)
        {
            _gitRepositoryProvider = gitRepositoryProvider;
            _stripingRegexParameter = parameters.OptionalFirstOrDefault();
        }

        [CanBeNull]
        public override HotspotItems GetLookupItems(IHotspotContext context)
        {
            return MacroUtil.SimpleEvaluateResult(Evaluate(context));
        }

        private string Evaluate(IHotspotContext context)
        {
            var psiSourceFile = context.SessionContext.Documents.First().GetPsiSourceFile(context.SessionContext.Solution);
            var sourceFileLocation = psiSourceFile.GetLocation();

            var gitRepositoryInfo = _gitRepositoryProvider.FetchGitRepositoryInfo(sourceFileLocation.Directory);

            if (gitRepositoryInfo == null)
                return "<cannot find Git repository directory>";

            var headFileReference = gitRepositoryInfo.ReadHeadFileReference();

            if (headFileReference == null)
                return "<cannot read reference in Git's HEAD file>";

            return ApplyStripingRegexArgument(headFileReference);
        }

        private string ApplyStripingRegexArgument(string headFileReference)
        {
            var strippingRegex = _stripingRegexParameter != null ? _stripingRegexParameter.GetValue() : "";

            if (!string.IsNullOrWhiteSpace(strippingRegex))
                return Regex.Replace(headFileReference, strippingRegex, "");

            return headFileReference;
        }
    }
}
