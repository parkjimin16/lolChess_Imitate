using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegatronCloak : BaseItem
{
    private ItemAttribute apDefItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.AP_Defense)
            {
                apDefItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }
    }

    public override void ResetItem()
    {
        base.ResetItem();

        apDefItemAttribute.InitItemAttributeValue();
    }
}
