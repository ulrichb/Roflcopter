using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Features.Inspections.TodoItems;
using JetBrains.ReSharper.Psi.Resources;
using JetBrains.UI.Options;
using JetBrains.UI.Options.OptionsDialog2.SimpleOptions;

namespace Roflcopter.Plugin.TodoItems
{
    [OptionsPage(
        "TodoItemsCountOptionsPage", "To-do Items Count", typeof(PsiSymbolsThemedIcons.Macro),
        ParentId = TodoExplorerOptionsPage.PID)]
    public class TodoItemsCountOptionsPage : SimpleOptionsPage
    {
        public TodoItemsCountOptionsPage([NotNull] Lifetime lifetime, [NotNull] OptionsSettingsSmartContext optionsSettingsSmartContext) :
            base(lifetime, optionsSettingsSmartContext)
        {
            var enabledOption = AddBoolOption((TodoItemsCountSettings s) => s.IsEnabled, "Enabled");

            AddText("Definitions:");
            var definitionsOption = AddStringOption(
                (TodoItemsCountSettings s) => s.Definitions, "",
                acceptsReturn: true,
                toolTipText: "Syntax: <To-do Title>[<Optional text-matching>]");


            enabledOption.CheckedProperty.FlowInto(lifetime, definitionsOption.GetIsEnabledProperty(), x => x);

            FinishPage();
        }
    }
}
