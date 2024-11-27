using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    #region Fields
    private int popupOrder = 50;

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

    public UIPopup CurrentPopup { get; private set; }
    #endregion

    #region Init

    /// <summary>
    /// ÆË¾÷ÀÇ ¾À ¼³Á¤
    /// </summary>
    /// <param name="uiObject"></param>
    public void SetCanvasPopup(GameObject uiObject)
    {
        var canvas = Utilities.GetOrAddComponent<Canvas>(uiObject);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = popupOrder++; 

        // Canvas Scaler ¼³Á¤
        var canvasScaler = Utilities.GetOrAddComponent<CanvasScaler>(uiObject);
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
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

    public void ClosePopup(string target)
    {
        if (popupStack.Count == 0) return;

        UIPopup popup = popupStack.Pop();

        if (popup.gameObject.name == target)
        {
            popupStack.Push(popup);
            return;
        }

        Destroy(popup.gameObject);
        popupOrder--;
    }

    public void CloseAllPopupUIExcept(string name)
    {
        int count = popupStack.Count;
        for(int i=0;i < count; i++)
        {
            ClosePopup(name);
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
        if (string.IsNullOrEmpty(elementName)) elementName = typeof(T).Name;

        GameObject obj = Manager.Asset.InstantiatePrefab(elementName, UIRoot.transform);
        T element = Utilities.GetOrAddComponent<T>(obj);

        return element;
    }

    private void Destroy(GameObject obj)
    {
        if (obj == null) return;
        UnityEngine.Object.Destroy(obj);
    }
    #endregion
}