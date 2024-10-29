using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarmogArmor : BaseItem
{
    private int tempMaxHp;
    private ItemAttribute hpItemAttribute = new ItemAttribute();


    public override void InitItemSkill()
    {
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.HP)
            {
                hpItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }

        if (EquipChampionBase == null)
            return;

        tempMaxHp = (int)(hpItemAttribute.GetAttributeValue() + (EquipChampionBase.Champion_MaxHp * 0.12f));
        hpItemAttribute.SetAttributeValue(tempMaxHp);
    }

    public override void ResetItem()
    {
        hpItemAttribute.InitItemAttributeValue();
    }
}
