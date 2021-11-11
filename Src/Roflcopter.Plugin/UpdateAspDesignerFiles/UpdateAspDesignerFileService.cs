#if RESHARPER
using System;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.UI.Tooltips;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.DocumentModel.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Asp.CodeBehind;
using JetBrains.ReSharper.Psi.Asp.CodeBehind.CodeGeneration;
using JetBrains.TextControl;
using JetBrains.Util;

namespace Roflcopter.Plugin.UpdateAspDesignerFiles
{
    [ShellComponent]
    public class UpdateAspDesignerFileService
    {
        private readonly AspCodeBehindGeneratorProvider _aspCodeBehindGeneratorProvider;
        private readonly ITooltipManager _tooltipManager;

        public const string ActionDescription = "Update ASP.NET Designer File (using ReSharper)";

        public UpdateAspDesignerFileService(AspCodeBehindGeneratorProvider aspCodeBehindGeneratorProvider, ITooltipManager tooltipManager)
        {
            _aspCodeBehindGeneratorProvider = aspCodeBehindGeneratorProvider;
            _tooltipManager = tooltipManager;
        }

        [CanBeNull]
        public Generator GetGenerator(IPsiSourceFile sourceFile)
        {
            var projectFile = sourceFile.ToProjectFile().NotNull("sourceFile.ToProjectFile() != null");

            var aspCodeBehindGenerator = _aspCodeBehindGeneratorProvider.Create(projectFile, AspGeneratorGeneration.Default);
            if (aspCodeBehindGenerator == null)
                return null;

            var project = projectFile.GetProject().NotNull("projectFile.GetProject() != null");
            return new Generator(this, aspCodeBehindGenerator, project);
        }

        public class Generator
        {
            private readonly UpdateAspDesignerFileService _updateAspDesignerFileService;

            private readonly AspCodeBehindGenerator _aspCodeBehindGenerator;
            private readonly IProject _project;

            public Generator(
                UpdateAspDesignerFileService updateAspDesignerFileService,
                AspCodeBehindGenerator aspCodeBehindGenerator,
                IProject project)
            {
                _updateAspDesignerFileService = updateAspDesignerFileService;
                _aspCodeBehindGenerator = aspCodeBehindGenerator;
                _project = project;
            }

            public Action<ITextControl> GenerateAndUpdateDesignerDocument(IProjectModelEditor projectModelEditor)
            {
                var designerCode = GenerateDesignerCode();

                var designerFilePath = _aspCodeBehindGenerator.GeneratedProjectFileLocation;

                var (targetFile, created) = GetOrCreateTargetFile(_project, designerFilePath, projectModelEditor);

                targetFile.GetDocument().SetText(designerCode);

                return textControl =>
                {
                    var message = $"{(created ? "Created" : "Updated")} '{targetFile.GetPresentableProjectPath()}'.";
                    _updateAspDesignerFileService._tooltipManager.Show(message, textControl.PopupWindowContextFactory.ForCaret());
                };
            }

            private string GenerateDesignerCode()
            {
                var stringBuilder = new StringBuilder();

                using (var writer = new StringWriter(stringBuilder))
                    _aspCodeBehindGenerator.Generate(writer);

                return stringBuilder.ToString();
            }

            private static (IProjectFile, bool Created) GetOrCreateTargetFile(
                IProject project,
                VirtualFileSystemPath targetFilePath,
                IProjectModelEditor projectModelEditor)
            {
                var targetFile = FindSingleProjectItemByLocation<IProjectFile>(project, targetFilePath);

                if (targetFile != null)
                    return (targetFile, Created: false);

                var projectFolder = FindSingleProjectItemByLocation<IProjectFolder>(project, targetFilePath.Directory)
                    .NotNull("Cannot find parent folder");
                var newProjectFile = projectModelEditor.AddFile(projectFolder, targetFilePath);

                return (newProjectFile, Created: true);
            }

            [CanBeNull]
            private static T FindSingleProjectItemByLocation<T>(IProject project, VirtualFileSystemPath path) where T : IProjectItem =>
                project.FindProjectItemsByLocation(path).OfType<T>().SingleOrDefault();
        }
    }
}
#endif
