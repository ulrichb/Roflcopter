using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Roflcopter.Plugin.CopyFqnProviders;

namespace Roflcopter.Plugin.Tests.CopyFqnProviders
{
    [TestNetFramework4]
    public class ShortNameTypeMemberFqnProviderTest : FqnProviderTestBase<ShortNameTypeMemberFqnProvider>
    {
        [Test]
        public void GetSortedFqns_WithTopLevelElement()
        {
            Test(info =>
            {
                info.DataContext = Add(info.DataContext, info.SomeClassElement);

                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result.Single().PresentableText, Is.EqualTo("SomeClass"));
            });
        }

        [Test]
        public void GetSortedFqns_WithProperty()
        {
            Test(info =>
            {
                info.DataContext = Add(info.DataContext, info.SomeClassElement.Properties.Single());

                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result.Single().PresentableText, Is.EqualTo("SomeClass.Property"));
            });
        }

        [Test]
        public void GetSortedFqns_WithMethod()
        {
            Test(info =>
            {
                info.DataContext = Add(info.DataContext, info.SomeClassElement.Methods.Single());

                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result.Single().PresentableText, Is.EqualTo("SomeClass.Method"));
            });
        }

        [Test]
        public void GetSortedFqns_WithNestedClass()
        {
            Test(info =>
            {
                info.DataContext = Add(info.DataContext, info.SomeClassElement.NestedTypes.Single());

                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result.Single().PresentableText, Is.EqualTo("SomeClass.NestedClass"));
            });
        }

        [Test]
        public void GetSortedFqns_WithNestedClassProperty()
        {
            Test(info =>
            {
                info.DataContext = Add(info.DataContext, info.SomeClassElement.NestedTypes.Single().Properties.Single());

                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result.Single().PresentableText, Is.EqualTo("SomeClass.NestedClass.Property"));
            });
        }

        [Test]
        public void GetSortedFqns_WithDisabledSetting()
        {
            Test(info =>
            {
                info.Settings.SetValue((CopyFqnProvidersSettings s) => s.EnableShortNames, false);

                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result, Is.Empty);
            });
        }

        [Test]
        public void GetSortedFqns_WithDataContextWithNullDeclaredElement()
        {
            Test(info =>
            {
                var result = info.Sut.GetSortedFqns(info.DataContext);

                Assert.That(result, Is.Empty);
            });
        }

        //

        [Test]
        public void IsApplicable_WithProperty()
        {
            Test(info =>
            {
                info.DataContext = Add(info.DataContext, info.SomeClassElement.Properties.Single());

                var result = info.Sut.IsApplicable(info.DataContext);

                Assert.That(result, Is.True);
            });
        }

        [Test]
        public void IsApplicable_WithDataContextWithNullDeclaredElement()
        {
            Test(info =>
            {
                var result = info.Sut.IsApplicable(info.DataContext);

                Assert.That(result, Is.False);
            });
        }

        [Test]
        public void IsApplicable_WithDisabledSetting()
        {
            Test(info =>
            {
                info.Settings.SetValue((CopyFqnProvidersSettings s) => s.EnableShortNames, false);

                var result = info.Sut.IsApplicable(info.DataContext);

                Assert.That(result, Is.False);
            });
        }

        //

        [Test]
        public void Priority()
        {
            Test(info =>
            {
                var result = info.Sut.Priority;

                Assert.That(result, Is.EqualTo(-10));
            });
        }

        protected override ShortNameTypeMemberFqnProvider CreateFqnProvider(ISolution solution) =>
            solution.GetComponent<ShortNameTypeMemberFqnProvider>();
    }
}
