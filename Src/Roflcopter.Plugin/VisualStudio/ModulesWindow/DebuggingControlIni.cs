#if !RS20171
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Util;

namespace Roflcopter.Plugin.VisualStudio.ModulesWindow
{
    public class DebuggingControlIni
    {
        private readonly Action<string> _showError;
        private readonly Action<string> _showInfo;

        public DebuggingControlIni(Action<string> showError, Action<string> showInfo)
        {
            _showError = showError;
            _showInfo = showInfo;
        }

        public IEnumerable<FileSystemPath> GetModulePathsWithoutDebuggingControlIni(IEnumerable<FileSystemPath> optimizedModulesPaths)
        {
            return optimizedModulesPaths.Where(x => x.IsAbsolute && !GetIniPathForModule(x).ExistsFile);
        }

        public IEnumerable<FileSystemPath> GetModulePathsWithDebuggingControlIni(IEnumerable<FileSystemPath> modulesPaths)
        {
            return modulesPaths.Where(x => x.IsAbsolute && GetIniPathForModule(x).ExistsFile);
        }

        public void WriteDebuggingControlIni(IEnumerable<FileSystemPath> optimizedModulesPaths)
        {
            var modulePaths = GetModulePathsWithoutDebuggingControlIni(optimizedModulesPaths).ToList();

            foreach (var modulePath in modulePaths)
            {
                try
                {
                    CreateForModule(modulePath);
                }
                catch (Exception ex)
                {
                    _showError($"Error while writing .ini for '{modulePath.Name}': " + ex.Message);

                    return;
                }
            }

            _showInfo(
                "Wrote .ini file for:" + Environment.NewLine +
                string.Join(Environment.NewLine, modulePaths.Select(x => x.Name)) + Environment.NewLine +
                Environment.NewLine +
                "Please restart the target application. " + Environment.NewLine +
                "Note that Ngen-precompiled assemblies have to be precompiled again, unless the .ini has no effect.");
        }

        public void RemoveDebuggingControlIni(IEnumerable<FileSystemPath> modulePaths)
        {
            var modulePathsWithDebuggingControlIni = GetModulePathsWithDebuggingControlIni(modulePaths).ToList();

            foreach (var modulePath in modulePathsWithDebuggingControlIni)
            {
                try
                {
                    DeleteForModule(modulePath);
                }
                catch (Exception ex)
                {
                    _showError($"Error while deleting .ini file for '{modulePath.Name}': " + ex.Message);

                    return;
                }
            }

            _showInfo(
                "Deleted .INI file for:" + Environment.NewLine +
                string.Join(Environment.NewLine, modulePathsWithDebuggingControlIni.Select(x => x.Name)) + Environment.NewLine);
        }

        private static void CreateForModule(FileSystemPath modulePath)
        {
            var iniPath = GetIniPathForModule(modulePath);

            var content = "[.NET Framework Debugging Control]" + Environment.NewLine +
                          "GenerateTrackingInfo=1" + Environment.NewLine +
                          "AllowOptimize=0" + Environment.NewLine;

            var ansiEncoding = Encoding.Default;
            iniPath.WriteAllText(content, ansiEncoding);
        }

        private static void DeleteForModule(FileSystemPath modulePath)
        {
            var iniPath = GetIniPathForModule(modulePath);

            iniPath.DeleteFile();
        }

        private static FileSystemPath GetIniPathForModule(FileSystemPath path) => path.ChangeExtension(".ini");
    }
}
#endif
