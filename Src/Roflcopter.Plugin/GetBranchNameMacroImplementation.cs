using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;
using JetBrains.Util;

namespace Roflcopter.Plugin
{
    [MacroImplementation(Definition = typeof(GetBranchNameMacroDefinition))]
    public class GetBranchNameMacroImplementation : SimpleMacroImplementation
    {
        [CanBeNull]
        private readonly IMacroParameterValueNew _argument;

        public GetBranchNameMacroImplementation([Optional] MacroParameterValueCollection arguments)
        {
            _argument = arguments.OptionalFirstOrDefault();
        }

        [CanBeNull]
        public override HotspotItems GetLookupItems(IHotspotContext context)
        {
            return MacroUtil.SimpleEvaluateResult(Evaluate(context));
        }

        private string Evaluate(IHotspotContext context)
        {
            var solutionDirectory = context.SessionContext.Solution.SolutionFilePath.Directory;

            var findGitHeadFile = FindGitDirectory(solutionDirectory);

            if (findGitHeadFile == null)
            {
                return "<cannot find .git directory>";
            }

            var headFile = findGitHeadFile.Combine("HEAD");

            if (!headFile.ExistsFile)
            {
                return "<cannot find .git/HEAD file>";
            }

            var headFileContent = File.ReadAllLines(headFile.FullPath).DefaultIfEmpty("").First();

            if (string.IsNullOrWhiteSpace(headFileContent))
            {
                return "<.git/HEAD file is empty>";
            }

            var strippedHeadFileContent = Regex.Replace(headFileContent, @"^ref: refs/heads/", "");

            var strippingRegex = _argument != null ? _argument.GetValue() : "";

            if (!string.IsNullOrWhiteSpace(strippingRegex))
            {
                strippedHeadFileContent = Regex.Replace(strippedHeadFileContent, strippingRegex, "");
            }

            return strippedHeadFileContent;
        }

        [CanBeNull]
        private static FileSystemPath FindGitDirectory(FileSystemPath dir)
        {
            do
            {
                var gitDirectory = dir.Combine(".git");

                if (gitDirectory.ExistsDirectory)
                    return gitDirectory;

                dir = dir.Parent;
            } while (!dir.IsEmpty);

            return null;
        }
    }
}
