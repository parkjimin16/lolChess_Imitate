using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salvation : BaseItem
{
    private bool isCoroutineRunning;
    private ItemAttribute totalDefItemAttribute;
    public override void InitItemSkill()
    {
        isCoroutineRunning = false;

        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.TotalDefense)
            {
                totalDefItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }
        if(totalDefItemAttribute == null)
        {
            Debug.Log("구우ㅝㄴ오류");
        }
    }

    public override void ResetItem()
    {
        isCoroutineRunning = false;
        StopAllCoroutines();
        totalDefItemAttribute.InitItemAttributeValue();
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        if (isCoroutineRunning || EquipChampion == null || Manager.Stage.isCripRound)
            return;

        List<GameObject> target = Manager.Stage.GetChampionsWithinOneTile(EquipChampion, Player.UserData);
        List<ChampionBase> targetChampionBase = new List<ChampionBase>();

        foreach (var obj in target)
        {
            ChampionBase cBase = obj.GetComponent<ChampionBase>();

            if (cBase == null)
                return;

            targetChampionBase.Add(cBase);
        }
         
        CoroutineHelper.StartCoroutine(ResetHealHpValueAfterDelay(targetChampionBase, 5.0f));
    }

    private IEnumerator ResetHealHpValueAfterDelay(List<ChampionBase> target, float delay)
    {
        isCoroutineRunning = true;

        totalDefItemAttribute.SetAttributeValue(0.1f);

        List<float> initialHp = new List<float>();
        for (int i = 0; i < target.Count; i++)
        {
            initialHp.Add(target[i].Champion_CurHp);
        }

        yield return new WaitForSeconds(delay);

        totalDefItemAttribute.InitItemAttributeValue();
        for (int i = 0; i < target.Count; i++)
        {
            float lostHp = initialHp[i] - target[i].Champion_CurHp;
            int healAmount = (int)(lostHp * 0.15f);
            target[i].ChampionHpMpController.AddHealth(healAmount, 1.0f);
        }
        isCoroutineRunning = false;
    }
}
