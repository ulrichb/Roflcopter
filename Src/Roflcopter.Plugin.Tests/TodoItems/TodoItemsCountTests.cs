using System;
using System.IO;
using System.Linq;
using JetBrains.Application.Components;
using JetBrains.Application.Settings;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using Roflcopter.Plugin.TodoItems;

namespace Roflcopter.Plugin.Tests.TodoItems
{
    [TestFixture]
    [TestNet60]
    public class TodoItemsCountTests : BaseTestWithSingleProject
    {
        protected override string RelativeTestDataPath => Path.Combine(base.RelativeTestDataPath, "..");

        [Test]
        public void TodoItemsCounts()
        {
            Test((consumer, _) =>
            {
                Assert.That(consumer.TodoItemsCounts, Is.Not.Null);
                Assert.That(consumer.TodoItemsCounts.NotNull().Select(x => (x.Definition.ToString(), x.Count)), Is.EqualTo(new[]
                {
                    ("Bug", 2),
                    ("Todo", 5),
                }));
            });
        }

        [Test]
        public void TodoItemsCountsWithCondition()
        {
            Test((consumer, settings) =>
            {
                var definitionText = "Todo\n Todo  [Important] ";
                settings.SetValue((TodoItemsCountSettings s) => s.Definitions, definitionText);

                Assert.That(consumer.TodoItemsCounts.NotNull().Select(x => (x.Definition.ToString(), x.Count)), Is.EqualTo(new[]
                {
                    ("Todo", 5),
                    ("Todo[Important]", 3),
                }));
            });
        }

        [Test]
        public void TodoItemsCounts_WithNonMatchingName()
        {
            Test((consumer, settings) =>
            {
                settings.SetValue((TodoItemsCountSettings s) => s.Definitions, "Todo\nNON_MATCHING");

                Assert.That(consumer.TodoItemsCounts.NotNull().Select(x => (x.Definition.ToString(), x.Count)), Is.EqualTo(new[]
                {
                    ("Todo", 5),
                    ("NON_MATCHING", 0),
                }));
            });
        }

        [Test]
        public void TodoItemsCounts_WithDuplicateDefinition()
        {
            Test((consumer, settings) =>
            {
                settings.SetValue((TodoItemsCountSettings s) => s.Definitions, "Todo\nTodo");

                Assert.That(consumer.TodoItemsCounts.NotNull().Select(x => (x.Definition.ToString(), x.Count)), Is.EqualTo(new[]
                {
                    ("Todo", 5),
                    ("Todo", 5),
                }));
            });
        }

        [Test]
        public void TodoItemsCountsWithDisabledSetting()
        {
            Test((consumer, settings) =>
            {
                settings.SetValue((TodoItemsCountSettings s) => s.IsEnabled, false);

                Assert.That(consumer.TodoItemsCounts, Is.Null);
            });
        }

        [Test]
        public void TodoItemsCountsWithEmptyDefinitions()
        {
            Test((consumer, settings) =>
            {
                settings.SetValue((TodoItemsCountSettings s) => s.Definitions, "");

                Assert.That(consumer.TodoItemsCounts, Is.Null);
            });
        }

        [Test]
        public void TodoItemsCounts_ConsumerUpdateRequestSignal()
        {
            Test((consumer, _) =>
            {
                var oldUpdateCounter = consumer.UpdateCounter;

                RunGuarded(() => consumer.UpdateRequestSignal.Fire());

                Assert.That(consumer.UpdateCounter, Is.EqualTo(oldUpdateCounter + 1));
            });
        }

        private void Test(Action<TestTodoItemsCountConsumer, IContextBoundSettingsStore> action)
        {
            // Use an explicit short-lived lifetime for the "settings transaction" because a) it must be scoped per test,
            // and b) the `base.TestLifetime` is destroyed within the test tear-down phase which would raise
            // "target dispatcher [...] does not support asynchronous execution or cross-thread marshalling" errors
            // (b/c TearDown is not `RunWithAsyncBehaviorAllowed`).

            using var temporarySettingsLifetimeDefinition = Lifetime.Define("TemporarySettings");

            ExecuteWithinSettingsTransactionGuarded(temporarySettingsLifetimeDefinition.Lifetime, (_, settings) =>
            {
                var projectFiles = new[] { "Sample.cs", "Sample.xml" }.Select(x => GetTestDataFilePath2(x).FullPath);

                WithSingleProject(projectFiles, (_, solution, _) =>
                {
                    Assert.That(solution.GetComponent<TodoItemsCountProvider>, Is.Not.Null);
                    var consumer = ShellInstance.GetComponent<TestTodoItemsCountConsumer>();

                    action(consumer, settings);
                });
            });
        }
    }
}
