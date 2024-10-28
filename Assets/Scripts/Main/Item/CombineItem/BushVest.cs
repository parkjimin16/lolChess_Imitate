using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushVest : BaseItem
{
    private int originalMaxHp;
    private int tempMaxHp;
    private float defValue;

    private ItemAttribute hpItemAttribute = new ItemAttribute();
    private ItemAttribute totalDefItemAttribute = new ItemAttribute();


    public override void InitItemSkill()
    {
        defValue = 0.08f;

        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if(iAttribute.ItemAttributeType == ItemAttributeType.HP)
            {
                hpItemAttribute = iAttribute;
            }
            else if(iAttribute.ItemAttributeType == ItemAttributeType.TotalDefense)
            {
                totalDefItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }

        if (EquipChampionBase == null)
            return;


        tempMaxHp = (int)(EquipChampionBase.Display_MaxHp * 0.05f);
        hpItemAttribute.SetAttributeValue(tempMaxHp);
        totalDefItemAttribute.SetAttributeValue(defValue);
    }

    public override void ResetItem()
    {
        hpItemAttribute.InitItemAttributeValue();
        totalDefItemAttribute.InitItemAttributeValue();
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        
    }
}
