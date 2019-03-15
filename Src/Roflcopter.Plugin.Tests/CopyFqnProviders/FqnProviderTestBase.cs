using System;
using System.IO;
using System.Linq;
using JetBrains.Application.Components;
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Features.Environment.CopyFqn;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.DataContext;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TestFramework;
#if RS20183
using JetBrains.Util;
#else
using JetBrains.Diagnostics;

#endif

namespace Roflcopter.Plugin.Tests.CopyFqnProviders
{
    public abstract class FqnProviderTestBase<TFqnProvider> : BaseTestWithSingleProject where TFqnProvider : class, IFqnProvider
    {
        protected override string RelativeTestDataPath => Path.Combine(base.RelativeTestDataPath, "..");

        protected void Test(
            Action<(TFqnProvider Sut, IContextBoundSettingsStore Settings, IDataContext DataContext, ITypeElement SomeClassElement)> action)
        {
            ExecuteWithinSettingsTransaction(settings =>
            {
                WithSingleProject(
                    GetTestDataFilePath2("SomeClass.cs").FullPath,
                    (lifetime, solution, project) => RunGuarded(() =>
                    {
                        var projectFile = project.GetAllProjectFiles().Single();
                        var primaryPsiFile = projectFile.GetPrimaryPsiFile().NotNull();

                        var sut = CreateFqnProvider(solution);
                        var dataContext = Add(CreateEmptyDataContext(), projectFile);
                        var someClass = primaryPsiFile.Descendants<IClassDeclaration>().First();

                        action((sut, settings, dataContext, someClass.DeclaredElement.NotNull()));
                    }));
            });
        }

        protected abstract TFqnProvider CreateFqnProvider(ISolution solution);

        private IDataContext CreateEmptyDataContext() => ShellInstance.GetComponent<DataContexts>().Empty;

        private IDataContext Add(IDataContext dataContext, IProjectFile projectFile)
        {
            return ShellInstance.GetComponent<DataContexts>().CloneWithAdditionalDataRules(
                TestFixtureLifetime,
                dataContext,
                DataRules.AddRule("<test data rule>", ProjectModelDataConstants.PROJECT_MODEL_ELEMENT, projectFile));
        }

        protected IDataContext Add(IDataContext dataContext, IDeclaredElement declaredElement)
        {
            return ShellInstance.GetComponent<DataContexts>().CloneWithAdditionalDataRules(
                TestFixtureLifetime,
                dataContext,
                DataRules.AddRule("<test data rule>", PsiDataConstants.DECLARED_ELEMENTS, new[] { declaredElement }));
        }
    }
}
