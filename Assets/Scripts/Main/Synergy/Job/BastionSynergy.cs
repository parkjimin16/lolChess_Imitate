using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BastionSynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 요새 로직 변수
    private float ad_Def;
    private float ap_Def;

    private Coroutine adDefCoroutine;

    private System.Random random = new System.Random();

    public BastionSynergy()
        : base("요새", ChampionLine.None, ChampionJob.Bastion, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {
            ad_Def = 0;
            ap_Def = 0;

            Deactivate(user);
        }
        else if (level >= 2 && level < 4)
        {
            ad_Def = 15;
            ap_Def = 15;
        }
        else if (level >= 4 && level < 6)
        {
            ad_Def = 40;
            ap_Def = 40;
        }
        else if (level >= 6 && level < 8)
        {
            ad_Def = 100;
            ap_Def = 100;
        }
        else if (level >= 8)
        {
            ad_Def = 250;
            ap_Def = 250;
        }


        //Debug.Log($"[요새] 레벨 {level} 효과 적용");
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
        
        if (adDefCoroutine != null)
        {
            CoroutineHelper.StopCoroutine(adDefCoroutine);
            adDefCoroutine = null;
        }

        //Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        BastionLogic(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region 요새 로직

    private void BastionLogic(UserData user)
    {
        if (adDefCoroutine != null)
            CoroutineHelper.StopCoroutine(adDefCoroutine);

        var list = GetTeamChampionBase(user);

        foreach (var cBase in list)
        {
            adDefCoroutine = CoroutineHelper.StartCoroutine(AttackCoroutine(cBase));
        }
    }
    private IEnumerator AttackCoroutine(ChampionBase rusher)
    {
        if (rusher.ChampionJob_First != ChampionJob.Bastion && rusher.ChampionJob_Second != ChampionJob.Bastion)
        {
            rusher.Synergy_AD_Def = ad_Def;
            rusher.Synergy_AP_Def += ap_Def;

            rusher.UpdateChampmionStat();
            yield break;
        }

        rusher.Synergy_AD_Def += ad_Def * 2;
        rusher.Synergy_AP_Def += ap_Def * 2;

        rusher.UpdateChampmionStat();

        yield return new WaitForSeconds(10f);

        rusher.Synergy_AD_Def -= ad_Def;
        rusher.Synergy_AP_Def -= ap_Def;

        rusher.UpdateChampmionStat();
    }

    private List<ChampionBase> GetTeamChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            list.Add(cBase);
        }

        return list;
    }
    #endregion
}
