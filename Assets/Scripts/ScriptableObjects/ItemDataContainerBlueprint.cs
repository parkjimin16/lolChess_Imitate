using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemDataContainer", menuName = "Blueprints/ItemDataContainer")]
public class ItemDataContainerBlueprint : ScriptableObject
{
    [SerializeField] private List<ItemBlueprint> itemDatas;
    [SerializeField] private List<ItemCombineDesk> itemCombineDesk;
    public List<ItemBlueprint> ItemDatas => itemDatas;
    public List<ItemCombineDesk> ItemCombineDesk => itemCombineDesk;

    public List<ItemBlueprint> FindItemType(ItemType type)
    {
        return itemDatas.Where(item => item.ItemType == type).ToList();
    }

    public List<ItemBlueprint> FindItemType(char id)
    {
        return ItemDatas.Where(item => item.ItemId[0] == id).ToList();
    }

    public string FindCombineItem(string item1, string item2)
    {
        foreach (var desk in itemCombineDesk)
        {
            if ((desk.FirstItem == item1 && desk.SecondItem == item2) ||
                (desk.FirstItem == item2 && desk.SecondItem == item1))
            {
                return desk.CombineItem;
            }
        }

        return null;
    }
}


[System.Serializable]
public class ItemBlueprint
{
    [Header("Item Info")]
    [SerializeField] private Material material;
    [SerializeField] private Sprite icon;
    [SerializeField] private string itemId;
    [SerializeField] private string itemName;
    [SerializeField, TextArea] private string description;
    [SerializeField] private ItemType itemType;
    [SerializeField] private List<ItemAttribute> itemAttribute;
    [SerializeField] private BaseItem baseItem;

    public Material Material => material;
    public Sprite Icon => icon;
    public string ItemId => itemId;
    public string ItemName => itemName;
    public string Description => description;
    public ItemType ItemType => itemType;
    public List<ItemAttribute> Attribute => itemAttribute;
    public BaseItem BaseItem => baseItem;
}


[System.Serializable]
public class ItemCombineDesk
{
    [SerializeField] private string firstItem;
    [SerializeField] private string secondItem;
    [SerializeField] private string combineItem;

    public string FirstItem => firstItem;
    public string SecondItem => secondItem;
    public string CombineItem => combineItem;
}
