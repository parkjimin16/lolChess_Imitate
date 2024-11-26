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

            iAttribute.InitItemAttributeValue();
        }
    }

    public override void ResetItem()
    {
        count = 0;
        hpItemAttribute.InitItemAttributeValue();
        adPowerItemAttribute.InitItemAttributeValue();
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
            return;


        if (EquipChampionBase.Champion_CurHp / EquipChampionBase.Champion_MaxHp <= 0.6f)
        {
            hpItemAttribute.SetAttributeValue(300);
            adPowerItemAttribute.SetAttributeValue(0.35f);

            count++;
        }
    }


}
