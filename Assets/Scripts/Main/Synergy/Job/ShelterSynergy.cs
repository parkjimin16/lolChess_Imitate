using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelterSynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 보호술사 로직 변수
    private float health_Heal;
    private float mana_Plus;

    private Coroutine recoveryCoroutine;

    public ShelterSynergy()
        : base("보호술사", ChampionLine.None, ChampionJob.Shelter, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {
            health_Heal = 0;
            mana_Plus = 0;

            Deactivate(user);
        }
        else if (level == 2 )
        {
            health_Heal = 0.02f;
            mana_Plus = 0.03f;

        }
        else if (level == 3)
        {
            health_Heal = 0.04f;
            mana_Plus = 0.05f;
        }
        else if (level == 4)
        {
            health_Heal = 0.06f;
            mana_Plus = 0.07f;
        }
        else if (level >= 5)
        {
            health_Heal = 0.09f;
            mana_Plus = 0.11f;
        }

        Debug.Log($"[보호술사] 레벨 {level} 효과 적용");
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


        if (recoveryCoroutine != null)
        {
            CoroutineHelper.StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }

        Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        ShelterLogic(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region 보호술사 로직

    private void ShelterLogic(UserData user)
    {
        if (recoveryCoroutine != null)
            CoroutineHelper.StopCoroutine(recoveryCoroutine);

        var list = GetTeamChampionBase(user);

        recoveryCoroutine = CoroutineHelper.StartCoroutine(RecoverCoroutine(list));
    }

    private IEnumerator RecoverCoroutine(List<ChampionBase> list)
    {
        while (true)
        {
            foreach (var cBase in list)
            {
                if (cBase == null) continue;

                bool isShelter = (cBase.ChampionJob_First == ChampionJob.Shelter || cBase.ChampionJob_Second == ChampionJob.Shelter);
                float multiplier = isShelter ? 2f : 1f;

                if (cBase.Champion_CurHp < cBase.Champion_MaxHp)
                {
                    float healthRecovery = cBase.Champion_MaxHp * health_Heal * multiplier;
                    cBase.ChampionHpMpController.AddHealth((int)healthRecovery, 1.0f);
                }
                else
                {
                    float manaRecovery = cBase.Champion_MaxMana * mana_Plus * multiplier;
                    cBase.ChampionHpMpController.ManaPlus((int)manaRecovery);
                }

                cBase.UpdateChampmionStat();
            }

            yield return new WaitForSeconds(3f); 
        }
    }
    private List<ChampionBase> GetTeamChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase != null)
                list.Add(cBase);
        }

        return list;
    }
    #endregion
}
