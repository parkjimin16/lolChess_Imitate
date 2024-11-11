using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_FinalTranscendence : BaseAugmenter
{
    // ���� ����
    private float total_Damage_First;
    private float total_Damage_Second;
    private Coroutine delayedApplyCoroutine;

    #region �ʱ�ȭ

    private void InitValue()
    {
        total_Damage_First = 0.2f;
        total_Damage_Second = 0.5f;
    }

    #endregion
    #region ����ü ����
    public override void ApplyNow(UserData user)
    {
        InitValue();
    }

    public override void ApplyStartRound(UserData user)
    {
        var list = GetUserChampions(user);

        foreach (var champion in list)
        {
            champion.Augmenter_Power_Upgrade += total_Damage_First;
            champion.UpdateChampmionStat();
        }

        delayedApplyCoroutine = CoroutineHelper.StartCoroutine(DelayedApplyStartRound(list));
    }

    public override void ApplyEndRound(UserData user)
    {
        InitValue();

        var list = GetUserChampions(user);

        if (delayedApplyCoroutine != null)
        {
            CoroutineHelper.StopCoroutine(delayedApplyCoroutine);
        }

        foreach (var cBase in list)
        {
            cBase.InitAugmenterStat();
            cBase.UpdateChampmionStat();
        }
    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion

    #region �ʿ� ����
    private IEnumerator DelayedApplyStartRound(List<ChampionBase> list)
    {
        yield return new WaitForSeconds(15f);

        foreach (var cBase in list)
        {
            cBase.Augmenter_Power_Upgrade += total_Damage_Second;
            cBase.UpdateChampmionStat();
        }
    }
    #endregion
}
