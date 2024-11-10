using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_InCompleteTranscendence : BaseAugmenter
{
    // 로직 변수
    private float total_Damage;
    private Coroutine delayedApplyCoroutine;

    #region 초기화

    private void InitValue()
    {
        total_Damage = 0.3f;
    }

    #endregion

    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        InitValue();
    }

    public override void ApplyStartRound(UserData user)
    {
        var list = GetUserChampions(user);
        delayedApplyCoroutine = CoroutineHelper.StartCoroutine(DelayedApplyStartRound(user, list));
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

    #region 불완전한 초월 로직
    private IEnumerator DelayedApplyStartRound(UserData user, List<ChampionBase> list)
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
