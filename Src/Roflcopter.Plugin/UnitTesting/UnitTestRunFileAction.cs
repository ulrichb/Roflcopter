using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Menu;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Actions;
using JetBrains.ReSharper.UnitTestFramework.Common;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Resources;
#if RS20171
using JetBrains.UI.ActionsRevised;
#else
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;

#endif

namespace Roflcopter.Plugin.UnitTesting
{
    [Action("UnitTest.RunFile", "&Run Unit Tests in File",
        Icon = typeof(UnitTestingThemedIcons.RunTest),
        Id = 1962458498,
        IdeaShortcuts = new[] { "Control+T F", "Control+T Control+F" },
        VsShortcuts = new[] { "Control+U F", "Control+U Control+F" })]
    public class UnitTestRunFileAction : UnitTestRunContextActionImpl,
        IInsertAfter<UnitTestContextMenuActionGroup, UnitTestRunContextAction>
    {
        [CanBeNull]
        protected override UnitTestElements GetElementsToRun([NotNull] IDataContext context)
        {
            var projectFile = GetProjectFile(context);

            if (projectFile == null)
                return null;

            var unitTestElements = context.GetData(UnitTestDataConstants.UnitTestElements.SELECTED);

            if (unitTestElements == null)
                return null;

            var criterion = new ProjectFileCriterion(projectFile);

            return new UnitTestElements(criterion, unitTestElements.Explicit);
        }

        [CanBeNull]
        private static IProjectFile GetProjectFile(IDataContext context)
        {
            return context.GetData(ProjectModelDataConstants.PROJECT_MODEL_ELEMENT) as IProjectFile;
        }
    }
}
