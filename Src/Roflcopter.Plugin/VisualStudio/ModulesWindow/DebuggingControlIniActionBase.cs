#if RESHARPER
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using JetBrains.Interop.WinApi;
using JetBrains.Util;

namespace Roflcopter.Plugin.VisualStudio.ModulesWindow
{
    [ExcludeFromCodeCoverage /* manually tested UI code */]
    public abstract class DebuggingControlIniActionBase
    {
        protected readonly DebuggingControlIni DebuggingControlIni;

        protected DebuggingControlIniActionBase()
        {
            const string messageBoxCaption = "Debugging Control .ini";

            DebuggingControlIni = new DebuggingControlIni(
                x => MessageBox.ShowError(x, messageBoxCaption),
                x => MessageBox.ShowInfo(x, messageBoxCaption));
        }

        protected static unsafe IEnumerable<ModulesListViewRow> GetSelectedModulesListViewRows()
        {
            var focus = User32Dll.GetFocus();

            var stringList = new List<ModulesListViewRow>();
            if ((IntPtr) focus == IntPtr.Zero)
                return stringList;
            var selectedCount = ListViewUtil.GetSelectedCount(focus);

            var num = -1;

            for (var index = 0; index < selectedCount; ++index)
            {
                num = ListViewUtil.GetNextSelectedItemIndex(focus, num);
                if (num != -1)
                {
                    stringList.Add(new ModulesListViewRow(
                        path: ListViewUtil.GetText(focus, num, 2),
                        optimized: ListViewUtil.GetText(focus, num, 3)));
                }
            }
            return stringList;
        }

        protected class ModulesListViewRow
        {
            public ModulesListViewRow([CanBeNull] string path, [CanBeNull] string optimized)
            {
                Path = path;
                Optimized = optimized;
            }

            [CanBeNull]
            public string Path { get; }

            [CanBeNull]
            public string Optimized { get; }
        }
    }
}
#endif
