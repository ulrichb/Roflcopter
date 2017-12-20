using System.Collections.Generic;
using System.Linq;
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
#if !RS20172
using System;

#endif

namespace Roflcopter.Plugin.TodoItems
{
    [SolutionComponent]
    public class TodoItemsCountProvider
    {
        private static readonly ILogger Logger = JetBrains.Util.Logging.Logger.GetLogger(typeof(TodoItemsCountProvider));

        private readonly IPrimaryTodoManager _primaryTodoManager;
        private readonly ISettingsCache _settingsCache;
        private readonly TodoItemsCountDefinitionsCachedSettingsReader _todoItemsCountDefinitionsCachedSettingsReader;
        private readonly IReadOnlyList<ITodoItemsCountConsumer> _todoItemsCountConsumers;

        [NotNull]
        private volatile Lifetime _currentSettingsCacheLifeTime;

        public TodoItemsCountProvider(
            Lifetime lifetime,
            IPrimaryTodoManager primaryTodoManager,
            SolutionSettingsCache solutionSettingsCache,
            TodoItemsCountDefinitionsCachedSettingsReader todoItemsCountDefinitionsCachedSettingsReader,
            IEnumerable<ITodoItemsCountConsumer> todoItemsCountConsumers,
            ISettingsStore settingsStore)
        {
            _primaryTodoManager = primaryTodoManager;
            _settingsCache = solutionSettingsCache;
            _todoItemsCountDefinitionsCachedSettingsReader = todoItemsCountDefinitionsCachedSettingsReader;
            _todoItemsCountConsumers = todoItemsCountConsumers.ToIReadOnlyList();

            _primaryTodoManager.FilesWereUpdated.Advise(lifetime, files =>
            {
                // Check for invalid changed files, else we'll get "not valid" exceptions in the 'AllItems' access
                // later (at least as observed during unit test shut down):
                if (files.WhereNotNull().All(x => x.IsValid()))
                    UpdateTodoItemsCounts();
            });

            var settingsCacheGetDataSequentialLifeTime = new SequentialLifetimes(lifetime);
            _currentSettingsCacheLifeTime = settingsCacheGetDataSequentialLifeTime.Next();

            settingsStore.AdviseChange(lifetime, _todoItemsCountDefinitionsCachedSettingsReader.KeyExposed, () =>
            {
                // We use the following lifetime to solve the issue that this 'ISettingsStore.AdviseChange()' callback
                // arrives earlier than the one used in the cache. => The cache has still the old value when accessed
                // in 'UpdateTodoItemsCounts()'. => Terminate the cache lifetime explicitly.
                _currentSettingsCacheLifeTime = settingsCacheGetDataSequentialLifeTime.Next();

                UpdateTodoItemsCounts();
            });

            foreach (var consumer in _todoItemsCountConsumers)
                consumer.UpdateRequestSignal.Advise(lifetime, () => { UpdateTodoItemsCounts(); });

            // IDEA: Combine the three event sources and execute update in background thread?
        }

        private void UpdateTodoItemsCounts()
        {
            Logger.Verbose(nameof(UpdateTodoItemsCounts) + " ...");

            var definitions = GetTodoItemsCountDefinitions();

            IReadOnlyList<TodoItemsCount> todoItemsCounts = null;

            // IDEA: Maybe also early exit when there are no consumers/they have no visible presentation?

            if (definitions != null && definitions.Count > 0)
            {
                var localTodoItemsCounts = new LocalList<TodoItemsCount>(definitions.Select(x => new TodoItemsCount(x)));

                var allTodoItems = FetchAllTodoItems();
#if !RS20172
                var nameByIdLookup = FetchTodoMatcherNameByIdLookup();
#endif

                foreach (var todoItemChunk in allTodoItems.SelectMany(x => x))
                foreach (var todoItem in todoItemChunk.Value) // IDEA: Parallelize?
                {
#if RS20172
                    var todoItemName = todoItem.Name;
#else
                    // IDEA: Instead store the Guid-Id in the "definition" (extend the 'CachedSettingsReader', see also R#'s TodoPatternStorage)
                    var todoItemName = nameByIdLookup[todoItem.PatternId].SingleItem();
#endif
                    foreach (var todoItemsCount in localTodoItemsCounts)
                    {
                        if (todoItemName == todoItemsCount.Definition.Name)
                            todoItemsCount.IncreaseIfMatches(todoItem);
                    }
                }

                todoItemsCounts = localTodoItemsCounts.ToArray();
            }

            foreach (var consumer in _todoItemsCountConsumers)
                consumer.Update(todoItemsCounts);
        }

#if RS20172
        private List<ChunkHashMap<IPsiSourceFile, List<TodoItemBase>>> FetchAllTodoItems()
#else
        private List<ChunkHashMap<IPsiSourceFile, List<ITodoItem>>> FetchAllTodoItems()
#endif
        {
            using (_primaryTodoManager.Lock())
            using (ReadLockCookie.Create())
            {
                return _primaryTodoManager.AllItems.ToList();
            }
        }

#if !RS20172
        private ILookup<Guid, string> FetchTodoMatcherNameByIdLookup()
        {
            return _primaryTodoManager.Matchers.ToLookup(x => x.Id, x => x.Name);
        }
#endif

        [CanBeNull]
        private IReadOnlyCollection<TodoItemsCountDefinition> GetTodoItemsCountDefinitions()
        {
            return _settingsCache.GetData(_currentSettingsCacheLifeTime, _todoItemsCountDefinitionsCachedSettingsReader);
        }
    }
}
