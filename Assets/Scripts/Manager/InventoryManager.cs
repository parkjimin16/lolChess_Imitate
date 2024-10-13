using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class InventoryManager
{
    #region ItemData Fields & Properties

    private string itemDataBaseText;
    private ItemDataContainerBlueprint itemDataBase;
    private Dictionary<string, ItemBlueprint> itemDataDictionary = new();

    public Dictionary<string, ItemBlueprint> ItemDataDictionary => itemDataDictionary;

    #endregion

    #region ItemDataMethod

    public void ParseItemData()
    {
        itemDataBase = Manager.Asset.GetBlueprint("ItemDataContainer") as ItemDataContainerBlueprint;
        foreach (var itemData in itemDataBase.ItemDatas)
        {
            itemDataDictionary.Add(itemData.ItemId, itemData);
        }
    }
    #endregion

    #region InventoryData Fields & Properties

    public InventoryData UserInventory { get; private set; }
    public List<UserItemData> WeaponItemList { get; private set; }
    public List<UserItemData> ArmorItemList { get; private set; }

    #endregion

    #region Inventory Data Methods

    public void Initialize()
    {
        //WeaponItemList = Manager.Data.Inventory.UserItemData.Where(ItemData => ItemData.itemID[0] == 'W').ToList();
        //ArmorItemList = Manager.Data.Inventory.UserItemData.Where(ItemData => ItemData.itemID[0] == 'A').ToList();
    }

    public UserItemData SearchItem(string itemID)
    {
        List<UserItemData> pickItem = UserInventory.UserItemData.Where(itemData => itemData.itemID == itemID).ToList();
        return pickItem[0];
    }

    #endregion

    #region Initialize Data Methods

    public void InitItem()
    {
        ParseItemData();
    }

    #endregion

    #region ItemData Control Method

    /// <summary>
    /// itemList로 전달받은 아이템 적용
    /// </summary>
    /// <param name="itemList"></param>
    public void ApplySelectTypeItem(List<UserItemData> itemList)
    {

    }

    #endregion
}

[System.Serializable]
public class InventoryData
{
    public List<UserItemData> UserItemData;
}

[System.Serializable]
public class UserItemData
{
    public string itemID;
    public int level;
    public bool equipped;

    public UserItemData(string ItemID, int Level, bool Equiped)
    {
        itemID = ItemID;
        level = Level;
        equipped = Equiped;
    }
}
