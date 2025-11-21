using System;
using JetBrains.Annotations;

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local
// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable ConvertToExtensionBlock

namespace Roflcopter.Sample.AssertionMessages.InvalidAssertionMessageHighlightingTests
{
    public class AssertionMessageExtensionMethodSamples
    {
        void NotNullExtension(SomeClass c) => c.NotNullExtension("c != null");
        void NotNullExtension_Invalid(SomeClass c) => c.NotNullExtension("x != null");

        void NotNullExtensionPropertyAccess(SomeClass c) => c.Prop.NotNullExtension("c.Prop != null");
        void NotNullExtensionPropertyAccess_Invalid(SomeClass c) => c.Prop.NotNullExtension("x.Prop != null");

        void NotNullExtensionWithoutNullEqualityMessage(SomeClass c) => c.NotNullExtension("some message");
        void NotNullExtensionWithoutMessageArgument(SomeClass c) => c.NotNullExtension();

        //

        void NotNullExtensionWithoutContractAnnotationAttribute(string s)
        {
            s.NotNullExtensionWithoutContractAnnotationAttribute("irrelevant != null");
            Console.WriteLine(s == null);
        }

        void NotNullExtensionWithNonMatchingContractAnnotation(string s) =>
            s.NotNullExtensionWithNonMatchingContractAnnotation("irrelevant != null");

        //

        void NotNullExtensionWithStaticInvocation(string s) => AssertionExtensions.NotNullExtension(s, "s != null");

        void NotNullExtensionWithStaticInvocation_Invalid(string s) => AssertionExtensions.NotNullExtension(s, "x != null");

        //

        void NotNullExtensionWithLegacyAnnotation(string s) => s.NotNullExtensionWithLegacyAnnotation("s != null");
        void NotNullExtensionWithLegacyAnnotation_Invalid(string s) => s.NotNullExtensionWithLegacyAnnotation("x != null");

        //

        public class SomeClass
        {
            public string Prop { get; set; }
            public bool Bool { get; set; }
        }
    }

    public static class AssertionExtensions
    {
        [ContractAnnotation("value:null => stop")]
        public static void NotNullExtension(this object value, string message = null)
        {
        }

        public static void NotNullExtensionWithoutContractAnnotationAttribute(this object value, string message)
        {
        }

        [ContractAnnotation("value2:null => stop")]
        public static void NotNullExtensionWithNonMatchingContractAnnotation(this object value, string message, object value2 = null)
        {
        }

        [AssertionMethod]
        public static void NotNullExtensionWithLegacyAnnotation(
            [AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this object value,
            string message)
        {
        }
    }
}
