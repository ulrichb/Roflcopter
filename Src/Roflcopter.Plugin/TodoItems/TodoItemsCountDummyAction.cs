using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Features.Inspections.Actions;
using JetBrains.UI.ActionsRevised;

namespace Roflcopter.Plugin.TodoItems
{
    [ActionGroup(nameof(TodoItemsCountDummyActionGroup), ActionGroupInsertStyles.Separated, Id = 944208910)]
    public class TodoItemsCountDummyActionGroup : IAction, IInsertLast<TodoExplorerActionBarActionGroup>
    {
        public TodoItemsCountDummyActionGroup(TodoItemsCountDummyAction _)
        {
        }
    }

    [Action(nameof(TodoItemsCountDummyAction), Id = 944208920)]
    public class TodoItemsCountDummyAction : IExecutableAction
    {
        public bool Update(IDataContext context, ActionPresentation presentation, [CanBeNull] DelegateUpdate nextUpdate)
        {
            var todoItemsCountProvider = context.GetComponent<TodoItemsCountProvider>();

            var todoItemsCounts = todoItemsCountProvider.TodoItemsCounts;

            if (todoItemsCounts == null)
                presentation.Text = null;
            else
                presentation.Text = string.Join(", ", todoItemsCounts.Select(x => $"{x.Definition}: {x.Count}"));

            return false;
        }

        public void Execute(IDataContext context, [CanBeNull] DelegateExecute nextExecute)
        {
        }
    }
}
