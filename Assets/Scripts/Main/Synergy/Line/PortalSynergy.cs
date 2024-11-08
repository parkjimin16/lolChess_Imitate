using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 차원문 로직 변수
    private int portalChampionLevel;
    private int portalGateLevel;
    private int maxHp;
    private Transform portalObjPosition;
    private GameObject portalObj;
    private PortalGate portalGate;

    private List<GameObject> enemyChampion;
    private List<ChampionBase> enemyChampionBase;

    public PortalSynergy()
        : base("차원문", ChampionLine.Portal, ChampionJob.None, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (portalObj == null)
        {
            foreach (var trans in user.PortalPosition)
            {
                if (trans.CompareTag("PlayerPortal"))
                    portalObjPosition = trans;
            }

            portalObj = Manager.Asset.InstantiatePrefab("PortalGate");
            portalObj.transform.position = new Vector3(portalObjPosition.position.x, portalObjPosition.position.y + 5f, portalObjPosition.position.z);
            portalGate = portalObj.GetComponent<PortalGate>();
            portalObj.SetActive(false);
        }


        if (level < 3)
        {
            enemyChampion = new List<GameObject>();
            enemyChampionBase = new List<ChampionBase>();
            maxHp = 0;
            portalGateLevel = 0;

            Deactivate(user);
            return;
        }
        else if (level >= 3 && level < 6)
        {
            portalGateLevel = 1;
            maxHp = 200;
        }
        else if (level >= 6 && level < 8)
        {
            portalGateLevel = 2;
            maxHp = 450;
        }
        else if (level >= 8 && level < 10)
        {
            portalGateLevel = 3;
            maxHp = 700;
        }
        else if (level >= 10)
        {
            portalGateLevel = 4;
            maxHp = 1500;
        }


        portalObj.SetActive(true);

        Debug.Log($"[차원문] 레벨 {level} 적용");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.Portal && cBase.ChampionLine_Second != ChampionLine.Portal)
                continue;

            cBase.Synergy_MaxHP -= maxHp;

            cBase.UpdateChampmionStat();
        }


        portalObj.SetActive(false);

        Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 3)
            return;

        PortalLogic(user);
    }


    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region 차원문 로직

    private void PortalLogic(UserData user)
    {
        UpdateSynergyDataForChampion(user);

        enemyChampion = GetEnemyChampion(user);
        enemyChampionBase = GetEnemyChampionBase(user);
        int index = GetFirstAliveEnemyIndex(enemyChampionBase);

        portalGate.Init(portalChampionLevel, enemyChampion[index]);
    }

    private void UpdateSynergyDataForChampion(UserData user)
    {
        portalChampionLevel = 0;

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;


            if (cBase.ChampionLine_First != ChampionLine.Portal && cBase.ChampionLine_Second != ChampionLine.Portal)
                continue;

            portalChampionLevel += cBase.ChampionLevel;
            cBase.Synergy_MaxHP += maxHp;
            cBase.ChampionHpMpController.AddHealth(maxHp, 1.0f);

            cBase.UpdateChampmionStat();
        }
    }

    private int GetFirstAliveEnemyIndex(List<ChampionBase> enemyChampionBase)
    {
        return enemyChampionBase.FindIndex(champion => !champion.ChampionHpMpController.IsDie());
    }

    private List<GameObject> GetEnemyChampion(UserData user)
    {
        return user.BattleChampionObject;
    }

    private List<ChampionBase> GetEnemyChampionBase(UserData user)
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
