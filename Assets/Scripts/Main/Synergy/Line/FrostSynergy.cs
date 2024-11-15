using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrostSynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 서리 로직 변수
    private List<GameObject> enemyChampion;
    private List<ChampionBase> enemyChampionBase;

    private float ap_Power;
    private float ad_Power;
    private int count;
    private bool isActive;
    public FrostSynergy()
        : base("서리", ChampionLine.Frost, ChampionJob.None, 0) { }


    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 3)
        {
            enemyChampion = new List<GameObject>();
            enemyChampionBase = new List<ChampionBase>();


            ad_Power = 0;
            ap_Power = 0;
            count = 0;
            isActive = false;

            Deactivate(user);
            return;
        }
        else if (level >= 3 && level < 5)
        {
            ad_Power = 0.16f;
            ap_Power = 0.16f;
            count = 1;

        }
        else if (level >= 5 && level < 7)
        {
            ad_Power = 0.4f;
            ap_Power = 0.4f;
            count = 2;
        }
        else if (level >= 7 && level < 9)
        {
            ad_Power = 0.55f;
            ap_Power = 0.55f;
            count = 3;
        }
        else if (level >= 9)
        {
            ad_Power = 0.9f;
            ap_Power = 0.9f;
            count = 4;
        }

        //Debug.Log($"[서리] 레벨 {level} 적용");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.Frost && cBase.ChampionLine_Second != ChampionLine.Frost)
                continue;


            cBase.Synergy_AD_Power -= ad_Power;
            cBase.Synergy_AP_Power -= ad_Power;

            cBase.UpdateChampmionStat();
        }

        ad_Power = 0;
        ad_Power = 0;
        isActive = false;

        //Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 3)
            return;

        isActive = true;
        SetFrostChampion(user);
        CoroutineHelper.StartCoroutine(CheckAndSpawnFrostWarrior());
    }

    public override void Deactivate(UserData user)
    {
        isActive = false;
        RemoveEffects(user);
    }

    #endregion


    #region 서리 로직

    private void SetFrostChampion(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.Frost && cBase.ChampionLine_Second != ChampionLine.Frost)
                continue;

            cBase.Synergy_AD_Power += ad_Power;
            cBase.Synergy_AP_Power += ap_Power;

            cBase.UpdateChampmionStat();
        }

        enemyChampion = GetEnemyChampion(user);
        enemyChampionBase = GetEnemyChampionBase(user);
    }

    private List<GameObject> GetEnemyChampion(UserData user)
    {
        return user.BattleChampionObject;
    }

    private List<ChampionBase> GetEnemyChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach(var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if(cBase != null)
                list.Add(cBase);
        }

        return list;
    }

    public int GetFirstAliveEnemyIndex(List<ChampionBase> enemyChampionBase)
    {
        return enemyChampionBase.FindIndex(champion => !champion.ChampionHpMpController.IsDie());
    }

    private IEnumerator CheckAndSpawnFrostWarrior()
    {
        List<ChampionBase> spawnedEnemies = new List<ChampionBase>();

        while (isActive && count > 0)
        {
            if (isActive && enemyChampion.Count > 0 && enemyChampion.Count == enemyChampionBase.Count)
            {
                for (int i = 0; i < enemyChampionBase.Count; i++)
                {
                    if (enemyChampionBase[i].ChampionHpMpController.IsDie() && !spawnedEnemies.Contains(enemyChampionBase[i]))
                    {
                        GameObject obj = Manager.Asset.InstantiatePrefab("FrostWarrior");
                        obj.transform.position = enemyChampion[i].transform.position;

                        FrostWarrior fWarrior = obj.GetComponent<FrostWarrior>();
                        int index = GetFirstAliveEnemyIndex(enemyChampionBase);

                        if (index != -1)
                        {
                            fWarrior.Init(enemyChampionBase[i].Champion_MaxHp, enemyChampionBase[i].Champion_AD_Power, enemyChampion[index].gameObject);
                        }

                        spawnedEnemies.Add(enemyChampionBase[i]);
                        count--;

                        if (count <= 0)
                            break;
                    }
                }
            }

            yield return new WaitForSeconds(0.25f); 
        }
    }

    #endregion
}
