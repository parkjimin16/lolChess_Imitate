using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianOath : BaseItem
{
    private bool isShieldDestroy;
    private bool hasShieldActivated;
    private int shieldAmount;
    private float shieldPercent;
    private ItemAttribute adDefItemAttribute = new ItemAttribute();
    private ItemAttribute apDefItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        shieldPercent = 0.25f;
        isShieldDestroy = false;
        hasShieldActivated = false;

        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.AD_Defense)
            {
                adDefItemAttribute = iAttribute;
            }
            else if (iAttribute.ItemAttributeType == ItemAttributeType.AP_Defense)
            {
                apDefItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }
    }

    public override void CheckHp(int curHp, int maxHp)
    {
        ActivateShield(curHp, maxHp);
    }

    private void ActivateShield(int currentHealth, int maxHealth)
    {
        float curHp = currentHealth;
        float maxHp = maxHealth;

        if (curHp / maxHp > 0.4f || hasShieldActivated)
        {
            Debug.Log("Return");
            return;
        }


        if (EquipChampionBase == null)
            return;


        if (!hasShieldActivated)
        {
            shieldAmount = (int)(EquipChampionBase.Champion_CurHp * shieldPercent);
            CoroutineHelper.StartCoroutine(ShieldCoroutine());
        }
    }

    public override void ResetItem()
    {
        shieldPercent = 0.25f;
        isShieldDestroy = false;
        hasShieldActivated = false;

        adDefItemAttribute.InitItemAttributeValue();
        apDefItemAttribute.InitItemAttributeValue();
    }

    private IEnumerator ShieldCoroutine()
    {
        hasShieldActivated = true;
        EquipChampionBase.SetShield(shieldAmount);
        Debug.Log("shield Start");

        yield return new WaitForSeconds(5);

        isShieldDestroy = true;
        adDefItemAttribute.SetAttributeValue(40);
        apDefItemAttribute.SetAttributeValue(20);
        Debug.Log("shield Done");
    }
}
