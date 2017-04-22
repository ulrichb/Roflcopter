namespace Roflcopter.Plugin.TodoItems
{
    public class TodoItemsCountDefinition
    {
        public TodoItemsCountDefinition(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}