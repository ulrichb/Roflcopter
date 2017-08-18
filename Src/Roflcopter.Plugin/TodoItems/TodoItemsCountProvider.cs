using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Extentions;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Settings.Cache;
using JetBrains.ReSharper.Feature.Services.TodoItems;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;
using JetBrains.Util.DataStructures;

namespace Roflcopter.Plugin.TodoItems
{
    [SolutionComponent]
    public class TodoItemsCountProvider : ICachedSettingsReader<IReadOnlyCollection<TodoItemsCountDefinition>>
    {
        private static readonly ILogger Logger = JetBrains.Util.Logging.Logger.GetLogger(typeof(TodoItemsCountProvider));

        [CanBeNull]
        private volatile IReadOnlyList<TodoItemsCount> _todoItemsCounts;

        private readonly Lifetime _lifetime;
        private readonly MultiplexingTodoManager _multiplexingTodoManager;
        private readonly ISettingsStore _settingsStore;
        private readonly ISettingsCache _settingsCache;

        public TodoItemsCountProvider(
            Lifetime lifetime,
            MultiplexingTodoManager multiplexingTodoManager,
            SolutionSettingsCache solutionSettingsCache,
            ISettingsStore settingsStore,
            IShellLocks shellLocks)
        {
            _lifetime = lifetime;
            _multiplexingTodoManager = multiplexingTodoManager;
            _settingsCache = solutionSettingsCache;
            _settingsStore = settingsStore;

            _multiplexingTodoManager.FilesWereUpdated.Advise(_lifetime, files =>
            {
                // Check for invalid changed files, else we'll get "not valid" exceptions in the 'AllItems' access
                // later (at least as observed during unit test shut down):
                if (files.WhereNotNull().All(x => x.IsValid()))
                    UpdatingTodoItemsCounts();
            });

            _settingsStore.AdviseChange(_lifetime, KeyExposed, () =>
            {
                // Using the Dispatcher here to solve the issue, that the ISettingsStore.AdviseChange() comes too early (our
                // settings cache has still the old value):
                shellLocks.Dispatcher.BeginInvoke(nameof(TodoItemsCountProvider), () =>
                    shellLocks.ReentrancyGuard.Execute(nameof(TodoItemsCountProvider), () => UpdatingTodoItemsCounts()));
            });
        }

        [CanBeNull]
        public IReadOnlyCollection<TodoItemsCount> TodoItemsCounts => _todoItemsCounts;

        private void UpdatingTodoItemsCounts()
        {
            Logger.Verbose(nameof(UpdatingTodoItemsCounts) + " ...");

            var todoItemsCountDefinitions = GetTodoItemsCountDefinitions();

            IReadOnlyList<TodoItemsCount> newTodoItemsCounts = null;

            if (todoItemsCountDefinitions != null)
            {
                var localTodoItemsCounts = new LocalList<TodoItemsCount>(todoItemsCountDefinitions.Select(x => new TodoItemsCount(x)));

                List<ChunkHashMap<IPsiSourceFile, List<TodoItemBase>>> allItems;

                using (_multiplexingTodoManager.Lock())
                using (ReadLockCookie.Create())
                {
                    allItems = _multiplexingTodoManager.AllItems.ToList();
                }

                foreach (var todoItem in allItems.SelectMany(x => x).SelectMany(x => x.Value))
                {
                    foreach (var newTodoItemsCount in localTodoItemsCounts)
                    {
                        newTodoItemsCount.IncreaseIfMatches(todoItem);
                    }
                }

                newTodoItemsCounts = localTodoItemsCounts.ToArray();
            }

            _todoItemsCounts = newTodoItemsCounts;
        }

        [CanBeNull]
        private IReadOnlyCollection<TodoItemsCountDefinition> GetTodoItemsCountDefinitions()
        {
            return _settingsCache.GetData(_lifetime, this);
        }

        public SettingsKey KeyExposed => _settingsStore.Schema.GetKey<TodoItemsCountSettings>();

        [CanBeNull]
        IReadOnlyCollection<TodoItemsCountDefinition> ICachedSettingsReader<IReadOnlyCollection<TodoItemsCountDefinition>>.
            ReadData(IContextBoundSettingsStore store)
        {
            var isEnabled = store.GetValue((TodoItemsCountSettings s) => s.IsEnabled);

            // IDEA: Maybe also return null when the display (explorer) isn't visible?

            if (!isEnabled)
                return null;

            var definitionsText = store.GetValue((TodoItemsCountSettings s) => s.Definitions);

            var matches = Regex.Matches(definitionsText, @"^\s*(?<Title>.+?)\s*(\[(?<Condition>.*)\]\s*)?$", RegexOptions.Multiline);

            var result = from Match match in matches
                         let title = match.Groups["Title"].Value
                         let condition = match.Groups["Condition"]
                         select new TodoItemsCountDefinition(title, condition.Success ? condition.Value : null);

            return result.ToList();
        }
    }
}
