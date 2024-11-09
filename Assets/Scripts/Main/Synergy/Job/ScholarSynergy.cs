using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScholarSynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 학자 로직 변수
    private int mana;
    private float ap_Power;

    private Coroutine scholarCoroutine;

    public ScholarSynergy()
        : base("학자", ChampionLine.None, ChampionJob.Scholar, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {
            mana = 0;
            ap_Power = 0;

            Deactivate(user);
        }
        else if (level >= 2 && level < 4)
        {
            mana = 3;
            ap_Power = 10;
        }
        else if (level >= 4 && level < 6)
        {
            mana = 6;
            ap_Power = 20;
        }
        else if (level >= 6)
        {
            mana = 12;
            ap_Power = 30;
        }


        Debug.Log($"[학자] 레벨 {level} 효과 적용");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            cBase.InitSynergyStat();
            cBase.UpdateChampmionStat();
        }

        if (scholarCoroutine != null)
        {
            CoroutineHelper.StopCoroutine(scholarCoroutine);
            scholarCoroutine = null; 
        }

        Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        ScholarLogic(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region 학자 로직

    private void ScholarLogic(UserData user)
    {
        if (scholarCoroutine != null)
            CoroutineHelper.StopCoroutine(scholarCoroutine);

        var list = GetScholarChampionBase(user);

        foreach (var demo in list)
        {
            demo.Synergy_AP_Power += ap_Power;
            demo.UpdateChampmionStat();
            scholarCoroutine = CoroutineHelper.StartCoroutine(ScholarCoroutine(demo));
        }
    }

    private IEnumerator ScholarCoroutine(ChampionBase demo)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            demo.ChampionHpMpController.ManaPlus(mana);
        }
    }

    private List<ChampionBase> GetScholarChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionJob_First != ChampionJob.Scholar && cBase.ChampionJob_Second != ChampionJob.Scholar)
                continue;

            list.Add(cBase);
        }

        return list;
    }
    #endregion
}
