using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SugarcraftSynergy : SynergyBase
{
    // �ó��� ����
    private int sugarCount;
    private float attackPower;
    private float spellPower;
    private int totalSugarCount;



    // ���޼��� ���� ����
    private int cakeStack; // ����ũ �� ��
    private GameObject sugarCake;
    private SugarCraftCake sugarCraftCake;

    public SugarcraftSynergy()
        : base("���޼���", ChampionLine.Sugarcraft, ChampionJob.None, 0) { }

    #region Unity Flow
    private void Awake()
    {
        totalSugarCount = 0;
    }

    #endregion

    #region Ȱ�� & ��Ȱ��ȭ
    protected override void ApplyEffects(UserData user, int level)
    {
        if (sugarCake == null)
        {
            GameObject obj = GameObject.Find("Craft");

            if (obj == null)
                Debug.Log("Craft Null");


            sugarCake = Manager.Asset.InstantiatePrefab("SugarCraftCake", obj.transform);

            Debug.Log(sugarCake.transform.position);
            sugarCraftCake = sugarCake.GetComponent<SugarCraftCake>();

            sugarCake.SetActive(true);
        }


        if (level < 2)
        {
            sugarCount = 0;
            attackPower = 0;
            spellPower = 0;
            Deactivate(user);
            return;
        }

        if (level >= 2 && level < 4)
        {
            sugarCount = 3;
            attackPower = 22;
            spellPower = 22;
        }
        else if (level >= 4 && level < 6)
        {
            sugarCount = 4;
            attackPower = 33;
            spellPower = 33;
        }
        else if (level >= 6)
        {
            sugarCount = 6;
            attackPower = 36;
            spellPower = 36;
            IncreaseAlliesHealth(100);
        }

        sugarCake.SetActive(true);
        //Activate(user);


        Debug.Log($"[���޼���] ���� {level} ����: ���� {sugarCount}, ���ݷ� {attackPower}, �ֹ��� {spellPower}");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;


            if (cBase.ChampionLine_First != ChampionLine.Sugarcraft && cBase.ChampionLine_Second != ChampionLine.Sugarcraft)
                continue;


            cBase.Synergy_AD_Power -= attackPower;
            cBase.Synergy_AP_Power -= spellPower;

            cBase.UpdateChampmionStat();
        }


        sugarCraftCake.SetCake(0);
        sugarCake.SetActive(false);
        totalSugarCount = 0;
        attackPower = 0;
        spellPower = 0;

        Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    public override void Activate(UserData user)
    {
        Debug.Log("Activate");
        GainSugarFromAlliedItems(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
        Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }
    #endregion

    #region ���޼��� ����
    private void GainSugarFromAlliedItems(UserData user)
    {
        int totalItemCount = CountAlliedCombinationItems(user);

        int gainedSugar = totalItemCount * sugarCount;
        totalSugarCount += gainedSugar;

        UpdateCakeStack();
        UpdateSynergyDataForChampion(user);
        Debug.Log($"������ {totalItemCount}���� ���� {gainedSugar}�� ȹ��. �� ����: {totalSugarCount}");
    }

    private int CountAlliedCombinationItems(UserData user)
    {
        int totalItems = 0;

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cbase = champion.GetComponent<ChampionBase>();
            if (cbase == null)
                return 0;

            totalItems += cbase.EquipItem.Count;
        }

        return totalItems;
    }

    private void UpdateCakeStack()
    {
        if(totalSugarCount >= 1 && totalSugarCount < 600)
        {
            sugarCraftCake.SetCake(0);
            Debug.Log("���޼��� ����");
        }
        else if(totalSugarCount >= 600 && totalSugarCount < 975)
        {
            sugarCraftCake.SetCake(1);
            Debug.Log("���޼��� ���� 1");
        }
        else if (totalSugarCount >= 975 && totalSugarCount < 1375)
        {
            sugarCraftCake.SetCake(2);
            Debug.Log("���޼��� ���� 2");
        }
        else if (totalSugarCount >= 1375 && totalSugarCount < 1800)
        {
            sugarCraftCake.SetCake(3);
            Debug.Log("���޼��� ���� 3");
        }
        else if (totalSugarCount >= 1800 && totalSugarCount < 2300)
        {
            sugarCraftCake.SetCake(4);
            Debug.Log("���޼��� ���� 4");
        }
        else if (totalSugarCount >= 2300)
        {
            sugarCraftCake.SetCake(5);
            Debug.Log("���޼��� ���� 5");
        }

        float additionalStats = 0.01f * totalSugarCount;

        attackPower *= additionalStats;
        spellPower *= additionalStats;
    }

    private void UpdateSynergyDataForChampion(UserData user)
    {
        foreach(var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;


            if (cBase.ChampionLine_First != ChampionLine.Sugarcraft && cBase.ChampionLine_Second != ChampionLine.Sugarcraft)
                continue;


            cBase.Synergy_AD_Power += attackPower;
            cBase.Synergy_AP_Power += spellPower;

            cBase.UpdateChampmionStat();
        }
    }

    private void IncreaseAlliesHealth(int amount)
    {
        // �Ʊ� ü�� ���� ���� ����
    }

    #endregion
}
