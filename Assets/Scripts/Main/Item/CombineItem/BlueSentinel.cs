using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueSentinel : BaseItem
{
    private ItemAttribute totalPowerItemAttribute = new ItemAttribute();


    public override void InitItemSkill()
    {
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.TotalPower)
            {
                totalPowerItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }
    }

    public override void ResetItem()
    {
        totalPowerItemAttribute.InitItemAttributeValue();
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        if (EquipChampionBase == null)
            return;

        if (EquipChampionBase.Champion_MaxMana == 0)
            return;

        EquipChampionBase.Item_MaxMana = -10;
    }
}
