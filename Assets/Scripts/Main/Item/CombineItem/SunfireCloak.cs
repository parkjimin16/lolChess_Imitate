using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunfireCloak : BaseItem
{
    private HashSet<ChampionBase> affectedChampions = new HashSet<ChampionBase>();

    private Coroutine currentCoroutine;

    public override void InitItemSkill()
    {
        affectedChampions.Clear();
        currentCoroutine = null;
    }

    public override void ResetItem()
    {
        StopAllCoroutines();
        affectedChampions.Clear();
        currentCoroutine = null;
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        if (currentCoroutine != null)
        {
            CoroutineHelper.StopCoroutine(currentCoroutine);
        }

        if (EquipChampion == null || targetChampion == null || Manager.Stage.isCripRound)
            return;

        UserData targetUser = targetChampion.GetComponent<ChampionBase>().Player.UserData;

        List<GameObject> target = Manager.Stage.GetChampionsWithinOneTile(EquipChampion, Player.UserData);
        List<ChampionBase> targetChampionBase = new List<ChampionBase>();

        foreach (var obj in target)
        {
            ChampionBase cBase = obj.GetComponent<ChampionBase>();
            if (cBase != null && cBase.Player.UserData == targetUser)
            {
                targetChampionBase.Add(cBase);
            }
        }

        if (targetChampionBase.Count > 0)
        {
            affectedChampions.UnionWith(targetChampionBase);
            CoroutineHelper.StartCoroutine(ResetHealHpValueAfterDelay(targetChampionBase, 2.0f));
        }

        currentCoroutine = CoroutineHelper.StartCoroutine(ResetHealHpValueAfterDelay(targetChampionBase, 2.0f));
    }

    private IEnumerator ResetHealHpValueAfterDelay(List<ChampionBase> target, float delay)
    {
        for (int i = 0; i < target.Count; i++)
        {
            target[i].HealHpValue -= 0.33f;
        }

        yield return new WaitForSeconds(delay);

        for (int i = 0; i < target.Count; i++)
        {
            target[i].HealHpValue = 1.0f;
        }

        affectedChampions.ExceptWith(target);
    }
}
