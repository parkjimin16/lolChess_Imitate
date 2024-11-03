using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantBelt : BaseItem
{
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
    }

    public override void ResetItem()
    {
        base.ResetItem();

        hpItemAttribute.InitItemAttributeValue();
    }
}
