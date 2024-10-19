using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ItemFrame : MonoBehaviour
{
    [SerializeField] private Image itemImge;
    [SerializeField] private TextMeshPro txt_ItemName;
    [SerializeField] private ItemBlueprint itemBlueprint;

    #region Property
    public Image ItemImage => itemImge;
    public TextMeshPro Txt_ItemName => txt_ItemName;
    public ItemBlueprint ItemBlueprint => itemBlueprint;
    #endregion


    public void Init(ItemBlueprint item)
    {
        if(item == null)
        {
            Debug.Log("item Null");
        }

        itemImge = item.Icon;
        txt_ItemName.text = item.ItemName;
        itemBlueprint = item;
    }

    #region UI
    public void ShowItemInfoUI()
    {
        var itemInfoUIPopup = Manager.UI.ShowPopup<UIPopupItemDetail>();

        Vector2 mousePosition = Input.mousePosition;
        itemInfoUIPopup.SetPosition(mousePosition);

        UIPopupItemDetail uiItemDetail = itemInfoUIPopup.GetComponent<UIPopupItemDetail>();
        uiItemDetail.SetItemData(itemBlueprint);
    }

    #endregion

    #region Combine
    public void TryCombineItem(ItemFrame targetItemFrame)
    {
        string combinedItemName = Manager.Item.ItemCombine(itemBlueprint.ItemId, targetItemFrame.ItemBlueprint.ItemId);

        if (combinedItemName == "error")
        {
            Debug.Log("아이템 조합 실패");
            return;
        }

        // 조합 성공 시 아이템 생성
        Manager.Item.CreateItem(combinedItemName, new Vector3(0, 0, 0));

        // 기존 아이템 파괴
        Destroy(targetItemFrame.gameObject);
        Destroy(gameObject);
    }
    #endregion
}
