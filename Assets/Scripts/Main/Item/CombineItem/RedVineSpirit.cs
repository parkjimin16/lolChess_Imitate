using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedVineSpirit : BaseItem
{
    private ItemAttribute totalPowerItemAttribute = new ItemAttribute();
    private bool isCoroutineRunning;

    public override void InitItemSkill()
    {
        isCoroutineRunning = false;

        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.TotalPower)
            {
                totalPowerItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }
    }

    public override void ResetItem()
    {
        totalPowerItemAttribute.InitItemAttributeValue();
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        if (targetChampion == null)
            return;

        ChampionBase target = targetChampion.GetComponent<ChampionBase>();


        if(target == null) 
            return;

        if (!isCoroutineRunning)
        {
            CoroutineHelper.StartCoroutine(ResetHealHpValueAfterDelay(target, 5.0f));
        }
    }


    private IEnumerator ResetHealHpValueAfterDelay(ChampionBase target, float delay)
    {
        isCoroutineRunning = true;
        target.HealHpValue -= 0.33f;

        yield return new WaitForSeconds(delay);

        target.HealHpValue = 1.0f;
        isCoroutineRunning = false;
    }
}
