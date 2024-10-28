using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NashorTooth : BaseItem
{
    private float timer;
    private float tempAdSpdValue;
    private bool isStartCoroutine;

    private ItemAttribute adSpdItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        timer = 5.0f;
        isStartCoroutine = false;

        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.AD_Speed)
            {
                adSpdItemAttribute = iAttribute;
                tempAdSpdValue = adSpdItemAttribute.GetAttributeValue();
            }
        }
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        if (!EquipChampionBase.ChampionAttackController.IsUseSkill())
            return;

        if(!isStartCoroutine)
            CoroutineHelper.StartCoroutine(ShieldDurationCoroutine());
    }

    public override void ResetItem()
    {
        adSpdItemAttribute.InitItemAttributeValue();
    }

    private IEnumerator ShieldDurationCoroutine()
    {
        isStartCoroutine = true;
        tempAdSpdValue += 0.6f;
        adSpdItemAttribute.SetAttributeValue(tempAdSpdValue);

        yield return new WaitForSeconds(timer);

        adSpdItemAttribute.InitItemAttributeValue();
        tempAdSpdValue = 0;
        isStartCoroutine = false;
    }
}
