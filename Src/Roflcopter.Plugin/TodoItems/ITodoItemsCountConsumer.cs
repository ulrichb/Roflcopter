using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.UI.ActionSystem.ActionBar;

namespace Roflcopter.Plugin.TodoItems
{
    /// <summary>
    /// A consumer for <see cref="TodoItemsCountProvider"/>.
    /// 
    /// This indirection (instead of depending on the provider directly in the "consumers") allows to register
    /// the consumers as shell components (where UI parts like the <see cref="IActionBarPatcher"/> life).
    /// </summary>
    public interface ITodoItemsCountConsumer
    {
        ISimpleSignal UpdateRequestSignal { get; }
        void Update([CanBeNull] IReadOnlyList<TodoItemsCount> todoItemsCounts);
    }
}
