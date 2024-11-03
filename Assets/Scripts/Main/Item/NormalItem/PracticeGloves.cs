using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeGloves : BaseItem
{
    private ItemAttribute criticalPercentItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.CriticalPercent)
            {
                criticalPercentItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }
    }

    public override void ResetItem()
    {
        base.ResetItem();

        criticalPercentItemAttribute.InitItemAttributeValue();
    }
}
    