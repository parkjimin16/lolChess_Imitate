using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_InCompleteTranscendence : BaseAugmenter
{
    // ���� ����
    private float total_Damage;
    private Coroutine delayedApplyCoroutine;

    #region �ʱ�ȭ

    private void InitValue()
    {
        total_Damage = 0.3f;
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

    #region �ҿ����� �ʿ� ����
    private IEnumerator DelayedApplyStartRound(List<ChampionBase> list)
    {
        yield return new WaitForSeconds(15f);

        foreach (var cBase in list)
        {
            cBase.Augmenter_Power_Upgrade += total_Damage;
            cBase.UpdateChampmionStat();
        }
    }
    #endregion
}
