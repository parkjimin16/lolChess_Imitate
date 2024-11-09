using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChampionManager
{
    #region 전체 챔피언
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

        Debug.Log($"전체 챔피언 수 : {userData.TotalChampionObject.Count}");
        Debug.Log($"배틀 챔피언 수 : {userData.NonBattleChampionObject.Count}");
        Debug.Log($"논배틀 챔피언 수 : {userData.BattleChampionObject.Count}");
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

    #region 비전투챔피언
    /// <summary>
    /// 구매했을 때 호출 + 밑에 놓을 때
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

        // 2성
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

        // 3성
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

    #region 전투챔피언

    /// <summary>
    /// 위에 놓았을 때 호출
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

    #region 강화로직
    private GameObject MergeChampion(UserData userData, GameObject champion)
    {
        List<ItemBlueprint> itemList = new List<ItemBlueprint> ();


        int countToRemove = 2;
        GameObject championToEnhance = champion;

        // 리스트를 뒤에서부터 순회하며 중복 챔피언 제거, 인덱스 에러남
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
    public int Level;        // 등급
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