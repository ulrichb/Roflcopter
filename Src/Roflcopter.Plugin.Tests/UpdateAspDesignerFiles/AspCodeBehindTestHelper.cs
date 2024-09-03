#if RESHARPER
using System.Collections.Generic;
using JetBrains.TestFramework;
using JetBrains.Util;

namespace Roflcopter.Plugin.Tests.UpdateAspDesignerFiles
{
    internal class AspCodeBehindTestHelper
    {
        private readonly BaseTestNoShell _test;

        public AspCodeBehindTestHelper(BaseTest test)
        {
            _test = test;
        }

        private string CodeBehindFileName => _test.TestMethodName + ".aspx.cs";
        public string DesignerFileName => _test.TestMethodName + ".aspx.designer.cs";

        public IEnumerable<string> ExtendTestFiles(IEnumerable<string> testFiles) => testFiles.Concat(CodeBehindFileName, DesignerFileName);
    }
}
#endif
