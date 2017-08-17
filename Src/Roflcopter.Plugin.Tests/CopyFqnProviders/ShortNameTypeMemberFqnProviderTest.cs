using System;
using System.IO;
using System.Linq;
using JetBrains.Application.Components;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NUnit.Framework;
using Roflcopter.Plugin.CopyFqnProviders;
using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Psi.DataContext;

namespace Roflcopter.Plugin.Tests.CopyFqnProviders
{
    [TestFixture]
    [TestNetFramework4]
    public class ShortNameTypeMemberFqnProviderTest : BaseTestWithSingleProject
    {
        protected override string RelativeTestDataPath => Path.Combine(base.RelativeTestDataPath, "..");

        [Test]
        public void IsApplicable_WithProperty()
        {
            Test((sut, someClass) =>
            {
                var dataContext = CreateDataContextWith(someClass.Properties.Single());

                var result = sut.IsApplicable(dataContext);

                Assert.That(result, Is.True);
            });
        }

        [Test]
        public void IsApplicable_WithDataContextWithNullDeclaredElement()
        {
            Test((sut, someClass) =>
            {
                var dataContext = CreateEmptyDataContext();

                var result = sut.IsApplicable(dataContext);

                Assert.That(result, Is.False);
            });
        }

        [Test]
        public void GetSortedFqns_WithProperty()
        {
            Test((sut, someClass) =>
            {
                var dataContext = CreateDataContextWith(someClass.Properties.Single());

                var result = sut.GetSortedFqns(dataContext);

                Assert.That(result.Single().PresentableText, Is.EqualTo("SomeClass.Property"));
            });
        }

        [Test]
        public void GetSortedFqns_WithMethod()
        {
            Test((sut, someClass) =>
            {
                var dataContext = CreateDataContextWith(someClass.Methods.Single());

                var result = sut.GetSortedFqns(dataContext);

                Assert.That(result.Single().PresentableText, Is.EqualTo("SomeClass.Method"));
            });
        }

        [Test]
        public void GetSortedFqns_WithNestedClass()
        {
            Test((sut, someClass) =>
            {
                var dataContext = CreateDataContextWith(someClass.NestedTypes.Single());

                var result = sut.GetSortedFqns(dataContext);

                Assert.That(result.Single().PresentableText, Is.EqualTo("SomeClass.NestedClass"));
            });
        }

        [Test]
        public void GetSortedFqns_WithNestedClassProperty()
        {
            Test((sut, someClass) =>
            {
                var dataContext = CreateDataContextWith(someClass.NestedTypes.Single().Properties.Single());

                var result = sut.GetSortedFqns(dataContext);

                Assert.That(result.Single().PresentableText, Is.EqualTo("SomeClass.NestedClass.Property"));
            });
        }

        [Test]
        public void GetSortedFqns_WithTopLevelElement_ReturnsNothing()
        {
            Test((sut, someClass) =>
            {
                var dataContext = CreateDataContextWith(someClass);

                var result = sut.GetSortedFqns(dataContext);

                Assert.That(result, Is.Empty);
            });
        }

        [Test]
        public void GetSortedFqns_WithDataContextWithNullDeclaredElement()
        {
            Test((sut, someClass) =>
            {
                var dataContext = CreateEmptyDataContext();

                var result = sut.GetSortedFqns(dataContext);

                Assert.That(result, Is.Empty);
            });
        }

        [Test]
        public void Priority()
        {
            Test((sut, someClass) =>
            {
                var result = sut.Priority;

                Assert.That(result, Is.EqualTo(-10));
            });
        }

        private void Test(Action<ShortNameTypeMemberFqnProvider, ITypeElement> action)
        {
            WithSingleProject(
                GetTestDataFilePath2("SomeClass.cs").FullPath,
                (lifetime, solution, project) => RunGuarded(() =>
                {
                    var primaryPsiFile = project.GetAllProjectFiles().Single().GetPrimaryPsiFile().NotNull();

                    var someClass = primaryPsiFile.Descendants<IClassDeclaration>().First();

                    var sut = solution.GetComponent<ShortNameTypeMemberFqnProvider>();

                    action(sut, someClass.DeclaredElement.NotNull());
                }));
        }

        private IDataContext CreateDataContextWith(IDeclaredElement declaredElement)
        {
            return ShellInstance.GetComponent<DataContexts>().CreateWithoutDataRules(
                TestFixtureLifetime,
                DataRules.AddRule("MyTestDataRule", PsiDataConstants.DECLARED_ELEMENTS, new[] { declaredElement }));
        }

        private IDataContext CreateEmptyDataContext() => ShellInstance.GetComponent<DataContexts>().Empty;
    }
}
