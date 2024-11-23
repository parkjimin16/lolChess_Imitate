using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class ItemManager
{
    private ItemDataContainerBlueprint itemDataBase;
    private Dictionary<string, ItemBlueprint> itemDataDictionary;
    private Dictionary<ItemAttributeType, Sprite> attributeDictionary;

    private List<ItemBlueprint> totalItems; // 모든 조합 아이템
    private List<ItemBlueprint> normalItem;
    private List<ItemBlueprint> combineItem;
    private List<ItemBlueprint> usingItem;
    private List<ItemBlueprint> symbolItem;

    #region Properties
    public Dictionary<string, ItemBlueprint> ItemDataDictionary => itemDataDictionary;
    public Dictionary<ItemAttributeType, Sprite> AttributeDictionary => attributeDictionary;


    public List<ItemBlueprint> TotalItmes => totalItems;
    public List<ItemBlueprint> NormalItem => normalItem;
    public List<ItemBlueprint> CombineItem => combineItem;
    public List<ItemBlueprint> UsingItem => usingItem;
    public List<ItemBlueprint> SymbolItem => symbolItem;
    #endregion

    public Sprite GetIcon(ItemAttributeType iType)
    {
        return attributeDictionary.TryGetValue(iType, out Sprite sprite) ? sprite : null;
    }

    public ItemBlueprint FindItemById(string id)
    {
        itemDataDictionary.TryGetValue(id, out ItemBlueprint bluePrint);
        return bluePrint;
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

    public List<ItemCombineDesk> GetItemCombineDesk(string itemId)
    {
        return itemDataBase.GetMatchingItemCombines(itemId);
    }

    #region Init
    public void Init()
    {
        totalItems = new List<ItemBlueprint>();
        normalItem = new List<ItemBlueprint>();
        combineItem = new List<ItemBlueprint>();
        usingItem = new List<ItemBlueprint>();  
        symbolItem = new List<ItemBlueprint>();

        itemDataDictionary = new Dictionary<string, ItemBlueprint>();
        attributeDictionary = new Dictionary<ItemAttributeType, Sprite>();

        ParseItemData();
    }

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

        attributeDictionary = new Dictionary<ItemAttributeType, Sprite>
        {
            { ItemAttributeType.AD_Power, Resources.Load<Sprite>("Sprite/Stats Icon/ad_power") },
            { ItemAttributeType.AD_Speed, Resources.Load<Sprite>("Sprite/Stats Icon/atk_spd") },
            { ItemAttributeType.AD_Defense, Resources.Load<Sprite>("Sprite/Stats Icon/ad_def") },
            { ItemAttributeType.AP_Power, Resources.Load<Sprite>("Sprite/Stats Icon/ap_power") },
            { ItemAttributeType.AP_Defense, Resources.Load<Sprite>("Sprite/Stats Icon/ap_def") },
            { ItemAttributeType.HP, Resources.Load<Sprite>("Sprite/Stats Icon/HP") },
            { ItemAttributeType.Mana, Resources.Load<Sprite>("Sprite/Stats Icon/Mana") },
            { ItemAttributeType.CriticalPercent, Resources.Load<Sprite>("Sprite/Stats Icon/critical_percent") },
            { ItemAttributeType.BloodSuck, Resources.Load<Sprite>("Sprite/Stats Icon/blood_suck") },
            { ItemAttributeType.TotalPower, Resources.Load<Sprite>("Sprite/Stats Icon/total_power") },
            { ItemAttributeType.TotalDefense, Resources.Load<Sprite>("Sprite/Stats Icon/total_def") }
        };
    }

    #endregion

    #region ItemDataMethod

    
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
        
        GameObject itemObj = Manager.ObjectPool.GetGo("ItemFrame");
        itemObj.transform.position = pos;

        ItemFrame iFrame = itemObj.GetComponent<ItemFrame>();

        if (iFrame != null)
        {
            iFrame.Init(item);
        }

        

        return itemObj;
    }

    public void StartCreatingItems(UserData user, List<ItemBlueprint> itemList)
    {
        List<string> itemId = new List<string>();
        List<string> champ = new List<string>();
        for (int i = 0; i < itemList.Count; i++)
        {
            itemId.Add(itemList[i].ItemId);
        }

        GameObject itemObj = Manager.ObjectPool.GetGo("Capsule");
        Capsule cap = itemObj.GetComponent<Capsule>();
        itemObj.transform.position = new Vector3(0, 1, 0);

        if (cap != null)
        {
            cap.InitCapsule(user, 0, itemId, champ);
        }
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
