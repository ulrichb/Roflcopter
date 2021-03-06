﻿using System;
using JetBrains.Annotations;

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local

namespace Roflcopter.Sample.AssertionMessages.InvalidAssertionMessageHighlightingTests
{
    public class AssertionMessageLegacyAnnotationSamples
    {
        // Note: These messages are generated by 'CSharpAssertionMessageItemProvider'.

        void AssertIsTrueNotNull(SomeClass c)
        {
            AssertIsTrue(c != null, "c != null");
            Console.WriteLine(c != null);
        }

        void AssertIsTrueNotNull_Invalid(SomeClass c) => AssertIsTrue(c != null, "x != null");

        void AssertIsTrueNotNullPropertyAccess(SomeClass c) => AssertIsTrue(c.Prop != null, "c.Prop != null");
        void AssertIsTrueNotNullPropertyAccess_Invalid(SomeClass c) => AssertIsTrue(c.Prop != null, "x.Prop != null");

        //

        void AssertIsTrueWithoutConditionAttribute(string s)
        {
            AssertIsTrueWithoutConditionAttribute(s != null, "irrelevant != null");
            Console.WriteLine(s != null);
        }

        void AssertIsTrueWithMultipleConditions(string s1, string s2, string s3)
        {
            AssertIsTrueWithMultipleConditions(s1 != null, "x1 != null", s2 != null, "irrelevant != null", s3 != null, "x3 != null");
            Console.WriteLine(s1 != null); // R# seems to only use the last [AssertionCondition]
            Console.WriteLine(s2 != null);
            Console.WriteLine(s3 != null);
        }

        //

        void AssertIsFalse(string s) => AssertIsFalse(s == null, "s == null");
        void AssertIsFalse_Invalid(string s) => AssertIsFalse(s == null, "x == null");
        void AssertNotNull(string s) => AssertNotNull(s, "s != null");
        void AssertNotNull_Invalid(string s) => AssertNotNull(s, "x != null");
        void AssertNull(string s) => AssertNull(s, "s == null");
        void AssertNull_Invalid(string s) => AssertNull(s, "x == null");

        //

        [AssertionMethod]
        public static void AssertIsTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition, string message)
        {
        }

        [AssertionMethod]
        public static void AssertIsTrueWithoutConditionAttribute(bool condition, string message)
        {
        }

        [AssertionMethod]
        public static void AssertIsTrueWithMultipleConditions(
            [AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition1,
            string message1,
            bool condition2,
            string message2,
            [AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition3,
            string message3)
        {
        }

        [AssertionMethod]
        public static void AssertIsFalse([AssertionCondition(AssertionConditionType.IS_FALSE)] bool condition, string message)
        {
        }

        [AssertionMethod]
        public static void AssertNotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] object value, string message)
        {
        }

        [AssertionMethod]
        public static void AssertNull([AssertionCondition(AssertionConditionType.IS_NULL)] object value, string message)
        {
        }

        public class SomeClass
        {
            public string Prop { get; set; }
            public bool Bool { get; set; }
        }
    }
}
