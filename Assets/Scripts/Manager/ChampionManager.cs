using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChampionManager
{
    private GameDataBlueprint gameDataBlueprint;

    #region �ʱ�ȭ
    public void Init(GameDataBlueprint gameDataBlueprint)
    {
        this.gameDataBlueprint = gameDataBlueprint;
    }
    #endregion

    #region è�Ǿ� ����

    /// <summary>
    /// ���� è�Ǿ� ����
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


    #region ���� è�Ǿ�

    #region ��ü è�Ǿ�
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

        // HexDictionary�� Ÿ�� ó��
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

        // HexDictionary�� Ÿ�� ó��
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

    #region ��ȭ����
    private GameObject MergeChampion(UserData userData, GameObject champion)
    {
        List<ItemBlueprint> itemList = new List<ItemBlueprint> ();


        int countToRemove = 2;
        GameObject championToEnhance = champion;

        // ����Ʈ�� �ڿ������� ��ȸ�ϸ� �ߺ� è�Ǿ� ����, �ε��� ������
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

                    // Ÿ���� ���� ���� ������Ʈ (�ʿ� ��)
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