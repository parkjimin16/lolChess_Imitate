using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SugarcraftSynergy : SynergyBase
{
    // 시너지 변수
    private int sugarCount;
    private float attackPower;
    private float spellPower;
    private int totalSugarCount;



    // 달콤술사 로직 변수
    private int cakeStack; // 케이크 층 수
    private GameObject sugarCake;
    private SugarCraftCake sugarCraftCake;

    public SugarcraftSynergy()
        : base("달콤술사", ChampionLine.Sugarcraft, ChampionJob.None, 0) { }

    #region Unity Flow
    private void Awake()
    {
        totalSugarCount = 0;
    }

    #endregion

    #region 활성 & 비활성화
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


        Debug.Log($"[달콤술사] 레벨 {level} 적용: 설탕 {sugarCount}, 공격력 {attackPower}, 주문력 {spellPower}");
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

        Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        Debug.Log("Activate");
        GainSugarFromAlliedItems(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
        Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }
    #endregion

    #region 달콤술사 로직
    private void GainSugarFromAlliedItems(UserData user)
    {
        int totalItemCount = CountAlliedCombinationItems(user);

        int gainedSugar = totalItemCount * sugarCount;
        totalSugarCount += gainedSugar;

        UpdateCakeStack();
        UpdateSynergyDataForChampion(user);
        Debug.Log($"아이템 {totalItemCount}개로 설탕 {gainedSugar}개 획득. 총 설탕: {totalSugarCount}");
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
            Debug.Log("달콤술사 스택");
        }
        else if(totalSugarCount >= 600 && totalSugarCount < 975)
        {
            sugarCraftCake.SetCake(1);
            Debug.Log("달콤술사 스택 1");
        }
        else if (totalSugarCount >= 975 && totalSugarCount < 1375)
        {
            sugarCraftCake.SetCake(2);
            Debug.Log("달콤술사 스택 2");
        }
        else if (totalSugarCount >= 1375 && totalSugarCount < 1800)
        {
            sugarCraftCake.SetCake(3);
            Debug.Log("달콤술사 스택 3");
        }
        else if (totalSugarCount >= 1800 && totalSugarCount < 2300)
        {
            sugarCraftCake.SetCake(4);
            Debug.Log("달콤술사 스택 4");
        }
        else if (totalSugarCount >= 2300)
        {
            sugarCraftCake.SetCake(5);
            Debug.Log("달콤술사 스택 5");
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
        // 아군 체력 증가 로직 구현
    }

    #endregion
}
