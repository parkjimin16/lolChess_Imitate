using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongHeart : BaseItem
{
    private float totalDefMin;
    private bool isHpUnderHalf;

    private ItemAttribute totalDefItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        totalDefMin = 0.08f;
        isHpUnderHalf = false;

        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if(iAttribute.ItemAttributeType == ItemAttributeType.TotalDefense)
            {
                totalDefItemAttribute = iAttribute;
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

        if (curHp / maxHp > 0.5f)
        {
            Debug.Log("Return");
            return;
        }

        isHpUnderHalf = true;
        if (EquipChampionBase == null)
            return;

        if (isHpUnderHalf)
        {
            totalDefItemAttribute.SetAttributeValue(totalDefMin);
            isHpUnderHalf = false;
        }
    }

    public override void ResetItem()
    {
        isHpUnderHalf = false;
        totalDefMin = 0.08f;

        totalDefItemAttribute.InitItemAttributeValue();
    }
}
