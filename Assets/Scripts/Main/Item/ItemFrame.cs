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

    public void ShowItemInfoUI()
    {
        // UI �������� �����Ͽ� ������ ������ ǥ���ϴ� ������ ����
        Debug.Log($"������ �̸�: {itemBlueprint.ItemName}"); 
    }

    public void TryCombineItem(ItemFrame targetItemFrame)
    {
        string combinedItemName = Manager.Item.ItemCombine(itemBlueprint.ItemId, targetItemFrame.ItemBlueprint.ItemId);

        if (combinedItemName == "error")
        {
            Debug.Log("������ ���� ����");
            return;
        }

        // ���� ���� �� ������ ����
        Manager.Item.CreateItem(combinedItemName, new Vector3(0, 0, 0));

        // ���� ������ �ı�
        Destroy(targetItemFrame.gameObject);
        Destroy(gameObject);
    }
}
