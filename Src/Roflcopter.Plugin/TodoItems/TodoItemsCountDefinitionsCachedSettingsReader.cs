using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Extentions;
using JetBrains.ProjectModel;

namespace Roflcopter.Plugin.TodoItems
{
    [SolutionComponent]
    public class TodoItemsCountDefinitionsCachedSettingsReader : ICachedSettingsReader<IReadOnlyCollection<TodoItemsCountDefinition>>
    {
        private readonly ISettingsStore _settingsStore;

        public TodoItemsCountDefinitionsCachedSettingsReader(ISettingsStore settingsStore)
        {
            _settingsStore = settingsStore;
        }

        public SettingsKey KeyExposed => _settingsStore.Schema.GetKey<TodoItemsCountSettings>();

        [CanBeNull]
        public IReadOnlyCollection<TodoItemsCountDefinition> ReadData(IContextBoundSettingsStore store)
        {
            var isEnabled = store.GetValue((TodoItemsCountSettings s) => s.IsEnabled);

            if (!isEnabled)
                return null;

            var definitionsText = store.GetValue((TodoItemsCountSettings s) => s.Definitions);

            var matches = Regex.Matches(definitionsText, @"^\s*(?<Name>.+?)\s*(\[(?<Condition>.*)\]\s*)?$", RegexOptions.Multiline);

            var result = from Match match in matches
                         let title = match.Groups["Name"].Value
                         let condition = match.Groups["Condition"]
                         select new TodoItemsCountDefinition(title, condition.Success ? condition.Value : null);

            return result.ToList();
        }
    }
}
