using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChampionManager
{
    #region ��ü è�Ǿ�
    private void SettingAllChampion(UserData userData)
    {
        userData.TotalChampionObject.Clear();
        userData.NonBattleChampionObject.Clear();
        userData.BattleChampionObject.Clear();

        foreach (var tileEntry in userData.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied && tile.championOnTile != null && tile.championOnTile.CompareTag("Champion"))
            {
                userData.TotalChampionObject.Add(tile.championOnTile);
                userData.NonBattleChampionObject.Add(tile.championOnTile);
            }
        }

        foreach (var tileEntry in userData.MapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied && tile.championOnTile != null)
            {
                userData.TotalChampionObject.Add(tile.championOnTile);
                userData.BattleChampionObject.Add(tile.championOnTile);
            }
        }

        Debug.Log($"��ü è�Ǿ� �� : {userData.TotalChampionObject.Count}");
        Debug.Log($"��Ʋ è�Ǿ� �� : {userData.NonBattleChampionObject.Count}");
        Debug.Log($"���Ʋ è�Ǿ� �� : {userData.BattleChampionObject.Count}");
    }
    private void SettingTotalChampion(UserData userData)
    {
        userData.TotalChampionObject.Clear();

        foreach (var tileEntry in userData.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied && tile.championOnTile != null && tile.championOnTile.CompareTag("Champion"))
            {
                userData.TotalChampionObject.Add(tile.championOnTile);
            }
        }

        foreach (var tileEntry in userData.MapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied && tile.championOnTile != null && tile.championOnTile.CompareTag("Champion"))
            {
                userData.TotalChampionObject.Add(tile.championOnTile);
            }
        }
    }
    #endregion

    #region ������è�Ǿ�
    /// <summary>
    /// �������� �� ȣ�� + �ؿ� ���� ��
    /// </summary>
    /// <param name="userData"></param>
    public void SettingNonBattleChampion(UserData userData)
    {
        SettingTotalChampion(userData);
        userData.NonBattleChampionObject.Clear();

        foreach (var tileEntry in userData.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied && tile.championOnTile != null && tile.championOnTile.CompareTag("Champion"))
            {
                AddNonBattleChampion(userData, tile.championOnTile);
            }
        }
    }

    public void AddNonBattleChampion(UserData userData, GameObject champion)
    {
        //if (userData.NonBattleChampionObject.Count <= 0)
        //    return;

        userData.NonBattleChampionObject.Add(champion);

        ChampionBase cBase = champion.GetComponent<ChampionBase>();

        // 2��
        int sameChampionCount = userData.TotalChampionObject.Count(obj =>
        {
            ChampionBase championBase = obj.GetComponent<ChampionBase>();
            return championBase != null && cBase != null && championBase.ChampionName == cBase.ChampionName && championBase.ChampionLevel == cBase.ChampionLevel;
        });

        GameObject championToEnhance = null;

        if (sameChampionCount >= 3)
        {
            championToEnhance = MergeChampion(userData, champion);
            SettingAllChampion(userData);
        }

        if (championToEnhance == null)
            return;

        // 3��
        ChampionBase tempChampionBase = championToEnhance.GetComponent<ChampionBase>();
        string championName = tempChampionBase.ChampionName;
        int sameChampionAfterMergeCount = userData.TotalChampionObject.Count(obj =>
        {
            ChampionBase championBase = obj.GetComponent<ChampionBase>();
            return championBase != null && championBase.ChampionName == championName && championBase.ChampionLevel == 2;
        });

        if (sameChampionAfterMergeCount >= 3)
        {
            championToEnhance = MergeChampion(userData, championToEnhance);
            SettingAllChampion(userData);
        }
    }
    public void RemoveNonBattleChampion(UserData userData, GameObject champion)
    {

    }
    #endregion

    #region ����è�Ǿ�

    /// <summary>
    /// ���� ������ �� ȣ��
    /// </summary>
    public void SettingBattleChampion(UserData userData)
    {
        SettingTotalChampion(userData);
        userData.BattleChampionObject.Clear();

        foreach (var tileEntry in userData.MapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied && tile.championOnTile != null && tile.championOnTile.CompareTag("Champion"))
            {
                AddBattleChampion(userData, tile.championOnTile);
            }
        }
    }

    public void AddBattleChampion(UserData userData, GameObject champion)
    {
        userData.BattleChampionObject.Add(champion);

        ChampionBase addChampionBase = champion.GetComponent<ChampionBase>();

        if (addChampionBase == null || userData.BattleChampionObject == null)
            return;


        Manager.Synerge.AddSynergyLine(userData, addChampionBase.ChampionName, Utilities.GetLineName(addChampionBase.ChampionLine_First));
        Manager.Synerge.AddSynergyLine(userData, addChampionBase.ChampionName, Utilities.GetLineName(addChampionBase.ChampionLine_Second));
        Manager.Synerge.AddSynergyJob(userData, addChampionBase.ChampionName, Utilities.GetJobName(addChampionBase.ChampionJob_First));
        Manager.Synerge.AddSynergyJob(userData, addChampionBase.ChampionName, Utilities.GetJobName(addChampionBase.ChampionJob_Second));
    }

    public void RemoveBattleChampion(UserData userData, GameObject champion)
    {

    }
    #endregion

    #region ��ȭ����
    private GameObject MergeChampion(UserData userData, GameObject champion)
    {
        List<ItemBlueprint> itemList = new List<ItemBlueprint> ();


        int countToRemove = 2;
        GameObject championToEnhance = champion;

        // ����Ʈ�� �ڿ������� ��ȸ�ϸ� �ߺ� è�Ǿ� ����, �ε��� ������
        for (int i = userData.TotalChampionObject.Count - 1; i >= 0 && countToRemove > 0; i--)
        {
            ChampionBase championBase = userData.TotalChampionObject[i].GetComponent<ChampionBase>();
            if (championBase != null && championBase.ChampionName == championToEnhance.GetComponent<ChampionBase>().ChampionName)
            {
                if (championBase == championToEnhance.GetComponent<ChampionBase>())
                {
                    continue;
                }


                itemList.AddRange(championBase.EquipItem);

                userData.TotalChampionObject[i].GetComponentInParent<HexTile>().isOccupied = false;
                userData.TotalChampionObject[i].GetComponentInParent<HexTile>().championOnTile = null;
                Utilities.Destroy(userData.TotalChampionObject[i]);
                userData.TotalChampionObject.RemoveAt(i);
                
                countToRemove--;
            }
        }

        if (itemList.Count > 0)
        {
            Manager.Item.StartCreatingItems(itemList, new Vector3(0, 10, 0));
        }

        itemList.Clear();
        EnhanceChampion(championToEnhance);
        return championToEnhance;
    }

    private void EnhanceChampion(GameObject champion)
    {
        var cBase = champion.GetComponent<ChampionBase>();
        if (cBase != null)
        {
            cBase.ChampionLevelUp();
        }
    }
    #endregion
}

[System.Serializable]
public class ChampionLevelData
{
    public int Level;        // ���
    public int Hp;
    public int Power;
}

[System.Serializable]
public class ChampionData
{
    public int Cost;
    public string[] Names;
}

[System.Serializable]
public class ChampionRandomData
{
    public int Level;
    public float[] Probability;
}

[System.Serializable]
public class ChampionMaxCount
{
    public int Cost;
    public int MaxCount;
}