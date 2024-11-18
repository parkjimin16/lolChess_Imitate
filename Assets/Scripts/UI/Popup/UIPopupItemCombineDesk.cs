using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIPopupItemCombineDesk : UIPopup
{
    #region ���� �� ������Ƽ
    [SerializeField] private List<ItemCombineDesk> itemCombineList;
    [Header("������ ����")]
    [SerializeField] private Image image_FirstItem;
    [SerializeField] private Image image_FirstItem_Attribute_Icon_1;
    [SerializeField] private Image image_FirstItem_Attribute_Icon_2;

    [SerializeField] private TextMeshProUGUI txt_FirstItem_Name;
    [SerializeField] private TextMeshProUGUI txt_FirstItem_Attribute_Name;
    [SerializeField] private TextMeshProUGUI txt_FirstITem_Attribute_Value_1;
    [SerializeField] private TextMeshProUGUI txt_FirstITem_Attribute_Value_2;

    [Header("������ ���� ��")]
    [SerializeField] private List<GameObject> itemCombineSlotList;

    #endregion

    #region �ʱ�ȭ

    public void InitItemCombinePopup(List<ItemCombineDesk> list, string itemId)
    {
        itemCombineList = list;

        ItemBlueprint item = Manager.Item.FindItemById(itemId);

        image_FirstItem.sprite = item.Icon;
        image_FirstItem_Attribute_Icon_1.sprite = Manager.Item.GetIcon(item.Attribute[0].ItemAttributeType);
        image_FirstItem_Attribute_Icon_2.sprite = Manager.Item.GetIcon(item.Attribute[0].ItemAttributeType);

        txt_FirstItem_Name.text = item.ItemName;
        txt_FirstItem_Attribute_Name.text = Utilities.SetItemAttributeDescription(item.Attribute[0].ItemAttributeType);
        txt_FirstITem_Attribute_Value_1.text = Utilities.SetDescriptionValueReturnString(item.Attribute[0]);
        txt_FirstITem_Attribute_Value_2.text = Utilities.SetDescriptionValueReturnString(item.Attribute[0]);

        for (int i=0;i < itemCombineSlotList.Count; i++)
        {
            if(i < itemCombineList.Count)
            {
                itemCombineSlotList[i].SetActive(true);

                ItemCombineDesk itemCombineDesk = itemCombineList[i];
                string secondItemId = (itemCombineDesk.FirstItem != itemId) ? itemCombineDesk.FirstItem : itemCombineDesk.SecondItem;

                ItemBlueprint second = Manager.Item.FindItemById(secondItemId);
                ItemBlueprint combine = Manager.Item.FindItemById(itemCombineList[i].CombineItem);
                ItemCombineSlot slot = itemCombineSlotList[i].GetComponent<ItemCombineSlot>();
                slot.InitSlot(second, combine);
            }
            else
            {
                itemCombineSlotList[i].SetActive(false);
            }
        }
    }


    #endregion

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }
}
