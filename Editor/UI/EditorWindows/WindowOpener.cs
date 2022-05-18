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
    public static Rect windowPosition = new Rect(0f, 0f, 1400f, 624f);
    public static Vector2 windowMinSize = new Vector2(1400f, 624f);
    private static EditorWindow _templateManager;




    [MenuItem(Consts.MenuName + Consts.Window_TemplateManager_Name)]
    internal static void OpenTemplateManager()
    {
        if (string.IsNullOrEmpty(ProjectSettingsService.Instance.GetCurrentProjectDirectory()))
        {
            var wnd = GetWindow<SelectProjectWindow>();
            wnd.titleContent = new GUIContent(Consts.Window_SelectProject_Name);
            wnd.SetOnComplete(OpenTemplateManagerPrivate);
            wnd.position = windowPosition;
            SetWindowSizeAboveMin(wnd);
            wnd.Show();

        } else
        {
            OpenTemplateManagerPrivate();
        }
    }

    private static void OpenTemplateManagerPrivate()
    {
        var wnd = GetWindow<TemplateManager>();
        wnd.titleContent = new GUIContent(Consts.Window_TemplateManager_Name);
        wnd.position = windowPosition;

        SetWindowSizeAboveMin(wnd);
        _templateManager = wnd;
        wnd.Show();
    }
    
    [MenuItem(Consts.MenuName + Consts.Window_RuntimeInspector_Name)]
    public static void OpenRuntimeInspector()
    {
        RunTimeInspector wnd = GetWindow<RunTimeInspector>();
        wnd.titleContent = new GUIContent(Consts.Window_RuntimeInspector_Name);
        wnd.Show();
        wnd.position = windowPosition;
        SetWindowSizeAboveMin(wnd);
    }

    [MenuItem(Consts.MenuName + Consts.Window_Logger_Name)]
    internal static void OpenAiLogger()
    {
        AiLogWindow wnd = GetWindow<AiLogWindow>();
        wnd.titleContent = new GUIContent(Consts.Window_Logger_Name);
        wnd.Show();
        wnd.position = windowPosition;
        SetWindowSizeAboveMin(wnd);
    }

    [MenuItem(Consts.MenuName + Consts.Window_AiTickerManager_Name)]
    public static void OpenAiTickerManager()
    {
        AiTickerSettingsWindow wnd = GetWindow<AiTickerSettingsWindow>();
        wnd.titleContent = new GUIContent(Consts.Window_AiTickerManager_Name);
        wnd.position = windowPosition;
        SetWindowSizeAboveMin(wnd);
        wnd.Show();

    }

    public static ResponseCurveWindow GetNewResponseCurveWindow()
    {
        ResponseCurveWindow wnd = CreateWindow<ResponseCurveWindow>();
        wnd.titleContent = new GUIContent("Response Curve");

        if(_templateManager != null)
        {
            wnd.position = _templateManager.position;

        } else
        {
            wnd.position = windowPosition;
        }
        SetWindowSizeAboveMin(wnd);
        return wnd;
    }

    private static void SetWindowSizeAboveMin(EditorWindow wnd)
    {
        if (wnd.position.x < windowMinSize.x)
        {
            windowPosition.x = windowMinSize.x;
        }

        if (windowPosition.y < windowMinSize.y)
        {
            windowPosition.y = windowMinSize.y;
        }
    }
}
