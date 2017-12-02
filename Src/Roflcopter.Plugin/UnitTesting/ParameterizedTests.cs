using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using Roflcopter.Plugin.Utilities;

namespace Roflcopter.Plugin.UnitTesting
{
    public static class ParameterizedTests
    {
        private static readonly IClrTypeName TestAttribute = new ClrTypeName("NUnit.Framework.TestAttribute");

        public static readonly IClrTypeName TestBuilderAttribute = new ClrTypeName("NUnit.Framework.Interfaces.ITestBuilder");
        public static readonly IClrTypeName TestCaseAttribute = new ClrTypeName("NUnit.Framework.TestCaseAttribute");
        public static readonly IClrTypeName TestCaseSourceAttribute = new ClrTypeName("NUnit.Framework.TestCaseSourceAttribute");

        public static readonly IClrTypeName ParameterDataSourceInterface = new ClrTypeName("NUnit.Framework.Interfaces.IParameterDataSource");
        public static readonly IClrTypeName ValuesAttribute = new ClrTypeName("NUnit.Framework.ValuesAttribute");
        public static readonly IClrTypeName ValueSourceAttribute = new ClrTypeName("NUnit.Framework.ValueSourceAttribute");

        public static bool IsTestOrTestCaseMethod(IMethodDeclaration methodDeclaration)
        {
            return methodDeclaration.AttributesEnumerable.Any(x => x.IsAttributeOrDerivedFrom(TestAttribute, TestCaseAttribute));
        }
    }
}
