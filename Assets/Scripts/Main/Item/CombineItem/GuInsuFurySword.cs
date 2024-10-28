using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuInsuFurySword : BaseItem
{
    private float adSpdValue;
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
            adSpdValue = adSpdItemAttribute.GetAttributeValue();
        }
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        UpgradeItemStats();
    }

    public override void ResetItem()
    {
        adSpdValue = 0;
    }

    private void UpgradeItemStats()
    {
        adSpdValue *= 1.05f;

        adSpdItemAttribute.SetAttributeValue(adSpdValue);
    }
}
