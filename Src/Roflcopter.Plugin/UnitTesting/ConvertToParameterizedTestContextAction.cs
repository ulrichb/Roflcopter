using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using Roflcopter.Plugin.Utilities;

namespace Roflcopter.Plugin.UnitTesting
{
    [ContextAction(
        Group = CSharpContextActions.GroupID,
        Name = Name,
        Description = "Converts NUnit test methods to parameterized tests",
        Priority = -5)]
    public class ConvertToParameterizedTestContextAction : ContextActionBase
    {
        private const string Name = "Convert to parameterized test";

        private readonly IContextActionDataProvider _dataProvider;

        public ConvertToParameterizedTestContextAction([NotNull] ICSharpContextActionDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public override string Text => Name;

        public override bool IsAvailable(IUserDataHolder _) => SelectedTestMethodDeclaration != null;

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator _)
        {
            var methodDeclaration = SelectedTestMethodDeclaration.NotNull();
            var psiModule = methodDeclaration.GetPsiModule();

            var elementFactory = CSharpElementFactory.GetInstance(methodDeclaration);

            var parameterDeclaration = methodDeclaration.AddParameterDeclarationBefore(
                elementFactory.CreateParameterDeclaration(
                    ParameterKind.VALUE,
                    isParams: false,
                    isVarArg: false,
                    type: psiModule.GetPredefinedType().Object,
                    name: "parameter",
                    defaultValue: null), anchor: null);

            var testCaseAttributeType = TypeFactory.CreateTypeByCLRName(ParameterizedTests.TestCaseAttribute, NullableAnnotation.Unknown, psiModule)
                .GetTypeElement().NotNull($"Cannot resolve '{ParameterizedTests.TestCaseAttribute}'");

            var testCaseAttribute = methodDeclaration.AddAttributeBefore(elementFactory.CreateAttribute(testCaseAttributeType), anchor: null);

            var testCaseArgument = testCaseAttribute.AddArgumentBefore(
                elementFactory.CreateArgument(ParameterKind.VALUE, elementFactory.CreateExpression("argument")), anchor: null);

            var hotspotNodes = new ITreeNode[]
            {
                parameterDeclaration.TypeUsage,
                parameterDeclaration.NameIdentifier,
                testCaseArgument,
            };

            return textControl =>
            {
                var endSelectionRange = DocumentRange.InvalidRange;
                var hotspotSession = textControl.CreateHotspotSessionAtopExistingText(solution, endSelectionRange, hotspotNodes);

                hotspotSession.ExecuteAndForget();
            };
        }

        [CanBeNull]
        private IMethodDeclaration SelectedTestMethodDeclaration
        {
            get
            {
                var methodDeclaration = _dataProvider.GetSelectedElement<IMethodDeclaration>();

                if (methodDeclaration != null)
                {
                    if (ParameterizedTests.IsTestMethodWithoutParameters(methodDeclaration))
                    {
                        return methodDeclaration;
                    }
                }

                return null;
            }
        }
    }
}
