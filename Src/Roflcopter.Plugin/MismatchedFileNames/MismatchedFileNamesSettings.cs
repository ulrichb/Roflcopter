using System.Diagnostics.CodeAnalysis;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Resources.Settings;

namespace Roflcopter.Plugin.MismatchedFileNames
{
    [SettingsKey(typeof(CodeInspectionSettings), "Mismatched file names")]
    [SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized")]
    public class MismatchedFileNamesSettings
    {
        [SettingsEntry(DefaultValue: @"(.partial)?(\.\w+)$", Description: "Allowed file name postfix regex")]
        public readonly string AllowedFileNamePostfixRegex;
    }
}
