using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchangelStaff : BaseItem
{
    private bool coroutineStart;
    private ItemAttribute apPowerItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        coroutineStart = false;

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
        coroutineStart = false;
        apPowerItemAttribute.InitItemAttributeValue();
    }
    public override void InitTargetObject(GameObject target)
    {
        if (EquipChampionBase == null || coroutineStart)
            return;

        CoroutineHelper.StartCoroutine(ApPowerUpCoroutine(5f));
    }

    private IEnumerator ApPowerUpCoroutine(float value)
    {
        while (true)
        {
            coroutineStart = true;
            int apPower = (int)apPowerItemAttribute.GetAttributeValue();

            yield return new WaitForSeconds(value);

            apPower += 5;
            apPowerItemAttribute.SetAttributeValue(apPower);
        }
    }
}
