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

    public List<ItemCombineDesk> GetMatchingItemCombines(string itemId)
    {
        return itemCombineDesk.Where(desk =>
            desk.FirstItem == itemId || desk.SecondItem == itemId).ToList();
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

    [SerializeField] private ChampionLine championLine;
    [SerializeField] private ChampionJob championJob;

    #region 프로퍼티
    public Material Material => material;
    public Sprite Icon => icon;
    public string ItemId => itemId;
    public string ItemName => itemName;
    public string Description => description;
    public ItemType ItemType => itemType;
    public List<ItemAttribute> Attribute => itemAttribute;
    public BaseItem BaseItem => baseItem;
    public ChampionLine ChampionLine => championLine;
    public ChampionJob ChampionJob => championJob;

    #endregion

    public void InitBaseItem()
    {
        baseItem.Initialize(this);
    }

    public bool CompareLine(ChampionLine cLine)
    {
        if (championLine == ChampionLine.None)
            return false;

        return championLine == cLine;
    }

    public bool CompareJob(ChampionJob cJob)
    {
        if (championJob == ChampionJob.None)
            return false;

        return ChampionJob == cJob;
    }

    public void InitAttribute()
    {
        foreach(var attribute in itemAttribute)
        {
            attribute.InitItemAttributeValue();
        }
    }
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
