using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChampionManager
{
    private GameDataBlueprint gameDataBlueprint;

    #region 초기화
    public void Init(GameDataBlueprint gameDataBlueprint)
    {
        this.gameDataBlueprint = gameDataBlueprint;
    }
    #endregion

    #region 챔피언 로직

    /// <summary>
    /// 상점 챔피언 갱신
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public List<string> GetRandomChampions(int level)
    {
        List<string> selectedChampions = new List<string>();

        ChampionRandomData currentData = gameDataBlueprint.ChampionRandomDataList[level - 1];

        for (int i = 0; i < 5; i++)
        {
            int costIndex = GetCostIndex(currentData.Probability);
            ChampionData costChampionData = GetChampionDataByCost(costIndex + 1);
            if (costChampionData != null && costChampionData.Names.Length > 0)
            {
                string selectedChampion = costChampionData.Names[Random.Range(0, costChampionData.Names.Length)];
                selectedChampions.Add(selectedChampion);
            }
        }

        return selectedChampions;
    }

    private int GetCostIndex(float[] probabilities)
    {
        float[] cumulativeProbabilities = new float[probabilities.Length];
        cumulativeProbabilities[0] = probabilities[0];

        for (int i = 1; i < probabilities.Length; i++)
        {
            cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + probabilities[i];
        }

        float randomValue = Random.Range(0f, 1f);

        for (int i = 0; i < cumulativeProbabilities.Length; i++)
        {
            if (randomValue < cumulativeProbabilities[i])
            {
                return i;
            }
        }

        return probabilities.Length - 1;
    }

    private ChampionData GetChampionDataByCost(int cost)
    {
        foreach (ChampionData data in gameDataBlueprint.ChampionDataList)
        {
            if (data.Cost == cost)
            {
                return data;
            }
        }
        return null;
    }


    public string GetRandomChapmion(int cost)
    {
        string newChampion = string.Empty;

        int count =  gameDataBlueprint.ChampionDataList[cost - 1].Names.Length;
        int randomIndex = Random.Range(0, count);
        newChampion = gameDataBlueprint.ChampionDataList[cost - 1].Names[randomIndex];

        return newChampion;
    }

    public void InstantiateChampion(UserData user, ChampionBlueprint cBlueprint, HexTile hextile, Transform tileTransform)
    {
        GameObject newChampionObject = Manager.Asset.InstantiatePrefab(cBlueprint.ChampionInstantiateName);
        GameObject frame = Manager.Asset.InstantiatePrefab("ChampionFrame");
        frame.transform.SetParent(newChampionObject.transform, false);
        newChampionObject.transform.position = tileTransform.position;

        newChampionObject.transform.SetParent(hextile.transform);
        hextile.championOnTile.Add(newChampionObject);

        ChampionBase cBase = newChampionObject.GetComponent<ChampionBase>();
        ChampionFrame cFrame = frame.GetComponentInChildren<ChampionFrame>();

        cBase.SetChampion(cBlueprint);
        cBase.InitChampion(cFrame);

        Manager.Champion.SettingNonBattleChampion(user);
    }
    #endregion


    #region 유저 챔피언

    #region 전체 챔피언
    private void SettingAllChampion(UserData userData)
    {
        userData.TotalChampionObject.Clear();
        userData.NonBattleChampionObject.Clear();
        userData.BattleChampionObject.Clear();

        foreach (var tileEntry in userData.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile)
                {
                    if (champion.CompareTag("Champion"))
                    {
                        userData.TotalChampionObject.Add(champion);
                        userData.NonBattleChampionObject.Add(champion);
                    }
                }
            }
        }

        // HexDictionary의 타일 처리
        foreach (var tileEntry in userData.MapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile)
                {
                    if (champion.CompareTag("Champion"))
                    {
                        userData.TotalChampionObject.Add(champion);
                        userData.BattleChampionObject.Add(champion);
                    }
                }
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
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile)
                {
                    if (champion.CompareTag("Champion"))
                    {
                        userData.TotalChampionObject.Add(champion);
                    }
                }
            }
        }

        // HexDictionary의 타일 처리
        foreach (var tileEntry in userData.MapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile)
                {
                    if (champion.CompareTag("Champion"))
                    {
                        userData.TotalChampionObject.Add(champion);
                    }
                }
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
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile)
                {
                    if (champion.CompareTag("Champion"))
                    {
                        AddNonBattleChampion(userData, champion);
                    }
                }
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
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile)
                {
                    if (champion.CompareTag("Champion"))
                    {
                        AddBattleChampion(userData, champion);
                    }
                }
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
            GameObject currentChampionObj = userData.TotalChampionObject[i];
            ChampionBase championBase = userData.TotalChampionObject[i].GetComponent<ChampionBase>();
            if (championBase != null && championBase.ChampionName == championToEnhance.GetComponent<ChampionBase>().ChampionName)
            {
                if (championBase == championToEnhance.GetComponent<ChampionBase>())
                {
                    continue;
                }


                itemList.AddRange(championBase.EquipItem);

                HexTile parentTile = currentChampionObj.GetComponentInParent<HexTile>();
                if (parentTile != null)
                {
                    parentTile.championOnTile.Remove(currentChampionObj);

                    // 타일의 점유 상태 업데이트 (필요 시)
                    // parentTile.isOccupied = parentTile.championsOnTile.Count > 0;
                }
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