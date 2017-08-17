using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;
using JetBrains.Util;

namespace Roflcopter.Plugin.Macros
{
    [MacroImplementation(Definition = typeof(GetBranchNameMacroDefinition))]
    public class GetBranchNameMacroImplementation : SimpleMacroImplementation
    {
        private readonly IGetBranchNameMacroPathProvider _pathProvider;
        private readonly string _gitDirectoryName;

        [CanBeNull]
        private readonly IMacroParameterValueNew _stripingRegexParameter;

        public GetBranchNameMacroImplementation(IGetBranchNameMacroPathProvider pathProvider, MacroParameterValueCollection parameters = null)
        {
            _pathProvider = pathProvider;
            _stripingRegexParameter = parameters.OptionalFirstOrDefault();

            _gitDirectoryName = pathProvider.GitDirectoryName;
        }

        [CanBeNull]
        public override HotspotItems GetLookupItems(IHotspotContext context)
        {
            return MacroUtil.SimpleEvaluateResult(Evaluate(context));
        }

        private string Evaluate(IHotspotContext context)
        {
            var sessionContextSolution = context.SessionContext.Solution;
            var solutionDirectory = _pathProvider.GetSolutionDirectory(sessionContextSolution);

            var findGitHeadFile = FindGitDirectory(solutionDirectory);

            if (findGitHeadFile == null)
                return $"<cannot find '{_gitDirectoryName}' directory>";

            var headFile = findGitHeadFile.Combine("HEAD");

            if (!headFile.ExistsFile)
                return $"<cannot find '{_gitDirectoryName}/HEAD' file>";

            var headFileContent = File.ReadAllLines(headFile.FullPath).DefaultIfEmpty("").First();

            if (string.IsNullOrWhiteSpace(headFileContent))
                return "<.git/HEAD file is empty>";

            var headFileRef = Regex.Replace(headFileContent, "^ref: refs/heads/", "");

            return ApplyStripingRegexArgument(headFileRef);
        }

        private string ApplyStripingRegexArgument(string headFileRef)
        {
            var strippingRegex = _stripingRegexParameter != null ? _stripingRegexParameter.GetValue() : "";

            if (!string.IsNullOrWhiteSpace(strippingRegex))
                return Regex.Replace(headFileRef, strippingRegex, "");

            return headFileRef;
        }

        [CanBeNull]
        private FileSystemPath FindGitDirectory(FileSystemPath dir)
        {
            do
            {
                var gitDirectory = dir.Combine(_gitDirectoryName);

                if (gitDirectory.ExistsDirectory)
                    return gitDirectory;

                dir = dir.Parent;
            } while (!dir.IsEmpty);

            return null;
        }
    }
}
