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
    [Action("&Write Debugging Control .ini file with disabled Jitter optimizations")]
    [ExcludeFromCodeCoverage /* manually tested UI code */]
    public class WriteDebuggingControlIniAction : DebuggingControlIniActionBase,
        IExecutableAction,
        IInsertAfter<InsertIntoVsModulesToolWindowContextMenu, GenerateSymbolsForModulesAction>
    {
        public bool Update(IDataContext context, ActionPresentation presentation, [CanBeNull] DelegateUpdate nextUpdate)
        {
            var selectedModulePaths = GetSelectedModulePathsWithOptimizedModules();

            return DebuggingControlIni.GetModulePathsWithoutDebuggingControlIni(selectedModulePaths).Any();
        }

        public void Execute(IDataContext context, [CanBeNull] DelegateExecute nextExecute)
        {
            var selectedModulePaths = GetSelectedModulePathsWithOptimizedModules();

            DebuggingControlIni.WriteDebuggingControlIni(selectedModulePaths);
        }

        private static readonly HashSet<string> NonOptimizedColumnValues = new HashSet<string>(new[] { "No", "N/A" });

        [ItemNotNull]
        private static IEnumerable<FileSystemPath> GetSelectedModulePathsWithOptimizedModules()
        {
            return GetSelectedModulesListViewRows()
                .Where(x => !NonOptimizedColumnValues.Contains(x.Optimized))
                .SelectNotNull(x => x.Path)
                .Select(x => FileSystemPath.Parse(x));
        }
    }
}
#endif
