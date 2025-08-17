#if RESHARPER
using System.Diagnostics.CodeAnalysis;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.Application.UI.Options.OptionsDialog.SimpleOptions;
using JetBrains.IDE.UI.Extensions;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
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
    public class TodoItemsCountOptionsPage : BeSimpleOptionsPage
    {
        public const string OptionsPageId = nameof(TodoItemsCountOptionsPage);

        public TodoItemsCountOptionsPage(
            Lifetime lifetime,
            OptionsPageContext optionsPageContext,
            OptionsSettingsSmartContext optionsSettingsSmartContext) : base(lifetime, optionsPageContext, optionsSettingsSmartContext)
        {
            AddBoolOption((TodoItemsCountSettings s) => s.IsEnabled, "Enabled");

            AddText("Definitions:");

            // Use BeControls.GetTextControl() + manual Bind() instead of GetBeTextBox() to support multi-line
            // editing (no handling of the "Enter" key). Just like the `ReSpellerSettingsPage`.

            var textControl = BeControls.GetTextControl(isReadonly: false);

            OptionsSettingsSmartContext
                .GetValueProperty(Lifetime, (TodoItemsCountSettings s) => s.Definitions)
                .Bind(Lifetime, textControl.Text);

            AddBinding(textControl, BindingStyle.IsEnabledProperty, (TodoItemsCountSettings s) => s.IsEnabled, x => x);

            AddControl(textControl);
            AddText("Syntax: <To-do title>[<Optional text-matching used to filter the counted items>]");
        }
    }
}
#endif
