using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morellonomicon : BaseItem
{
    private bool isCoroutineRunning;

    public override void InitItemSkill()
    {
        isCoroutineRunning = false;
    }

    public override void ResetItem()
    {
        isCoroutineRunning = false;
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        if (targetChampion == null)
            return;

        ChampionBase target = targetChampion.GetComponent<ChampionBase>();


        if (target == null)
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
