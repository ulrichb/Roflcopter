using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace Roflcopter.Plugin.MismatchedFileNames
{
    [ElementProblemAnalyzer(typeof(ICSharpFile), HighlightingTypes = new[] { typeof(MismatchedFileNameHighlighting) })]
    public class MismatchedFileNameElementProblemAnalyzer : ElementProblemAnalyzer<ICSharpFile>
    {
        protected override void Run(ICSharpFile file, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            var tolLevelTypeDeclarations = GetTolLevelTypeDeclarations(file);

            var mainTypeDeclaration = FindMainTypeDeclaration(tolLevelTypeDeclarations);

            if (mainTypeDeclaration != null)
            {
                var psiSourceFile = file.GetSourceFile().NotNull("file.GetSourceFile() != null");

                var fileName = psiSourceFile.Name;

                var allowedFileNamePostfixRegex = data.SettingsStore.GetValue((MismatchedFileNamesSettings s) => s.AllowedFileNamePostfixRegex);

                var fileNameWithReplacedPostfix = Regex.Replace(fileName, allowedFileNamePostfixRegex, Path.GetExtension(fileName));
                var expectedFileName = mainTypeDeclaration.DeclaredName + Path.GetExtension(fileName);

                if (fileNameWithReplacedPostfix != expectedFileName)
                {
                    consumer.AddHighlighting(new MismatchedFileNameHighlighting(mainTypeDeclaration, fileName, expectedFileName));
                }
            }
        }

        private static LocalList<ITypeDeclaration> GetTolLevelTypeDeclarations(IFile file)
        {
            var result = new LocalList<ITypeDeclaration>();

            var descendantsEnumerator = file.Descendants();

            while (descendantsEnumerator.MoveNext())
            {
                if (descendantsEnumerator.Current is ITypeDeclaration typeDeclaration)
                {
                    result.Add(typeDeclaration);
                    descendantsEnumerator.SkipThisNode();
                }
            }

            return result;
        }

        [CanBeNull]
        private static ITypeDeclaration FindMainTypeDeclaration(LocalList<ITypeDeclaration> typeDeclarations)
        {
            if (typeDeclarations.Count == 1)
                return typeDeclarations[0];

            if (typeDeclarations.Count == 2)
            {
                if (IsDeclarationPair(typeDeclarations[0], typeDeclarations[1]))
                    return typeDeclarations[0];

                if (IsDeclarationPair(typeDeclarations[1], typeDeclarations[0]))
                    return typeDeclarations[1];
            }

            return null;
        }

        [Pure]
        private static bool IsDeclarationPair(ITypeDeclaration mainTypeDeclaration, ITypeDeclaration otherTypeDeclaration) =>
            "I" + mainTypeDeclaration.DeclaredName == otherTypeDeclaration.DeclaredName;
    }
}
