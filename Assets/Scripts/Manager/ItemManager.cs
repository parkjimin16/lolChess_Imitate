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
            itemData.InitAttribute();
        }

        normalItem = itemDataBase.FindItemType('A');
        combineItem = itemDataBase.FindItemType('B');
        symbolItem = itemDataBase.FindItemType('C');
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

    public void StartCreatingItems(List<ItemBlueprint> itemList, Vector3 startPosition)
    {
        List<ItemBlueprint> temp = itemList;
        for (int i = 0; i < temp.Count; i++)
        {
            ItemBlueprint item = temp[i];

            if (item != null)
            {
                item.InitBaseItem();

                GameObject itemObj = Manager.Asset.InstantiatePrefab("ItemFrame");
                itemObj.transform.position = startPosition;

                ItemFrame iFrame = itemObj.GetComponent<ItemFrame>();

                if (iFrame != null)
                {
                    iFrame.Init(item);
                }

                startPosition += new Vector3(0, 0, 1);

            }
            else
            {
                Debug.Log("Item is null");
            }
        }
    }

    public ItemBlueprint GetItemBlueprint(List<ItemBlueprint> list)
    {
        if (list != null && list.Count > 0)
        {
            int randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }
        else
            return null; 
    }

    #endregion
}


[System.Serializable]
public class ItemAttribute
{
    public ItemAttributeType ItemAttributeType;
    public float AttributeValue;

    private float tempValue;

    public float GetAttributeValue()
    {
        return tempValue;
    }

    public void SetAttributeValue(float value)
    {
        tempValue = value;
    }

    public void InitItemAttributeValue()
    {
        tempValue = AttributeValue;
    }

    [System.Serializable]
    public class ItemOriginalState
    {
        public Vector3 originalPosition;
        public Transform originalParent;
        public ItemTile originalItemTile;
        public int originalTileIndex; // 아이템이 위치한 타일의 인덱스
        public bool wasActive;
    }
}
