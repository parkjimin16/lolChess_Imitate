using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandOfJustice : BaseItem
{
    private ItemAttribute adPowerItemAttribute = new ItemAttribute();
    private ItemAttribute apPowerItemAttribute = new ItemAttribute();
    private ItemAttribute bloodSuckItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.AD_Power)
            {
                adPowerItemAttribute = iAttribute;
            }
            else if (iAttribute.ItemAttributeType == ItemAttributeType.AP_Power)
            {
                apPowerItemAttribute = iAttribute;
            }
            else if (iAttribute.ItemAttributeType == ItemAttributeType.BloodSuck)
            {
                bloodSuckItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }

        ApplyRandomAttributeBoost();
    }

    public override void ResetItem()
    {
        adPowerItemAttribute.InitItemAttributeValue();
        apPowerItemAttribute.InitItemAttributeValue();
        bloodSuckItemAttribute.InitItemAttributeValue();
    }

    private void ApplyRandomAttributeBoost()
    {
        int randomAttribute = Random.Range(0, 2);

        switch (randomAttribute)
        {
            case 0:
                adPowerItemAttribute.SetAttributeValue(adPowerItemAttribute.GetAttributeValue() * 2);
                apPowerItemAttribute.SetAttributeValue(apPowerItemAttribute.GetAttributeValue() * 2);
                break;
            case 1:
                bloodSuckItemAttribute.SetAttributeValue(bloodSuckItemAttribute.GetAttributeValue() * 2);
                break;
        }
    }
}
