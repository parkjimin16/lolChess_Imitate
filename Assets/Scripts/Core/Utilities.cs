using JetBrains.Annotations;
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

    #region Attribute Name

    private static string SetAttributeNameLine(ChampionLine line)
    {
        switch (line)
        {
            case ChampionLine.None:
                return "None";
            case ChampionLine.Sugarcraft:
                return "달콤술사";
            case ChampionLine.Druid:
                return "드루이드";
            case ChampionLine.Witchcraft:
                return "마녀";
            case ChampionLine.Meadist:
                return "벌꿀술사";
            case ChampionLine.Frost:
                return "서리";
            case ChampionLine.Eldritch:
                return "섬뜩한 힘";
            case ChampionLine.SpaceAndTime:
                return "시공간";
            case ChampionLine.Arcana:
                return "아르카나";
            case ChampionLine.Fairy:
                return "요정";
            case ChampionLine.Dragon:
                return "용";
            case ChampionLine.Portal:
                return "차원문";
            case ChampionLine.Hunger:
                return "허기";
            case ChampionLine.Pyro:
                return "화염";
            default:
                return "오류";

        }
    }

    public static string SetLineName(ChampionLine line)
    {
        string _line = SetAttributeNameLine(line);
        return _line;
    }


    private static string SetAttributeNameJob(ChampionJob job)
    {
        switch (job)
        {
            case ChampionJob.None:
                return "None";
            case ChampionJob.Pal:
                return "단짝";
            case ChampionJob.Mage:
                return "마도사";
            case ChampionJob.Batqueen:
                return "박쥐여왕";
            case ChampionJob.Shelter:
                return "보호술사";
            case ChampionJob.Hunter:
                return "사냥꾼";
            case ChampionJob.Vanguard:
                return "선봉대";
            case ChampionJob.Rusher:
                return "쇄도자";
            case ChampionJob.Bastion:
                return "요새";
            case ChampionJob.Enchantress:
                return "요술사";
            case ChampionJob.Warrior:
                return "전사";
            case ChampionJob.Overmind:
                return "초월체";
            case ChampionJob.Demolition:
                return "폭파단";
            case ChampionJob.Scholar:
                return "학자";
            case ChampionJob.Transmogrifier:
                return "형상변환자";
            default:
                return "오류";
        }
    }

    public static string SetJobName(ChampionJob job)
    {
        string _job = SetAttributeNameJob(job);
        return _job;
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
            case ItemAttributeType.TotalPower:
                return "피해량";
            case ItemAttributeType.TotalDefense:
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

    private static string SetDescriptionValue(ItemAttribute item)
    {
        ItemAttributeType iType = item.ItemAttributeType;

        switch (iType)
        {
            case ItemAttributeType.AD_Power:
                return (item.AttributeValue * 100).ToString();
            case ItemAttributeType.AD_Speed:
                return (item.AttributeValue * 100).ToString();
            case ItemAttributeType.AD_Defense:
                return item.AttributeValue.ToString();
            case ItemAttributeType.AP_Power:
                return item.AttributeValue.ToString();
            case ItemAttributeType.AP_Defense:
                return item.AttributeValue.ToString();
            case ItemAttributeType.Mana:
                return item.AttributeValue.ToString();
            case ItemAttributeType.HP:
                return item.AttributeValue.ToString();
            case ItemAttributeType.CriticalPercent:
                return (item.AttributeValue * 100).ToString();
            case ItemAttributeType.BloodSuck:
                return (item.AttributeValue * 100).ToString();
            case ItemAttributeType.TotalPower:
                return (item.AttributeValue * 100).ToString();
            case ItemAttributeType.TotalDefense:
                return (item.AttributeValue * 100).ToString();
            default:
                return "등록되지 않은 타입";
        }
    }

    public static string SetDescriptionValueReturnString(ItemAttribute item)
    {
        string value = SetDescriptionValue(item);
        return value;
    }
    #endregion
}
