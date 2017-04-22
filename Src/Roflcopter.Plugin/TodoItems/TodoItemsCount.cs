using JetBrains.ReSharper.Feature.Services.TodoItems;

namespace Roflcopter.Plugin.TodoItems
{
    public class TodoItemsCount
    {
        public TodoItemsCount(TodoItemsCountDefinition definition)
        {
            Definition = definition;
        }

        public TodoItemsCountDefinition Definition { get; }

        public int Count { get; private set; }

        public void IncreaseIfMatches(TodoItemBase todoItem)
        {
            if (todoItem.Name == Definition.Name)
                Count++;
        }
    }
}