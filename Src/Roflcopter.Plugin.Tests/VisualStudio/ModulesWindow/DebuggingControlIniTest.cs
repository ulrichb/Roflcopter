#if !RS20171
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Util;
using NUnit.Framework;
using Roflcopter.Plugin.VisualStudio.ModulesWindow;

namespace Roflcopter.Plugin.Tests.VisualStudio.ModulesWindow
{
    [TestFixture]
    public class DebuggingControlIniTest
    {
        private DebuggingControlIni _sut;
        private FileSystemPath _sampleModulePath1Relative;
        private FileSystemPath _sampleModulePath1;
        private FileSystemPath _sampleModulePath2;
        private List<string> _errors;
        private List<string> _infos;

        [SetUp]
        public void SetUp()
        {
            _sampleModulePath1Relative = FileSystemPath.Parse("module1.dll");
            _sampleModulePath1 = _sampleModulePath1Relative.MakeAbsoluteBasedOn(FileSystemPath.Parse(Environment.CurrentDirectory));
            _sampleModulePath2 = FileSystemPath.Parse("module2.dll").MakeAbsoluteBasedOn(FileSystemPath.Parse(Environment.CurrentDirectory));

            _errors = new List<string>();
            _infos = new List<string>();
            _sut = new DebuggingControlIni(error => _errors.Add(error), info => _infos.Add(info));
        }

        [TearDown]
        public void TearDown()
        {
            _sampleModulePath1.ChangeExtension(".ini").DeleteFile();
            _sampleModulePath2.ChangeExtension(".ini").DeleteFile();
        }

        public class WithoutExistingIni : DebuggingControlIniTest
        {
            [Test]
            public void GetModulePathsWithoutDebuggingControlIni()
            {
                var result = _sut.GetModulePathsWithoutDebuggingControlIni(new[] { _sampleModulePath1 });

                Assert.That(result, Is.EqualTo(new[] { _sampleModulePath1 }));
            }

            [Test]
            public void GetModulePathsWithoutDebuggingControlIni_AndRelativePath()
            {
                var result = _sut.GetModulePathsWithoutDebuggingControlIni(new[] { _sampleModulePath1Relative });

                Assert.That(result, Is.Empty);
            }

            //

            [Test]
            public void GetModulePathsWithDebuggingControlIni()
            {
                var result = _sut.GetModulePathsWithDebuggingControlIni(new[] { _sampleModulePath1 });

                Assert.That(result, Is.Empty);
            }

            //

            [Test]
            public void WriteDebuggingControlIni()
            {
                var modulesPaths = new[] { _sampleModulePath1, FileSystemPath.Parse("invalid.dll"), _sampleModulePath2 };

                _sut.WriteDebuggingControlIni(modulesPaths);

                Assert.That(
                    _infos.Single(),
                    Is.StringStarting($"Wrote .ini file for:{Environment.NewLine}module1.dll{Environment.NewLine}module2.dll"));

                var iniText = _sampleModulePath1.ChangeExtension(".ini").ReadAllText2(Encoding.Default).Text;
                Assert.That(iniText, Is.StringStarting("[.NET Framework Debugging Control]"));
            }

            //

            [Test]
            public void WriteDebuggingControlIni_WithError()
            {
                // Use this trick to lock the first item's .ini file _after_ it has been enumerated:
                Stream stream = null;
                var modulesPathsEnumerable = new[] { _sampleModulePath1, _sampleModulePath2 }.Select((x, i) =>
                {
                    if (i == 1)
                        stream = _sampleModulePath1.ChangeExtension(".ini").OpenStream(FileMode.CreateNew);

                    return x;
                });

                try
                {
                    _sut.WriteDebuggingControlIni(modulesPathsEnumerable);

                    Assert.That(_errors.Single(), Is.StringStarting($"Error while writing .ini for '{_sampleModulePath1.Name}': "));
                }
                finally
                {
                    stream.Dispose();
                }
            }
        }

        public class WithExistingIniFiles : DebuggingControlIniTest
        {
            [SetUp]
            public new void SetUp()
            {
                _sampleModulePath1.ChangeExtension(".ini").WriteAllText("");
                _sampleModulePath2.ChangeExtension(".ini").WriteAllText("");
            }

            [Test]
            public void GetModulePathsWithoutDebuggingControlIni()
            {
                var result = _sut.GetModulePathsWithoutDebuggingControlIni(new[] { _sampleModulePath1 });

                Assert.That(result, Is.Empty);
            }

            //

            [Test]
            public void GetModulePathsWithDebuggingControlIni()
            {
                var result = _sut.GetModulePathsWithDebuggingControlIni(new[] { _sampleModulePath1 });

                Assert.That(result, Is.EqualTo(new[] { _sampleModulePath1 }));
            }

            [Test]
            public void GetModulePathsWithDebuggingControlIni_AndRelativePath()
            {
                var result = _sut.GetModulePathsWithDebuggingControlIni(new[] { _sampleModulePath1Relative });

                Assert.That(result, Is.Empty);
            }

            //

            [Test]
            public void RemoveDebuggingControlIni()
            {
                var modulesPaths = new[] { _sampleModulePath1, FileSystemPath.Parse("invalid.dll"), _sampleModulePath2 };

                _sut.RemoveDebuggingControlIni(modulesPaths);

                Assert.That(
                    _infos.Single(),
                    Is.StringStarting($"Deleted .INI file for:{Environment.NewLine}module1.dll{Environment.NewLine}module2.dll"));
            }

            //

            [Test]
            public void RemoveDebuggingControlIni_WithError()
            {
                using (_sampleModulePath1.ChangeExtension(".ini").OpenStream(FileMode.Open))
                {
                    var modulesPaths = new[] { _sampleModulePath1, _sampleModulePath2 };

                    _sut.RemoveDebuggingControlIni(modulesPaths);

                    Assert.That(_errors.Single(), Is.StringStarting($"Error while deleting .ini file for '{_sampleModulePath1.Name}': "));
                }
            }
        }
    }
}
#endif
