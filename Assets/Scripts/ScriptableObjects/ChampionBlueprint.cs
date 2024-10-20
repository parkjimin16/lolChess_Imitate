using System;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline;
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
    [SerializeField] private Image championImage;

    [Header("Champion Stats_1")]
    [SerializeField] private List<ChampionLevelData> championLevelData;

    [SerializeField] private int championLevel;
    [SerializeField] private float hp_Total;
    [SerializeField] private float hp_Cur;
    [SerializeField] private float mana_Total;
    [SerializeField] private float mana_Cur;
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
    public Image ChampionImage => championImage;


    // Champion Stats_1
    public List<ChampionLevelData> ChampionLevelData => championLevelData;
    public int ChampionLevel => championLevel;
    public float HP_Total => hp_Total;
    public float HP_Cur => hp_Cur;
    public float Mana_Total => mana_Total;
    public float Mana_Cur => mana_Cur;
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


    public void ChampionSet()
    {
        hp_Total = championLevelData[0].Hp;
        ad_Power = championLevelData[0].Power;

        hp_Cur = hp_Total;
    }

    public ChampionLevelData GetChampionLevelData(int level)
    {
        ChampionLevelData cData = championLevelData[level - 1];

        return cData;
    }
}

