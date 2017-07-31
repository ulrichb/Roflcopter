using JetBrains.Annotations;

namespace Roflcopter.Plugin.TodoItems
{
    public class TodoItemsCountDefinition
    {
        public TodoItemsCountDefinition(string name, [CanBeNull] string condition)
        {
            Name = name;
            Condition = condition;
        }

        public string Name { get; }

        [CanBeNull]
        public string Condition { get; }

        public override string ToString() => Name;
    }
}
