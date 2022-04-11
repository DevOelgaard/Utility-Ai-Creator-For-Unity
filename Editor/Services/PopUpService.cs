using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PopUpService: EditorWindow
{
        private static ButtonPopUpWindow ButtonPopUp => GetWindow<ButtonPopUpWindow>();
        public static void AskToSaveIfProjectNotSavedThenSelectProjectToLoad()
        {
                InitWithThreeButtonsThenAction(LoadProject);
        }
        
        public static void AskToSaveIfProjectNotSavedThenCreateNew()
        {
                InitWithThreeButtonsThenAction(CreateNewProject);
        }

        private static void InitWithThreeButtonsThenAction(Action action)
        {
                if (ProjectSettingsService.Instance.ProjectSaved())
                {
                        action.Invoke();
                        return;
                }

                var saveButton = new Button(() =>
                {
                        UasTemplateService.Instance.Save();
                        action.Invoke();
                });
                saveButton.text = "Save";

                var noSaveButton = new Button(action)
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

        private static void LoadProject()
        {
                ProjectSettingsService.Instance.LoadProject();
                UasTemplateService.Instance.LoadCurrentProject();

                var tmpManager = GetWindow<TemplateManager>();
                tmpManager.UpdateLeftPanel();
                ButtonPopUp.Close();
        }

        private static void CreateNewProject()
        {
                ProjectSettingsService.Instance.CreateProject();
                UasTemplateService.Instance.LoadCurrentProject();
        }
}