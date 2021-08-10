using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Menu;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Actions;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Resources;

namespace Roflcopter.Plugin.UnitTesting
{
    [Action("UnitTest.RunFile", "Run All Tests in &File",
        Icon = typeof(UnitTestingThemedIcons.RunAll),
        Id = 1962458498,
        IdeaShortcuts = new[] { "Control+T I", "Control+T Control+I" },
        VsShortcuts = new[] { "Control+U I", "Control+U Control+I" })]
    public class UnitTestRunFileAction : UnitTestRunContextActionImpl,
        IInsertAfter<UnitTestContextMenuActionGroup, UnitTestRunContextAction>
    {
        protected override UnitTestElements GetElementsToRun([NotNull] IDataContext context)
        {
            var projectFile = GetProjectFile(context);

            if (projectFile == null)
                return null;

            var criterion = new ProjectFileCriterion(projectFile);

            return new UnitTestElements(criterion);
        }

        [CanBeNull]
        private static IProjectFile GetProjectFile(IDataContext context)
        {
            return context.GetData(ProjectModelDataConstants.PROJECT_MODEL_ELEMENT) as IProjectFile;
        }
    }
}
