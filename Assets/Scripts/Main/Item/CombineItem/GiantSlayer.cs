using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 거인 학살자
/// </summary>
public class GiantSlayer : BaseItem
{
    private ItemAttribute itemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        foreach(ItemAttribute iAttribute in ItemAttributes)
        {
            if(iAttribute.ItemAttributeType == ItemAttributeType.TotalPower)
            {
                itemAttribute = iAttribute;
                itemAttribute.AttributeValue = 0.25f;
            }
        }
    }

    public override void ResetItem()
    {
        itemAttribute.AttributeValue = 0;
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        if (targetChampion == null)
            return;

        InitItemSkill();

        ChampionBase targetChampionBase = targetChampion.GetComponent<ChampionBase>();

        if(targetChampionBase == null)
        {
            // 테스트용
            itemAttribute.AttributeValue = 0.25f;
            return;
        }

        GiantSlayerSkill(targetChampionBase);
    }

    private void GiantSlayerSkill(ChampionBase champion)
    {
        if (champion.MaxHP >= 1750)
        {
            itemAttribute.AttributeValue = 0.25f;
        }
        else
        {
            ResetItem();
        }
    }
}
