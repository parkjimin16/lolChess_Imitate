using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownGuard : BaseItem
{
    private bool hasShieldActivated;
    private int shieldAmount;

    private float shieldPercent;

    private ItemAttribute apPowerItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        shieldPercent = 0.25f;
        hasShieldActivated = false;

        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.AP_Power)
            {
                apPowerItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }

    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        if (EquipChampionBase == null)
            return;


        if (!hasShieldActivated)
        {
            shieldAmount = (int)(EquipChampionBase.Champion_MaxHp * shieldPercent);
            CoroutineHelper.StartCoroutine(ShieldCoroutine());
        }
    }
    public override void ResetItem()
    {
        shieldPercent = 0.25f;
        hasShieldActivated = false;

        apPowerItemAttribute.InitItemAttributeValue();
    }

    private IEnumerator ShieldCoroutine()
    {
        hasShieldActivated = true;
        EquipChampionBase.SetShield(shieldAmount);

        yield return new WaitForSeconds(8);

        apPowerItemAttribute.SetAttributeValue(45);
    }
}
