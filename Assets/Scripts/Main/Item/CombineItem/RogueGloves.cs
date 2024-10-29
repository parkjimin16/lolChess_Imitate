using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class RogueGloves : BaseItem
{
     private List<ItemBlueprint> equippedItems = new List<ItemBlueprint>();
    public override void InitItemSkill()
    {
        RandomItem();
    }

    public override void ResetItem()
    {
        RemoveEquippedItems();
    }
    
    private void RandomItem()
    {
        if (EquipChampionBase == null)
            return;

        equippedItems.Clear();

        for (int i= 0;i < 2; i++)
        {
            string itemId = Manager.Item.CombineItem[Random.Range(0, Manager.Item.CombineItem.Count)].ItemId;
            ItemBlueprint newItem;
            if (Manager.Item.ItemDataDictionary.TryGetValue(itemId, out newItem))
            {
                EquipChampionBase.GetItem(newItem);
                equippedItems.Add(newItem);
            }
            else
            {
                Debug.LogWarning("아이테 아이디에 맞는 아이템이 없음");
                return;
            }
        }
    }

    private void RemoveEquippedItems()
    {
        EquipChampionBase.EquipItem.Clear();
        equippedItems.Clear();

        ItemBlueprint newItem;
        if (Manager.Item.ItemDataDictionary.TryGetValue("B036", out newItem))
        {
            EquipChampionBase.GetItem(newItem);
            equippedItems.Add(newItem);
        }
        EquipChampionBase.GetItem(newItem);
    }
}
