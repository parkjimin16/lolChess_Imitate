using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecurveBow : BaseItem
{
    private ItemAttribute adSpdItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.AD_Speed)
            {
                adSpdItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }
    }

    public override void ResetItem()
    {
        base.ResetItem();

        adSpdItemAttribute.InitItemAttributeValue();
    }
}
