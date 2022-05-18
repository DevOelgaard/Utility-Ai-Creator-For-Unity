using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonPopUpWindow: EditorWindow
{
        private Label textLabel;
        private VisualElement buttonContainer;

        public void CreateGUI()
        {
                var root = rootVisualElement;
        
                var treeAsset = AssetService.GetVisualTreeAsset(GetType().FullName);
                treeAsset.CloneTree(root);
                root.styleSheets.Add(StylesService.GetStyleSheet(GetType().FullName));
                textLabel = root.Q<Label>("Text");
                buttonContainer = root.Q<VisualElement>("ButtonContainer");
        }

        internal void Open(string windowTitle, string text, List<Button> buttons)
        {
                titleContent = new GUIContent(windowTitle);

                buttonContainer.Clear();
                textLabel.text = text;
                foreach (var button in buttons)
                {
                        buttonContainer.Add(button);
                }
                Show();
        }
}