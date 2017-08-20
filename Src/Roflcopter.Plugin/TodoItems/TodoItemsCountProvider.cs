using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
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

        private readonly MultiplexingTodoManager _multiplexingTodoManager;
        private readonly ISettingsStore _settingsStore;
        private readonly ISettingsCache _settingsCache;

        [NotNull]
        private volatile Lifetime _currentSettingsCacheLifeTime;

        public TodoItemsCountProvider(
            Lifetime lifetime,
            MultiplexingTodoManager multiplexingTodoManager,
            SolutionSettingsCache solutionSettingsCache,
            ISettingsStore settingsStore)
        {
            _multiplexingTodoManager = multiplexingTodoManager;
            _settingsCache = solutionSettingsCache;
            _settingsStore = settingsStore;

            _multiplexingTodoManager.FilesWereUpdated.Advise(lifetime, files =>
            {
                // Check for invalid changed files, else we'll get "not valid" exceptions in the 'AllItems' access
                // later (at least as observed during unit test shut down):
                if (files.WhereNotNull().All(x => x.IsValid()))
                    UpdateTodoItemsCounts();
            });

            var settingsCacheGetDataSequentialLifeTime = new SequentialLifetimes(lifetime);
            _currentSettingsCacheLifeTime = settingsCacheGetDataSequentialLifeTime.Next();

            _settingsStore.AdviseChange(lifetime, KeyExposed, () =>
            {
                // We use the following lifetime to solve the issue that this 'ISettingsStore.AdviseChange()' callback
                // arrives earlier than the one used in the cache. => The cache has still the old value when accessed
                // in 'UpdateTodoItemsCounts()'. => Terminate the cache lifetime explicitly.
                _currentSettingsCacheLifeTime = settingsCacheGetDataSequentialLifeTime.Next();

                UpdateTodoItemsCounts();
            });
        }

        [CanBeNull]
        public IReadOnlyCollection<TodoItemsCount> TodoItemsCounts => _todoItemsCounts;

        private void UpdateTodoItemsCounts()
        {
            Logger.Verbose(nameof(UpdateTodoItemsCounts) + " ...");

            var todoItemsCountDefinitions = GetTodoItemsCountDefinitions();

            IReadOnlyList<TodoItemsCount> newTodoItemsCounts = null;

            if (todoItemsCountDefinitions != null && todoItemsCountDefinitions.Count > 0)
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
            return _settingsCache.GetData(_currentSettingsCacheLifeTime, this);
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
