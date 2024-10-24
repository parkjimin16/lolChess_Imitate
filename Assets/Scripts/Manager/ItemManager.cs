using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager
{
    private ItemDataContainerBlueprint itemDataBase;
    private Dictionary<string, ItemBlueprint> itemDataDictionary;

    private List<ItemBlueprint> totalItems; // 모든 조합 아이템
    private List<ItemBlueprint> normalItem;
    private List<ItemBlueprint> combineItem;
    private List<ItemBlueprint> usingItem;
    private List<ItemBlueprint> symbolItem;



    public List<ItemBlueprint> TotalItmes => totalItems;
    public List<ItemBlueprint> NormalItem => normalItem;
    public List<ItemBlueprint> CombineItem => combineItem;
    public List<ItemBlueprint> UsingItem => usingItem;
    public List<ItemBlueprint> SymbolItem => symbolItem;
    #region Properties

    public Dictionary<string, ItemBlueprint> ItemDataDictionary => itemDataDictionary;

    #endregion


    #region Init
    public void Init()
    {
        totalItems = new List<ItemBlueprint>();
        normalItem = new List<ItemBlueprint>();
        combineItem = new List<ItemBlueprint>();
        usingItem = new List<ItemBlueprint>();  
        symbolItem = new List<ItemBlueprint>();

        itemDataDictionary = new Dictionary<string, ItemBlueprint>();

        ParseItemData();
    }

    #endregion

    #region ItemDataMethod

    public void ParseItemData()
    {
        itemDataDictionary.Clear();

        itemDataBase = Manager.Asset.GetBlueprint("ItemDataContainer") as ItemDataContainerBlueprint;
        foreach (var itemData in itemDataBase.ItemDatas)
        {
            itemDataDictionary.Add(itemData.ItemId, itemData);

            totalItems.Add(itemData);
            normalItem = itemDataBase.FindItemType('A');
            combineItem = itemDataBase.FindItemType('B');
            symbolItem = itemDataBase.FindItemType('C');
        }
    }

    public ItemBlueprint FindItemById(string id)
    {
        itemDataDictionary.TryGetValue(id, out ItemBlueprint bluePrint);
        return bluePrint;
    }
    
    public string ItemCombine(string item1, string item2)
    {
        if (itemDataBase == null)
            return null;

        foreach(var desk in itemDataBase.ItemCombineDesk)
        {
            if ((desk.FirstItem == item1 && desk.SecondItem == item2) ||
              (desk.FirstItem == item2 && desk.SecondItem == item1))
            {
                return desk.CombineItem;
            }
        }

        // 조합 실패
        return "error";
    }

    public GameObject CreateItem(string itemId, Vector3 pos)
    {
        ItemBlueprint item = FindItemById(itemId);

        if(item == null)
        {
            Debug.Log("Find Null");
            return null;
        }

        item.InitBaseItem();
        
        GameObject itemObj = Manager.Asset.InstantiatePrefab("ItemFrame");
        itemObj.transform.position = pos;

        ItemFrame iFrame = itemObj.GetComponent<ItemFrame>();

        if (iFrame != null)
        {
            iFrame.Init(item);
        }

        return itemObj;
    }

    #endregion
}


[System.Serializable]
public class ItemAttribute
{
    public ItemAttributeType ItemAttributeType;
    public float AttributeValue;
}
