#if RESHARPER
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.Util;
#if RS20172
using JetBrains.PsiFeatures.VisualStudio.SinceVs10.Debugger;
#else
using JetBrains.PsiFeatures.VisualStudio.Debugger.ExternalSources;

#endif

namespace Roflcopter.Plugin.VisualStudio.ModulesWindow
{
    [Action("&Remove Debugging Control .ini file")]
    [ExcludeFromCodeCoverage /* manually tested UI code */]
    public class RemoveDebuggingControlIniAction : DebuggingControlIniActionBase,
        IExecutableAction,
        IInsertAfter<InsertIntoVsModulesToolWindowContextMenu, WriteDebuggingControlIniAction>
    {
        public bool Update(IDataContext context, ActionPresentation presentation, [CanBeNull] DelegateUpdate nextUpdate)
        {
            return DebuggingControlIni.GetModulePathsWithDebuggingControlIni(GetSelectedModulePaths()).Any();
        }

        public void Execute(IDataContext context, [CanBeNull] DelegateExecute nextExecute)
        {
            DebuggingControlIni.RemoveDebuggingControlIni(GetSelectedModulePaths());
        }

        [ItemNotNull]
        private static IEnumerable<FileSystemPath> GetSelectedModulePaths()
        {
            return GetSelectedModulesListViewRows()
                .SelectNotNull(x => x.Path)
                .Select(x => FileSystemPath.Parse(x));
        }
    }
}
#endif
