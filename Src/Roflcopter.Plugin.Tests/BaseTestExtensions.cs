using JetBrains.Diagnostics;
using JetBrains.TestFramework;

namespace Roflcopter.Plugin.Tests
{
    internal static class BaseTestExtensions
    {
        public static string CalculateRelativeTestDataPath(this BaseTest test)
        {
            return test.GetType().FullName.NotNull()
                .Replace("Roflcopter.Plugin.Tests.", "").Replace('.', '/').Replace('+', '/');
        }
    }
}
