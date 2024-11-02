using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ChampionBlueprint", menuName = "Blueprints/ChampionBlueprint")]
public class ChampionBlueprint : ScriptableObject
{
    [Header("Champion Info")]
    [SerializeField] private string championInstantiateName;
    [SerializeField] private string championName;
    [SerializeField] private GameObject championObj;
    [SerializeField] private ChampionLine championLine_First;
    [SerializeField] private ChampionLine championLine_Second;
    [SerializeField] private ChampionJob championJob_First;
    [SerializeField] private ChampionJob championJob_Second;
    [SerializeField] private ChampionCost championCost;
    [SerializeField] private Sprite championImage;

    [Header("Champion Stats_1")]
    [SerializeField] private List<ChampionLevelData> championLevelData;

    [SerializeField] private int championLevel;
    [SerializeField] private float maxHp;
    [SerializeField] private float curHp;
    [SerializeField] private float maxMana;
    [SerializeField] private float curMana;
    [SerializeField] private int attack_Range;
    [SerializeField] private float speed;
    [SerializeField] private SkillBlueprint skillBlueprint;


    [Header("Champion Stats_2")]
    [SerializeField] private float ad_Power;
    [SerializeField] private float ap_Power;
    [SerializeField] private int ad_Defense;
    [SerializeField] private int ap_Defense;
    [SerializeField] private float attack_Speed;
    [SerializeField] private float critical_Percent;
    [SerializeField] private float critical_Power;
    [SerializeField] private float blood_Suck;
    [SerializeField] private float power_Upgrade;
    [SerializeField] private float total_Defense;



    // Champion Info
    public string ChampionInstantiateName => championInstantiateName;
    public string ChampionName => championName;
    public GameObject ChampionObj => championObj;
    public ChampionLine ChampionLine_First => championLine_First;
    public ChampionLine ChampionLine_Second => championLine_Second;
    public ChampionJob ChampionJob_First => championJob_First;
    public ChampionJob ChampionJob_Second => championJob_Second;
    public ChampionCost ChampionCost => championCost;
    public Sprite ChampionImage => championImage;


    // Champion Stats_1
    public List<ChampionLevelData> ChampionLevelData => championLevelData;
    public int ChampionLevel => championLevel;
    public float MaxHP => maxHp;
    public float CurHP => curHp;
    public float MaxMana => maxMana;
    public float CurMana => curMana;
    public float Speed => speed;
    public int Attack_Range => attack_Range;
    public SkillBlueprint SkillBlueprint => skillBlueprint;


    // Champion Stats_2
    public float AD_Power => ad_Power;
    public float AP_Power => ap_Power;
    public int AD_Defense => ad_Defense;
    public int AP_Defense => ap_Defense;
    public float AttackSpeed => attack_Speed;
    public float Critical_Percent => critical_Percent;
    public float Critical_Power => critical_Power;
    public float Blood_Suck => blood_Suck;
    public float Power_Upgrade => power_Upgrade;
    public float Total_Defense => total_Defense;


    public void ChampionSet(int level)
    {
        maxHp = championLevelData[level - 1].Hp;
        ad_Power = championLevelData[level - 1].Power;

        curHp = maxHp;
    }

    public int GetLevelHp(int level)
    {
        return ChampionLevelData[level - 1].Hp;
    }

    public int GetLevelAdPower(int level)
    {
        return ChampionLevelData[level - 1].Power;
    }
}


