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
            Test(sut =>
            {
                Assert.That(sut.TodoItemsCounts, Is.Not.Null);
                Assert.That(sut.TodoItemsCounts.Select(x => new { Definition = x.Definition.ToString(), x.Count }), Is.EqualTo(new[]
                {
                    new { Definition = "Bug", Count = 2 },
                    new { Definition = "Todo", Count = 5 },
                }));
            });
        }

        [Test]
        public void TodoItemsCountDummyAction()
        {
            Test(_ =>
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
            Test(_ =>
            {
                var sut = new TodoItemsCountDummyAction();
                var dataContext = ShellInstance.GetComponent<DataContexts>().Empty;
                var actionPresentation = new ActionPresentation();

                sut.Update(dataContext, actionPresentation, null);

                Assert.That(actionPresentation.Text, Is.EqualTo(null));
            });
        }

        private void Test(Action<TodoItemsCountProvider> action)
        {
            var files = new[] { "Sample.cs", "Sample.xml" };

            WithSingleProject(
                files.Select(x => GetTestDataFilePath2(x).FullPath),
                (lifetime, solution, project) =>
                {
                    var sut = solution.GetComponent<TodoItemsCountProvider>();

                    action(sut);
                });
        }
    }
}
