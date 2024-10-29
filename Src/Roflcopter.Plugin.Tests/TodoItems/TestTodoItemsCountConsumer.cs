using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Parts;
using JetBrains.DataFlow;
using Roflcopter.Plugin.TodoItems;

namespace Roflcopter.Plugin.Tests.TodoItems
{
    [ShellComponent(Instantiation.DemandAnyThreadSafe)]
    internal class TestTodoItemsCountConsumer : ITodoItemsCountConsumer
    {
        [CanBeNull]
        public IReadOnlyList<TodoItemsCount> TodoItemsCounts;

        public TestTodoItemsCountConsumer()
        {
            UpdateRequestSignal = new SimpleSignal($"{nameof(TestTodoItemsCountConsumer)}.{nameof(UpdateRequestSignal)}");
        }

        public ISimpleSignal UpdateRequestSignal { get; }

        public int UpdateCounter;

        public void Update(IReadOnlyList<TodoItemsCount> todoItemsCounts)
        {
            TodoItemsCounts = todoItemsCounts;
            UpdateCounter++;
        }
    }
}
