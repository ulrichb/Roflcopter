#if RESHARPER
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Features.Inspections.Actions;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;
using JetBrains.Util.Threading.Tasks;
using Roflcopter.Plugin.TodoItems.OptionsPages;
#if RS20171
using JetBrains.UI.Actions.Views;
using JetBrains.UI.ActionSystem.ActionBar;
using JetBrains.UI.Options;

#else
using JetBrains.Application.Threading;
using JetBrains.Application.UI.ActionSystem.ActionBar;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.UI.SrcView.Actions.ActionBar;
using JetBrains.UI.SrcView.Actions.Views;

#endif

namespace Roflcopter.Plugin.TodoItems.Presentation
{
    [ShellComponent]
    [ExcludeFromCodeCoverage /* manually tested UI code */]
    public class TodoItemsCountPresenter : IActionBarPatcher, ITodoItemsCountConsumer
    {
        private readonly IShellLocks _shellLocks;

        [CanBeNull]
        private ActionLabel _label;

        [CanBeNull]
        private ActionSeparator _separator;

        public TodoItemsCountPresenter(IShellLocks shellLocks)
        {
            _shellLocks = shellLocks;

            UpdateRequestSignal = new SimpleSignal($"{nameof(TodoItemsCountPresenter)}.{nameof(UpdateRequestSignal)}");
        }

        public void Patch([NotNull] Lifetime lifetime, [NotNull] IActionBar actionBar)
        {
            if (actionBar.ActionGroup.ActionId == TodoExplorerActionBarActionGroup.ID)
            {
                _separator = actionBar.InjectSeparator(int.MaxValue);

                _label = actionBar.InjectLabel(int.MaxValue, "Updating...", lifetime);
                _label.NotNull().MouseDoubleClick += Label_MouseDoubleClick;

                lifetime.AddAction(() =>
                {
                    _label.NotNull().MouseDoubleClick -= Label_MouseDoubleClick;
                    _label = null;
                    _separator = null;
                });

                _shellLocks.Tasks.Queue(lifetime, () => UpdateRequestSignal.Fire(), TaskPriority.BelowNormal);
            }
        }

        public ISimpleSignal UpdateRequestSignal { get; }

        public void Update([CanBeNull] IReadOnlyList<TodoItemsCount> todoItemsCounts)
        {
            _shellLocks.Dispatcher.Invoke(
                nameof(TodoItemsCountPresenter) + "." + nameof(Update),
                () =>
                {
                    if (_label != null)
                    {
                        if (todoItemsCounts == null)
                        {
                            _separator.NotNull().Visibility = Visibility.Collapsed;
                            _label.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            _separator.NotNull().Visibility = Visibility.Visible;
                            _label.Visibility = Visibility.Visible;
                            _label.Content = string.Join(", ", todoItemsCounts.Select(x => $"{x.Definition}: {x.Count}"));
                        }
                    }
                },
                TaskPriority.BelowNormal);
        }

        private static void Label_MouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Shell.Instance.GetComponent<OptionsManager>().ShowOptionsModal(TodoItemsCountOptionsPage.OptionsPageId);
        }
    }
}
#endif
