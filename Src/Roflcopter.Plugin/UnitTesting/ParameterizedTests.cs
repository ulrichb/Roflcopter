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

        public static readonly IClrTypeName TestCaseAttribute = new ClrTypeName("NUnit.Framework.TestCaseAttribute");

        public static bool IsTestMethodWithoutParameters(IMethodDeclaration methodDeclaration)
        {
            return methodDeclaration.AttributesEnumerable.Any(x => x.IsAttributeOrDerivedFrom(TestAttribute)) &&
                   methodDeclaration.ParameterDeclarations.Count == 0;
        }
    }
}
