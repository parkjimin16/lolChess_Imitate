using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainVest : BaseItem
{
    private ItemAttribute adDefItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.AD_Defense)
            {
                adDefItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }
    }

    public override void ResetItem()
    {
        base.ResetItem();

        adDefItemAttribute.InitItemAttributeValue();
    }
}
