using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

internal class StylesService
{
    internal static StyleSheet GetStyleSheet(string name)
    {
        var guid = AssetDatabase.FindAssets(name + " t:StyleSheet").First();
        var path = AssetDatabase.GUIDToAssetPath(guid);
        if (!string.IsNullOrEmpty(path))
        {
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
        } else
        {
            return null;
        } 
    }
}