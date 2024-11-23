using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeadistSynergy : SynergyBase
{
    // �ó��� ����
    private int level;

    // ���ܼ��� ���� ����
    private float totalPower;
    private float totalDefense;

    private List<GameObject> beeObjects = new List<GameObject>();

    public MeadistSynergy()
        : base("���ܼ���", ChampionLine.Meadist, ChampionJob.None, 0) { }

    #region Ȱ�� & ��Ȱ��ȭ
    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;


        if (level >= 0 && level < 3)
        {
            totalPower = 0;
            totalDefense = 0;
            Deactivate(user);
        }
        else if (level >= 3 && level < 5)
        {
            totalPower = 0.07f;
            totalDefense = 0.03f;
        }
        else if (level >= 5 && level < 7)
        {
            totalPower = 0.13f;
            totalDefense = 0.05f;
        }
        else if (level >= 7)
        {
            totalPower = 0.2f;
            totalDefense = 0.1f;
        }
        //Debug.Log($"[���ܼ���] ���� {level} ȿ�� ����");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.Meadist && cBase.ChampionLine_Second != ChampionLine.Meadist)
                continue;


            cBase.Synergy_Power_Upgrade -= totalPower;
            cBase.Synergy_Total_Def -= totalDefense;

            cBase.UpdateChampmionStat();
        }

        totalPower = 0;
        totalDefense = 0;


        if(beeObjects.Count> 0)
        {
            foreach (var bee in beeObjects)
            {
                Destroy(bee);
            }
        }



        //Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    public override void Activate(UserData user)
    {
        if (level < 3)
            return;

        SetBee(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region ���ܼ��� ����

    private void SetBee(UserData user)
    {
        var list = GetMeadistChampion(user);

        foreach(var champion in list)
        {
            GameObject bee = Manager.Asset.InstantiatePrefab("Bee", champion.transform);
            bee.SetActive(true);

            ChampionBase cBase = champion.GetComponent<ChampionBase>();
            cBase.Synergy_Power_Upgrade += totalPower;
            cBase.Synergy_Total_Def += totalDefense;

           
            cBase.UpdateChampmionStat();
            beeObjects.Add(bee);
        }
    }

    private List<GameObject> GetMeadistChampion(UserData user)
    {
        var list = new List<GameObject>();

        foreach(var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            Debug.Log(cBase.ChampionName);

            if (cBase.ChampionLine_First != ChampionLine.Meadist && cBase.ChampionLine_Second != ChampionLine.Meadist)
                continue;

            list.Add(champion);
        }

        return list;
    }

    #endregion
}
