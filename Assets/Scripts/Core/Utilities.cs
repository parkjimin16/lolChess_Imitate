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
                return "#C3C2C5"; // ȸ��
            case ChampionCost.TwoCost:
                return "#93DCC3"; // ���
            case ChampionCost.ThreeCost:
                return "#56BAF8"; // �Ķ���
            case ChampionCost.FourCost:
                return "#D500FF"; // �����
            case ChampionCost.FiveCost:
                return "#FFD150"; // �����
            default:
                return "��ϵ��� ���� è�Ǿ� �ܰ�";
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
                return "���޼���";
            case ChampionLine.Druid:
                return "����̵�";
            case ChampionLine.Witchcraft:
                return "����";
            case ChampionLine.Meadist:
                return "���ܼ���";
            case ChampionLine.Frost:
                return "����";
            case ChampionLine.Eldritch:
                return "������ ��";
            case ChampionLine.SpaceAndTime:
                return "�ð���";
            case ChampionLine.Arcana:
                return "�Ƹ�ī��";
            case ChampionLine.Fairy:
                return "����";
            case ChampionLine.Dragon:
                return "��";
            case ChampionLine.Portal:
                return "������";
            case ChampionLine.Hunger:
                return "���";
            case ChampionLine.Pyro:
                return "ȭ��";
            default:
                return "����";

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
                return "��¦";
            case ChampionJob.Mage:
                return "������";
            case ChampionJob.Batqueen:
                return "���㿩��";
            case ChampionJob.Shelter:
                return "��ȣ����";
            case ChampionJob.Hunter:
                return "��ɲ�";
            case ChampionJob.Vanguard:
                return "������";
            case ChampionJob.Rusher:
                return "�⵵��";
            case ChampionJob.Bastion:
                return "���";
            case ChampionJob.Enchantress:
                return "�����";
            case ChampionJob.Warrior:
                return "����";
            case ChampionJob.Overmind:
                return "�ʿ�ü";
            case ChampionJob.Demolition:
                return "���Ĵ�";
            case ChampionJob.Scholar:
                return "����";
            case ChampionJob.Transmogrifier:
                return "����ȯ��";
            default:
                return "����";
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
                return "���ݷ�";
            case ItemAttributeType.AD_Speed:
                return "���� �ӵ�";
            case ItemAttributeType.AD_Defense:
                return "����";
            case ItemAttributeType.AP_Power:
                return "�ֹ���";
            case ItemAttributeType.AP_Defense:
                return "���� ���׷�";
            case ItemAttributeType.Mana:
                return "����";
            case ItemAttributeType.HP:
                return "ü��";
            case ItemAttributeType.CriticalPercent:
                return "ġ��Ÿ Ȯ��";
            case ItemAttributeType.BloodSuck:
                return "��� ���� ����";
            case ItemAttributeType.TotalPower:
                return "���ط�";
            case ItemAttributeType.TotalDefense:
                return "������";
            default:
                return "��ϵ��� ���� Ÿ��";
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
                return "��ϵ��� ���� Ÿ��";
        }
    }

    public static string SetDescriptionValueReturnString(ItemAttribute item)
    {
        string value = SetDescriptionValue(item);
        return value;
    }
    #endregion
}
