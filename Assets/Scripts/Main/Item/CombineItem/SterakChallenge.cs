using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SterakChallenge : BaseItem
{
    private int count;
    private ItemAttribute adPowerItemAttribute = new ItemAttribute();
    private ItemAttribute hpItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        count = 0;
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.HP)
            {
                hpItemAttribute = iAttribute;
            }
            else if(iAttribute.ItemAttributeType == ItemAttributeType.AD_Power)
            {
                adPowerItemAttribute = iAttribute;
            }
        }
    }

    public override void ResetItem()
    {
        Debug.Log("Reset Item");
        count = 0;
        hpItemAttribute.AttributeValue = 150;
        adPowerItemAttribute.AttributeValue = 0.15f;
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        if (count > 0)
            return;

        CheckChampionHp();
    }

    private void CheckChampionHp()
    {
        if (EquipChampion == null || EquipChampionBase == null)
        {
            return;
        }


        if (EquipChampionBase.Display_CurHp / EquipChampionBase.Display_MaxHp <= 0.6f)
        {
            float hp = 300;
            float ad = 0.35f;

            hpItemAttribute.AttributeValue = hp;
            adPowerItemAttribute.AttributeValue = ad;

            count++;
        }
    }
}
