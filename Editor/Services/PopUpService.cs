using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PopUpService: EditorWindow
{
        private static ButtonPopUpWindow ButtonPopUp => GetWindow<ButtonPopUpWindow>();
        public static async Task AskToSaveIfProjectNotSavedThenSelectProjectToLoad()
        {
                await InitWithThreeButtonsThenAction(LoadProject);
        }
        
        public static async Task AskToSaveIfProjectNotSavedThenCreateNew()
        {
                await InitWithThreeButtonsThenAction(CreateNewProject);
        }

        private static async Task InitWithThreeButtonsThenAction(Func<Task> task)
        {
                if (ProjectSettingsService.Instance.ProjectSaved())
                {
                        await task.Invoke();
                        return;
                }

                async void Save()
                {
                        await UasTemplateService.Instance.Save();
                        await task.Invoke();
                }

                var saveButton = new Button(Save)
                {
                        text = "Save"
                };

                async void RunTask()
                {
                        await task.Invoke();
                }

                var noSaveButton = new Button(RunTask)
                {
                        text = "Don't Save"
                };

                var cancelButton = new Button(ButtonPopUp.Close)
                {
                        text = "Cancel"
                };

                var buttonList = new List<Button>()
                {
                        saveButton,
                        noSaveButton,
                        cancelButton
                };
                
                ButtonPopUp.Open("Project Not Saved!","Project not saved. Save before continuing?", buttonList);

                var tmpManagerPos = GetWindow<TemplateManager>().position;
                var size = new Vector2(ButtonPopUp.position.width, ButtonPopUp.position.height);
                var centerPosition = new Vector2(tmpManagerPos.x / 1.5f - size.x/2,
                        tmpManagerPos.y * 1.5f + size.y/2);
                
                ButtonPopUp.position = new Rect(centerPosition, size);
        }

        private static async Task LoadProject()
        {
                ButtonPopUp.Close();
                ProjectSettingsService.Instance.LoadProject();
                await UasTemplateService.Instance.LoadCurrentProject();
        }

        private static async Task CreateNewProject()
        {
                ButtonPopUp.Close();
                await ProjectSettingsService.Instance.CreateProject();
                await UasTemplateService.Instance.LoadCurrentProject();
        }
}