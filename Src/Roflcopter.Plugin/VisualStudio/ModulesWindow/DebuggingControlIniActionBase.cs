#if RESHARPER
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
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

        protected static IEnumerable<ModulesListViewRow> GetSelectedModulesListViewRows()
        {
            var focus = NativeMethods.GetFocus();

            var stringList = new List<ModulesListViewRow>();

            if (focus == IntPtr.Zero)
                return stringList;

            var selectedCount = NativeMethods.GetSelectedCount(focus);

            var currentItemIndex = -1;

            for (var i = 0; i < selectedCount; i++)
            {
                currentItemIndex = NativeMethods.GetNextSelectedItemIndex(focus, currentItemIndex);
                Assertion.Assert(currentItemIndex >= 0, "currentItemIndex >= 0");

                stringList.Add(new ModulesListViewRow(
                    path: NativeMethods.GetText(focus, currentItemIndex, column: 2),
                    optimized: NativeMethods.GetText(focus, currentItemIndex, column: 3)));
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

        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetFocus();

            public static int GetSelectedCount(IntPtr hWnd)
            {
                return SendMessage(hWnd, LvmGetSelectedCount, IntPtr.Zero, IntPtr.Zero).ToInt32();
            }

            public static int GetNextSelectedItemIndex(IntPtr hWnd, int currentIndex)
            {
                return SendMessage(hWnd, LvmGetNextItem, (IntPtr) currentIndex, (IntPtr) LvniSelected).ToInt32();
            }

            [CanBeNull]
            public static string GetText(IntPtr hWnd, int itemIndex, int column)
            {
                string result = null;

                const int maxTextLength = 1024;
                var textPtr = Marshal.AllocHGlobal(maxTextLength * 2);
                try
                {
                    var lvItem = new LvItem
                    {
                        mask = LvifText,
                        iItem = itemIndex,
                        iSubItem = column,
                        pszText = textPtr,
                        cchTextMax = maxTextLength
                    };

                    if (SendMessage(hWnd, LvmGetItemW, IntPtr.Zero, ref lvItem) != IntPtr.Zero)
                    {
                        result = Marshal.PtrToStringUni(textPtr);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(textPtr);
                }

                return result;
            }

            private const int LvniSelected = 0x0002;

            private const int LvmFirst = 0x1000;
            private const int LvmGetNextItem = LvmFirst + 12;
            private const int LvmGetSelectedCount = LvmFirst + 50;
            private const int LvmGetItemW = LvmFirst + 75;

            [StructLayout(LayoutKind.Sequential)]
            private struct LvItem
            {
                public int mask;
                public int iItem;
                public int iSubItem;
                private readonly int state;
                private readonly int stateMask;
                public IntPtr pszText;
                public int cchTextMax;
                private readonly int iImage;
                private readonly IntPtr lParam;
            }

            private const int LvifText = 0x0001;

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, ref LvItem lParam);
        }
    }
}
#endif
