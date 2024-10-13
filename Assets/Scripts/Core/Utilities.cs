using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class Utilities
{
    public static T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() ?? obj.AddComponent<T>();
    }

    #region Cost Color
    private static string SetCostColor(ChampionCost championCost)
    {
        switch (championCost)
        {
            case ChampionCost.OneCost:
                return "#C3C2C5"; // 회색
            case ChampionCost.TwoCost:
                return "#93DCC3"; // 녹색
            case ChampionCost.ThreeCost:
                return "#56BAF8"; // 파란색
            case ChampionCost.FourCost:
                return "#D500FF"; // 보라색
            case ChampionCost.FiveCost:
                return "#FFD150"; // 노란색
            default:
                return "등록되지 않은 챔피언 단계";
        }
    }

    public static Color SetSlotColor(ChampionCost championCost)
    {
        ColorUtility.TryParseHtmlString(SetCostColor(championCost), out Color color);
        return color;
    }
    #endregion


    #region Cost IntValue
    private static int SetCost(ChampionCost championCost)
    {
        switch (championCost)
        {
            case ChampionCost.OneCost:
                return 1;
            case ChampionCost.TwoCost:
                return 2;
            case ChampionCost.ThreeCost:
                return 3; 
            case ChampionCost.FourCost:
                return 4; 
            case ChampionCost.FiveCost:
                return 5;
            default:
                return 0;
        }
    }

    public static int SetSlotCost(ChampionCost championCost)
    {
        int cost = SetCost(championCost);
        return cost;
    }
    #endregion

    #region ItemAttribute Description

    private static string SetDescription(ItemAttributeType iType)
    {
        switch (iType)
        {
            case ItemAttributeType.AD_Power:
                return "공격력";
            case ItemAttributeType.AD_Speed:
                return "공격 속도";
            case ItemAttributeType.AD_Defense:
                return "방어력";
            case ItemAttributeType.AP_Power:
                return "주문력";
            case ItemAttributeType.AP_Defense:
                return "마법 저항력";
            case ItemAttributeType.Mana:
                return "마나";
            case ItemAttributeType.HP:
                return "체력";
            case ItemAttributeType.CriticalPercent:
                return "치명타 확률";
            case ItemAttributeType.BloodSuck:
                return "모든 피해 흡혈";
            case ItemAttributeType.Total_Power:
                return "피해량";
            case ItemAttributeType.Special:
                return "내구력";
            default:
                return "등록되지 않은 타입";
        }
    }

    public static string SetItemAttributeDescription(ItemAttributeType iType)
    {
        string desc = SetDescription(iType);
        return desc;
    }
    #endregion
}
