using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ChampionBlueprint", menuName = "Blueprints/ChampionBlueprint")]
public class ChampionBlueprint : ScriptableObject
{
    [Header("Champion Info")]
    [SerializeField] private string championName;
    [SerializeField] private GameObject championObj;
    [SerializeField] private ChampionLine championLine_First;
    [SerializeField] private ChampionLine championLine_Second;
    [SerializeField] private ChampionJob championJob_First;
    [SerializeField] private ChampionJob championJob_Second;
    [SerializeField] private ChampionCost championCost;
    [SerializeField] private Image championImage;

    [Header("Champion Stats")]
    [SerializeField] private List<ChampionLevelData> championLevelData;
    [SerializeField] private float attack_Speed;
    [SerializeField] private int ad_Defense;
    [SerializeField] private int ap_Defense;
    [SerializeField] private float speed;
    [SerializeField] private float mana_Total;
    [SerializeField] private float mana_Cur;
    [SerializeField] private int attack_Range;
    [SerializeField] private SkillBlueprint skillBlueprint;


    public string ChampionName => championName;

    public GameObject ChampionObj => championObj;

    public ChampionLine ChampionLine_First => championLine_First;

    public ChampionLine ChampionLine_Second => championLine_Second;

    public ChampionJob ChampionJob_First => championJob_First;

    public ChampionJob ChampionJob_Second => championJob_Second;

    public ChampionCost ChampionCost => championCost;

    public Image ChampionImage => championImage;

    public List<ChampionLevelData> ChampionLevelData => championLevelData;

    public float AttackSpeed => attack_Speed;

    public int AD_Defense => ad_Defense;

    public int AP_Defense => ap_Defense;

    public float Speed => speed;
    public float Mana_Total => mana_Total;
    public float Mana_Cur => mana_Cur;
    public int Attack_Range => attack_Range;

    public SkillBlueprint SkillBlueprint => skillBlueprint;
}

