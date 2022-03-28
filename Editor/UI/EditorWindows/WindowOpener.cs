using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UniRx;
using UniRxExtension;

internal class WindowOpener: EditorWindow
{
    public static Rect WindowPosition = new Rect(0f, 0f, 1920*0.6f, 1080 / 2);

    [MenuItem(Consts.MenuName + Consts.Window_AiInspector_Name)]
    public static void OpenRuntimInspector()
    {
        RunTimeInspector wnd = GetWindow<RunTimeInspector>();
        wnd.titleContent = new GUIContent(Consts.Window_AiInspector_Name);
        wnd.Show();
        wnd.position = WindowPosition;
    }


    [MenuItem(Consts.MenuName + Consts.Window_TemplateManager_Name)]
    internal static void OpenTemplateManager()
    {
        TemplateManager wnd = GetWindow<TemplateManager>();
        wnd.titleContent = new GUIContent(Consts.Window_TemplateManager_Name);
        wnd.Show();
        wnd.position = WindowPosition;
    }

    [MenuItem(Consts.MenuName + Consts.Window_Logger_Name)]
    internal static void OpenAiLogger()
    {
        AiLogWindow wnd = GetWindow<AiLogWindow>();
        wnd.titleContent = new GUIContent(Consts.Window_Logger_Name);
        wnd.Show();
        wnd.position = WindowPosition;
    }

    [MenuItem(Consts.MenuName + Consts.Window_AiTickerManager_Name)]
    public static void OpenAiTickerManager()
    {
        AiTickerSettingsWindow wnd = GetWindow<AiTickerSettingsWindow>();
        wnd.titleContent = new GUIContent(Consts.Window_AiTickerManager_Name);
        wnd.Show();
        wnd.position = WindowPosition;
    }

    public static ResponseCurveWindow OpenResponseCurve()
    {
        ResponseCurveWindow wnd = GetWindow<ResponseCurveWindow>();
        wnd.titleContent = new GUIContent("Response Curve");
        wnd.Show();
        wnd.position = WindowPosition;
        return wnd;
    }
}
