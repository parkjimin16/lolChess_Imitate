using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EveningArmor : BaseItem
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
        if (isCoroutineRunning || EquipChampion == null)
            return;

        List<GameObject> target = Manager.Stage.GetChampionsWithinOneTile(EquipChampion);
        List<ChampionBase> targetChampionBase = new List<ChampionBase>();

        foreach (var obj in target)
        {
            ChampionBase cBase = obj.GetComponent<ChampionBase>();

            if (cBase == null)
                return;

            targetChampionBase.Add(cBase);
        }

        CoroutineHelper.StartCoroutine(ResetHealHpValueAfterDelay(targetChampionBase, 2.0f));
    }

    private IEnumerator ResetHealHpValueAfterDelay(List<ChampionBase> target, float delay)
    {
        isCoroutineRunning = true;

        for (int i = 0; i < target.Count; i++)
        {
            target[i].Champion_AD_Def -= 0.3f;
        }

        yield return new WaitForSeconds(delay);

        for (int i = 0; i < target.Count; i++)
        {
            target[i].Champion_AD_Def += 0.3f;
        }
        isCoroutineRunning = false;
    }
}
