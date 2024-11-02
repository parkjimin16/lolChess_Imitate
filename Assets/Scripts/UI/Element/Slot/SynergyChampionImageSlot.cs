using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SynergyChampionImageSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image chamionImage;
    [SerializeField] private Image outline;
    private List<string> symbolName;
    private string championName;

    private void Awake()
    {
        symbolName = new List<string>();
    }

    public void InitSlot(Sprite champion, Color championColor ,Color outlineColor, List<string> name, string championName)
    {
        chamionImage.sprite = champion;
        chamionImage.color = championColor;
        outline.color = outlineColor;
        symbolName.Clear();
        symbolName.AddRange(name);
        this.championName = championName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ShowPopupUI();
        }
    }

    private void ShowPopupUI()
    {
        var symbolChampionDetailPopup = Manager.UI.ShowPopup<UIPopupSynergyChampionDetail>();

        if (symbolChampionDetailPopup == null)
            return;

        symbolChampionDetailPopup.SetChampionSymbolPopupData(championName, symbolName);
    }

}
