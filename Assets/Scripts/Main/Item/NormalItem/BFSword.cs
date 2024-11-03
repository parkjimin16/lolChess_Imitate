using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFSword : BaseItem
{
    private ItemAttribute adPowerItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.AD_Power)
            {
                adPowerItemAttribute = iAttribute;
            }
           
            iAttribute.InitItemAttributeValue();
        }
    }

    public override void ResetItem()
    {
        base.ResetItem();

        adPowerItemAttribute.InitItemAttributeValue();
    }
}
