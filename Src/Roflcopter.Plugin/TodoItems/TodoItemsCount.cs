using System;
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

        public void IncreaseIfMatches(ITodoItem todoItem)
        {
            if (todoItem.Name == Definition.Name)
            {
                if (Definition.Condition == null || IsConditionMatching(todoItem, Definition.Condition))
                {
                    Count++;
                }
            }
        }

        private bool IsConditionMatching(ITodoItem todoItem, string condition)
        {
            return todoItem.Text.IndexOf(condition, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
    }
}
