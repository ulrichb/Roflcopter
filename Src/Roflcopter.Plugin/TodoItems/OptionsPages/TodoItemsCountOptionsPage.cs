#if RESHARPER
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.Application.UI.Options.OptionsDialog.SimpleOptions;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Features.Inspections.TodoItems;
using JetBrains.ReSharper.Psi.Resources;

namespace Roflcopter.Plugin.TodoItems.OptionsPages
{
    [OptionsPage(
        OptionsPageId,
        "To-do Items Count",
        typeofIcon: typeof(PsiSymbolsThemedIcons.Macro),
        ParentId = TodoExplorerOptionsPage.PID)]
    [ExcludeFromCodeCoverage /* manually tested UI code */]
#pragma warning disable 618
    // TODO after dropping 20181 support: Refactor to BeSimpleOptionsPage
    public class TodoItemsCountOptionsPage : SimpleOptionsPage
#pragma warning restore 618
    {
        public const string OptionsPageId = nameof(TodoItemsCountOptionsPage);

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
#endif
