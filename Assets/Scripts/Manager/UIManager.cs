using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    #region Fields
    private int sceneOrder = 10;
    private int popupOrder = 50;

    private readonly Stack<UIScene> sceneStack = new();
    private readonly Stack<UIPopup> popupStack = new();
    #endregion

    #region Property

    public GameObject UIRoot
    {
        get
        {
            GameObject root = GameObject.Find("@UIRoot") ?? new GameObject("@UIRoot");
            return root;
        }
    }

    public UITopScene Top { get; private set; }
    public UIScene CurrentScene { get; private set; } 
    public UIScene CurrentSubScene { get; private set; } 
    public UIPopup CurrentPopup { get; private set; }
    #endregion

    #region Init


    /// <summary>
    /// 씬의 Canvas 설정
    /// </summary>
    /// <param name="uiObject"></param>
    public void SetCanvasScene(GameObject uiObject)
    {
        var canvas = Utilities.GetOrAddComponent<Canvas>(uiObject);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = sceneOrder++;


        var canvasScaler = Utilities.GetOrAddComponent<CanvasScaler>(uiObject);
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
    }

    /// <summary>
    /// 팝업의 씬 설정
    /// </summary>
    /// <param name="uiObject"></param>
    public void SetCanvasPopup(GameObject uiObject)
    {
        var canvas = Utilities.GetOrAddComponent<Canvas>(uiObject);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = popupOrder++; // 팝업의 정렬 순서를 증가

        // Canvas Scaler 설정
        var canvasScaler = Utilities.GetOrAddComponent<CanvasScaler>(uiObject);
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
    }
    #endregion

    #region Scene
    public void InitTop(UITopScene uITopMain)
    {
        Top = uITopMain; 
    }

    public T ShowScene<T>(string sceneName = null) where T : UIScene
    {
        if (string.IsNullOrEmpty(sceneName)) sceneName = typeof(T).Name;

        GameObject obj = Manager.Asset.InstantiatePrefab(sceneName, UIRoot.transform);
        T scene = Utilities.GetOrAddComponent<T>(obj);
        CurrentScene = scene;

        return scene;
    }

    public T ShowSubScene<T>(string sceneName = null) where T : UIScene
    {
        if(string.IsNullOrEmpty(sceneName)) sceneName= typeof(T).Name;

        if(CurrentSubScene != null)
        {
            CloseSubScene();
        }

        GameObject obj = Manager.Asset.InstantiatePrefab(sceneName, UIRoot.transform);
        T scene = Utilities.GetOrAddComponent<T>(obj);
        CurrentSubScene = scene;
        sceneStack.Push(scene);

        return scene;
    }

    public void CloseSubScene()
    {
        if (sceneStack.Count == 0) return;

        UIScene scene = sceneStack.Pop();
        Destroy(scene.gameObject);
        sceneOrder--;
    }

    public bool CheckSceneStack()
    {
        return sceneStack.Count != 0;
    }
    #endregion

    #region Popup

    public T ShowPopup<T>(string popupName = null) where T : UIPopup
    {
        if (string.IsNullOrEmpty(popupName)) popupName = typeof(T).Name;

        for (int i = popupStack.Count - 1; i >= 0; i--)
        {
            var _popup = popupStack.Peek(); 
            if (_popup.name == popupName)
            {
                Destroy(_popup.gameObject);
                popupStack.Pop(); 
            }
            else
            {
                break;
            }
        }

        GameObject obj = Manager.Asset.InstantiatePrefab(popupName, UIRoot.transform);
        T popup = Utilities.GetOrAddComponent<T>(obj);
        CurrentPopup = popup;
        popupStack.Push(popup);

        return popup;
    }


    public void ClosePopup()
    {
        if (popupStack.Count == 0) return;

        UIPopup popup = popupStack.Pop();
        Destroy(popup.gameObject);
        popupOrder--;
    }


    public void CloseAllPopupUI()
    {
        while (popupStack.Count > 0)
        {
            ClosePopup(); 
        }
    }

    public bool CheckPopupStack()
    {
        return popupStack.Count != 0; 
    }

    #endregion

    #region Elements

    public T AddElement<T>(string elementName = null) where T : UIBase
    {
        if(string.IsNullOrEmpty(elementName)) elementName = typeof(T).Name;

        GameObject obj = Manager.Asset.InstantiatePrefab(elementName, UIRoot.transform);
        T element = Utilities.GetOrAddComponent<T>(obj);

        return element;
    }

    private void Destroy(GameObject obj)
    {
        if(obj == null) return;
        UnityEngine.Object.Destroy(obj);
    }
    #endregion
}
