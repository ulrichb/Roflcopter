using JetBrains.Annotations;

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local

namespace Roflcopter.Sample.AssertionMessages.InvalidAssertionMessageHighlightingTests
{
    public class AssertionMessageErrorSamples
    {
        void MissingMessageArgument(string s) => AssertIsTrue(s != null);

        void InvalidMessageArgumentType(string s) => AssertIsTrue(s != null, 666);

        void NonMatchingArguments(string s) => AssertIsTrue(true, "dont care", s != null, "s != null");

        void UnresolvableCall(string s) => Unresolvable(s != null, "s != null");

        void ExtensionCallWithoutParenthesis(string s) => s.ErrorSamplesNotNullExtension;

        [ContractAnnotation("condition:false => stop")]
        public static void AssertIsTrue(bool condition, string message)
        {
        }

        void NonErrorSample(string s) => s.ErrorSamplesNotNullExtension("x != null");
    }

    public static class AssertionMessageErrorSamplesExtensions
    {
        [ContractAnnotation("value:null => stop")]
        public static void ErrorSamplesNotNullExtension(this object value, string message)
        {
        }
    }
}
