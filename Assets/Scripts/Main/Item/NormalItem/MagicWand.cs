using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWand : BaseItem
{
    private ItemAttribute apPowerItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.AP_Power)
            {
                apPowerItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }
    }

    public override void ResetItem()
    {
        base.ResetItem();

        apPowerItemAttribute.InitItemAttributeValue();
    }
}
