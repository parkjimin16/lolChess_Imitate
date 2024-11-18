using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemCombineSlot : MonoBehaviour
{
    #region º¯¼ö

    [SerializeField] private Image image_SecondItem;
    [SerializeField] private Image image_CombineItem;
    [SerializeField] private TextMeshProUGUI txt_CombineItem_Name;

    #endregion

    public void InitSlot(ItemBlueprint second, ItemBlueprint combine)
    {
        image_SecondItem.sprite = second.Icon;
        image_CombineItem.sprite = combine.Icon;
        txt_CombineItem_Name.text = combine.ItemName;
    }
}
