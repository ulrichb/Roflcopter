using System.IO;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace Roflcopter.Plugin.MismatchedFileNames
{
    [ElementProblemAnalyzer(
        typeof(ICSharpFile),
        HighlightingTypes = new[] { typeof(MismatchedFileNameHighlighting) })]
    public class MismatchedFileNameHighlightingElementProblemAnalyzer : ElementProblemAnalyzer<ICSharpFile>
    {
        protected override void Run(
            ICSharpFile file,
            ElementProblemAnalyzerData data,
            IHighlightingConsumer consumer)
        {
            var typeDeclarations = new LocalList<ITypeDeclaration>();

            var descendantsEnumerator = file.Descendants();

            while (descendantsEnumerator.MoveNext())
            {
                if (descendantsEnumerator.Current is ITypeDeclaration typeDeclaration)
                {
                    typeDeclarations.Add(typeDeclaration);
                    descendantsEnumerator.SkipThisNode();
                }
            }

            if (typeDeclarations.Count == 1)
            {
                var typeDeclaration = typeDeclarations[0];

                var psiSourceFile = file.GetSourceFile().NotNull("file.GetSourceFile() != null");

                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(psiSourceFile.Name);

                if (fileNameWithoutExtension != typeDeclaration.DeclaredName)
                {
                    consumer.AddHighlighting(new MismatchedFileNameHighlighting(typeDeclaration, psiSourceFile.Name));
                }
            }
        }
    }
}
