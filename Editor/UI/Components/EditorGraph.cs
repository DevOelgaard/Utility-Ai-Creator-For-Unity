using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class EditorGraph : GraphView
{
    public EditorGraph()
    {
        Insert(0, new GridBackground());
        //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UAS/Scripts/UtilityAi/EditorWindow/Styles/EditorGraph.uss");

        //styleSheets.Add(styleSheet);
    }

    public new class UxmlFactory : UxmlFactory<EditorGraph, UxmlTraits> { }

}
