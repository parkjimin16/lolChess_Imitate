using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelterSynergy : SynergyBase
{
    // �ó��� ����
    private int level;

    // ��ȣ���� ���� ����
    private float health_Heal;
    private float mana_Plus;

    private Coroutine recoveryCoroutine;

    public ShelterSynergy()
        : base("��ȣ����", ChampionLine.None, ChampionJob.Shelter, 0) { }

    #region Ȱ�� & ��Ȱ��ȭ

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

        Debug.Log($"[��ȣ����] ���� {level} ȿ�� ����");
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

        Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
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

    #region ��ȣ���� ����

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
