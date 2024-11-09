using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmogrifierSynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 형상변환자 로직 변수
    private float total_Hp;
    private float health_Heal;

    private Coroutine healCoroutine;
    private Coroutine healthCoroutine;

    public TransmogrifierSynergy()
        : base("형상변환자", ChampionLine.None, ChampionJob.Transmogrifier, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {
            total_Hp = 0;
            health_Heal = 0;

            Deactivate(user);
        }
        else if (level >= 2 && level < 4)
        {
            total_Hp = 0.1f;
            health_Heal = 0;
        }
        else if (level >= 4 && level < 6)
        {
            total_Hp = 0.18f;
            health_Heal = 0;
        }
        else if (level >= 6 && level < 8)
        {
            total_Hp = 0.27f;
            health_Heal = 0;
        }
        else if (level >= 8)
        {
            total_Hp = 0.33f;
            health_Heal = 0.03f;
        }


        Debug.Log($"[형상변환자] 레벨 {level} 효과 적용");
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


        if (healCoroutine != null)
        {
            CoroutineHelper.StopCoroutine(healCoroutine);
            healCoroutine = null;
        }

        if(healthCoroutine != null)
        {
            CoroutineHelper.StopCoroutine(healthCoroutine);
            healthCoroutine = null;
        }

        Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        TransLogic(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region 형상변환자 로직

    private void TransLogic(UserData user)
    {
        var list = GetTransmogrifierChampionBase(user);

        foreach (var cBase in list)
        {
            float hp = cBase.Champion_MaxHp * total_Hp;
            cBase.Synergy_MaxHP += (int)hp;
            cBase.UpdateChampmionStat();
            cBase.ChampionHpMpController.AddHealth((int)hp, 1.0f);
            cBase.UpdateChampmionStat();

            healthCoroutine = CoroutineHelper.StartCoroutine(HpUpCoroutine(cBase));
            healCoroutine = CoroutineHelper.StartCoroutine(HealCoroutine(cBase));
        }
    }
    private IEnumerator HealCoroutine(ChampionBase cBase)
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            float hp = cBase.Champion_MaxHp * health_Heal;
            cBase.ChampionHpMpController.AddHealth((int)hp, 1.0f);
            cBase.UpdateChampmionStat();
        }    
    }

    private IEnumerator HpUpCoroutine(ChampionBase cBase)
    {
        yield return new WaitForSeconds(5.0f);

        float hp = cBase.Champion_MaxHp * total_Hp;
        cBase.Synergy_MaxHP += (int)hp * 2;
        cBase.UpdateChampmionStat();
        cBase.ChampionHpMpController.AddHealth((int)hp * 2, 1.0f);
        cBase.UpdateChampmionStat();
    }
    private List<ChampionBase> GetTransmogrifierChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionJob_First != ChampionJob.Transmogrifier && cBase.ChampionJob_Second != ChampionJob.Transmogrifier)
                continue;

            list.Add(cBase);
        }

        return list;
    }
    #endregion
}
