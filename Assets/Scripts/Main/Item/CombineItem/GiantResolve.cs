using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantResolve : BaseItem
{
    private int count;

    private float adPower, apPower, adDef, apDef;
    private ItemAttribute adDefItemAttribute = new ItemAttribute();
    private ItemAttribute apDefItemAttribute = new ItemAttribute();

    private ItemAttribute adPowerItemAttribute = new ItemAttribute();
    private ItemAttribute apPowerItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        count = 0;

        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.AP_Defense)
            {
                apDefItemAttribute = iAttribute;
            }
            else if (iAttribute.ItemAttributeType == ItemAttributeType.AD_Defense)
            {
                adDefItemAttribute = iAttribute;
            }
            else if(iAttribute.ItemAttributeType == ItemAttributeType.AD_Power)
            {
                adPowerItemAttribute = iAttribute;
            }
            else if(iAttribute.ItemAttributeType == ItemAttributeType.AP_Power)
            {
                apPowerItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();

            adPower = adPowerItemAttribute.GetAttributeValue();
            apPower = apPowerItemAttribute.GetAttributeValue();
            adDef = adDefItemAttribute.GetAttributeValue();
            apDef = apDefItemAttribute.GetAttributeValue();
        }
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        if (count > 25)
            return;

        PlusItemStats();
    }

    public override void ResetItem()
    {
        count = 0;

        adPower = 0;
        apPower = 0;
        adDef = 0;
        apDef = 0;

        adPowerItemAttribute.InitItemAttributeValue();
        apPowerItemAttribute.InitItemAttributeValue();
        adDefItemAttribute.InitItemAttributeValue();
        apDefItemAttribute.InitItemAttributeValue();
    }

    private void PlusItemStats()
    {
        if(count == 25)
        {
            adDef += 20;
            apDef += 20;
        }

        adPower += 2;
        apPower += 2;


        adPowerItemAttribute.SetAttributeValue(adPower);
        apPowerItemAttribute.SetAttributeValue(apPower);
        adDefItemAttribute.SetAttributeValue(adDef);
        apDefItemAttribute.SetAttributeValue(apDef);

        count++;
    }
}
