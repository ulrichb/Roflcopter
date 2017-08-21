using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.DataFlow;

namespace Roflcopter.Plugin.TodoItems
{
    /// <summary>
    /// A consumer for <see cref="TodoItemsCountProvider"/>.
    /// 
    /// This indirection (instead of depending on the provider directly in the "consumers") allows to register
    /// the consumers as shell components (where UI parts like the IActionBarPatcher live).
    /// </summary>
    public interface ITodoItemsCountConsumer
    {
        ISimpleSignal UpdateRequestSignal { get; }
        void Update([CanBeNull] IReadOnlyList<TodoItemsCount> todoItemsCounts);
    }
}
