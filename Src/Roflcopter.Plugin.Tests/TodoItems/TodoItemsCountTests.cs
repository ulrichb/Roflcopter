using System;
using System.IO;
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application.Components;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Roflcopter.Plugin.TodoItems;
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings;

namespace Roflcopter.Plugin.Tests.TodoItems
{
    [TestFixture]
    [TestNetFramework4]
    public class TodoItemsCountTests : BaseTestWithSingleProject
    {
        protected override string RelativeTestDataPath => Path.Combine(base.RelativeTestDataPath, "..");

        [Test]
        public void TodoItemsCounts()
        {
            Test((sut, _) =>
            {
                Assert.That(sut.TodoItemsCounts, Is.Not.Null);
                Assert.That(sut.TodoItemsCounts.Select(x => (x.Definition.ToString(), x.Count)), Is.EqualTo(new[]
                {
                    ("Bug", 2),
                    ("Todo", 5),
                }));
            });
        }

        [Test]
        public void TodoItemsCountsWithCondition()
        {
            Test((sut, settings) =>
            {
                var definitionText = "Todo\n Todo  [Important] ";
                RunGuarded(() => settings.SetValue((TodoItemsCountSettings s) => s.Definitions, definitionText));

                Assert.That(sut.TodoItemsCounts, Is.Not.Null);
                Assert.That(sut.TodoItemsCounts.Select(x => (x.Definition.ToString(), x.Count)), Is.EqualTo(new[]
                {
                    ("Todo", 5),
                    ("Todo[Important]", 3),
                }));
            });
        }

        [Test]
        public void TodoItemsCountsWithDisabledSetting()
        {
            Test((sut, settings) =>
            {
                settings.SetValue((TodoItemsCountSettings s) => s.IsEnabled, false);

                Assert.That(sut.TodoItemsCounts, Is.Null);
            });
        }

        [Test]
        public void TodoItemsCountDummyAction()
        {
            Test((_, __) =>
            {
                var sut = new TodoItemsCountDummyAction();
                var dataContext = ShellInstance.GetComponent<DataContexts>().CreateWithDataRules(TestFixtureLifetime);
                var actionPresentation = new ActionPresentation();

                var result = sut.Update(dataContext, actionPresentation, null);

                Assert.That(result, Is.EqualTo(false));
                Assert.That(actionPresentation.Text, Is.EqualTo("Bug: 2, Todo: 5"));
            });
        }

        [Test]
        public void TodoItemsCountDummyAction_WithEmptyDataContext()
        {
            Test((_, __) =>
            {
                var sut = new TodoItemsCountDummyAction();
                var dataContext = ShellInstance.GetComponent<DataContexts>().Empty;
                var actionPresentation = new ActionPresentation();

                sut.Update(dataContext, actionPresentation, null);

                Assert.That(actionPresentation.Text, Is.EqualTo(null));
            });
        }

        private void Test(Action<TodoItemsCountProvider, IContextBoundSettingsStore> action)
        {
            var files = new[] { "Sample.cs", "Sample.xml" };

            ExecuteWithinSettingsTransaction(settings =>
            {
                WithSingleProject(
                    files.Select(x => GetTestDataFilePath2(x).FullPath),
                    (lifetime, solution, project) =>
                    {
                        var sut = solution.GetComponent<TodoItemsCountProvider>();

                        action(sut, settings);
                    });

                // Disable to solve issues with TodoItemsCountProvider-updates during termination of
                // the "settings transaction":
                settings.SetValue((TodoItemsCountSettings s) => s.IsEnabled, false);
            });
        }
    }
}
