#if RS20171
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Resolve.ExtensionMethods;

namespace Roflcopter.Plugin.AssertionMessages
{
    public static class Compatibility
    {
        [Pure]
        public static bool IsExtensionMethodInvocation(this IResolveResult resolveResult) => resolveResult.IsExtensionMethod();
    }
}
#endif
