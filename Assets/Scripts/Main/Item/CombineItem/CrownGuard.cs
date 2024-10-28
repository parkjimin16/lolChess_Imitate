using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownGuard : BaseItem
{
    
    private bool isShieldDestroy;
    private bool hasShieldActivated;
    private int shieldAmount;

    private float shieldPercent;

    private ItemAttribute apPowerItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        shieldPercent = 0.25f;
        isShieldDestroy = false;
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
            shieldAmount = (int)(EquipChampionBase.Display_MaxHp * shieldPercent);
            CoroutineHelper.StartCoroutine(ShieldCoroutine());
        }
    }
    public override void ResetItem()
    {
        shieldPercent = 0.25f;
        isShieldDestroy = false;
        hasShieldActivated = false;

        apPowerItemAttribute.InitItemAttributeValue();
    }

    private IEnumerator ShieldCoroutine()
    {
        hasShieldActivated = true;
        EquipChampionBase.SetShield(shieldAmount);
        Debug.Log("shield Start");

        yield return new WaitForSeconds(8);

        isShieldDestroy = true;
        apPowerItemAttribute.SetAttributeValue(45);
        Debug.Log("shield Done");
    }
}
