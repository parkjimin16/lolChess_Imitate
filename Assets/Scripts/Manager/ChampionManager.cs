using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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


    /// <summary>
    /// 챔피언 이름으로 생성
    /// </summary>
    public string GetChampionInstantiateName(string name)
    {
        int index = name.IndexOf('_');

        string instantiateName = index >= 0 ? name.Substring(index + 1) : string.Empty;

        foreach(ChampionData data in gameDataBlueprint.ChampionDataList)
        {
            for(int i =0;i < data.Names.Length; i++)
            {
                if (data.Names[i].Contains(instantiateName))
                {
                    return data.Names[i];
                }
            }
        }

        return string.Empty;
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
        GameObject frame = Manager.ObjectPool.GetGo("ChampionFrame");
        
        frame.transform.SetParent(newChampionObject.transform, false);
        newChampionObject.transform.position = tileTransform.position;

        newChampionObject.transform.SetParent(hextile.transform);
        hextile.championOnTile.Add(newChampionObject);

        ChampionBase cBase = newChampionObject.GetComponent<ChampionBase>();
        ChampionFrame cFrame = frame.GetComponentInChildren<ChampionFrame>();

        Player player = Manager.Game.PlayerListObject[user.UserId].GetComponent<Player>();
        cBase.SetChampion(cBlueprint, player);
        cBase.InitChampion(cFrame);

        if (Manager.Stage.IsBattleOngoing && Manager.Battle.IsUserMove)
        {
            Debug.Log($" 유저 움직임? : {Manager.Battle.IsUserMove}");
            Player enemy = Manager.Stage.FindMyEnemy(user).GetComponent<Player>();

            if (enemy != null)
            {
                Manager.Champion.SettingNonBattleChampion_Battle(user, enemy.UserData);
            }
        }
        else
        {
            Debug.Log($" 유저 안 움직임? : {Manager.Battle.IsUserMove}");
            Manager.Champion.SettingNonBattleChampion(user);
        }


        if(Manager.Stage.IsBattleOngoing)
        {
            ChampionOriginalState originalState = new ChampionOriginalState
            {
                originalPosition = hextile.transform.position,
                originalParent = hextile.transform,
                originalTile = hextile,
                wasActive = newChampionObject.activeSelf,
                originalMapInfo = user.MapInfo // 원래 맵 정보 저장
            };
            Debug.Log(originalState.originalMapInfo.playerData.UserData.UserName);
            user.ChampionOriginState[newChampionObject] = originalState;
        }
    }

    // 챔피언 생성 위치 반환
    public HexTile FindChampionPos(UserData user)
    {
        foreach (var tileEntry in user.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (!tile.isOccupied)
                return tile;
        }

        return null;
    }

    // 챔피언 생성 가능한 위치의 수 반환
    public int GetEmptyTileCount(UserData user)
    {
        int emptyTileCount = 0;

        foreach (var tileEntry in user.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (!tile.isOccupied)
            {
                emptyTileCount++;
            }
        }

        return emptyTileCount;
    }
    #endregion

    #region 챔피언 제거

    public void RemoveChampion(UserData user, GameObject targetChampion)
    {
        ChampionBase cBase = targetChampion.GetComponent<ChampionBase>();

        if (cBase.EquipItem.Count > 0)
        {
            GameObject obj = Manager.ObjectPool.GetGo("Capsule");
            obj.transform.position = new Vector3(0, 1, 0);
            Capsule cap = obj.GetComponent<Capsule>();
            List<string> item = new List<string>();
            List<string> champion = new List<string>();

            for (int i = 0; i < cBase.EquipItem.Count; i++)
            {
                item.Add(cBase.EquipItem[i].ItemId);
            }

            cap.InitCapsule(cBase.ChampionSellCost(Utilities.SetSlotCost(cBase.ChampionCost), cBase.ChampionLevel), item, champion);
        }
        else
        {
            user.UserGold += Utilities.SetSlotCost(cBase.ChampionCost);
        }


        // totalChampionObject에서 제거
        if (user.TotalChampionObject.Contains(targetChampion))
        {
            user.TotalChampionObject.Remove(targetChampion);
        }

        // battleChampionObject에서 제거
        if (user.BattleChampionObject.Contains(targetChampion))
        {
            user.BattleChampionObject.Remove(targetChampion);
        }

        // nonBattleChampionObject에서 제거
        if (user.NonBattleChampionObject.Contains(targetChampion))
        {
            user.NonBattleChampionObject.Remove(targetChampion);
        }
        if (user.ChampionOriginState.ContainsKey(targetChampion))
        {
            user.ChampionOriginState.Remove(targetChampion);
        }

        Manager.Synergy.UpdateSynergies(user);
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
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    {
                        userData.TotalChampionObject.Add(champion);
                        userData.NonBattleChampionObject.Add(champion);
                    }
                }
            }
        }

        foreach (var tileEntry in userData.MapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile)
                {
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    { 
                        userData.TotalChampionObject.Add(champion);
                        userData.BattleChampionObject.Add(champion);
                    }
                }
            }
        }
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
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

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
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    {
                        userData.TotalChampionObject.Add(champion);
                    }
                }
            }
        }
    }
    private void SettingTotalChampion_Battle(UserData userData, UserData enemyData)
    {
        userData.TotalChampionObject.Clear();

        foreach (var tileEntry in enemyData.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile)
                {
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    {
                        userData.TotalChampionObject.Add(champion);
                    }
                }
            }
        }

        // HexDictionary의 타일 처리
        foreach (var tileEntry in enemyData.MapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile)
                {
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
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
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion"))
                    {
                        Debug.Log("논 배틀 추가 진짜");
                        AddNonBattleChampion(userData, champion);
                    }
                }
            }
        }
    }

    public void SettingNonBattleChampion_Battle(UserData userData, UserData enemyData)
    {
        SettingTotalChampion_Battle(userData, enemyData);
        userData.NonBattleChampionObject.Clear();

        foreach (var tileEntry in enemyData.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile)
                {
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    {
                        AddNonBattleChampion(userData, champion);
                    }
                }
            }
        }
    }

    public void AddNonBattleChampion(UserData userData, GameObject champion)
    {
        if (Manager.Stage.IsBattleOngoing)
        {
            userData.MergeChampionQueue.Enqueue(champion);
            return;
        }

        ProcessMerge(userData, champion);
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
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
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


        Manager.Synergy.AddSynergyLine(userData, addChampionBase.ChampionName, Utilities.GetLineName(addChampionBase.ChampionLine_First));
        Manager.Synergy.AddSynergyLine(userData, addChampionBase.ChampionName, Utilities.GetLineName(addChampionBase.ChampionLine_Second));
        Manager.Synergy.AddSynergyJob(userData, addChampionBase.ChampionName, Utilities.GetJobName(addChampionBase.ChampionJob_First));
        Manager.Synergy.AddSynergyJob(userData, addChampionBase.ChampionName, Utilities.GetJobName(addChampionBase.ChampionJob_Second));
    }

    public void RemoveBattleChampion(UserData userData, GameObject champion)
    {

    }
    #endregion

    #region 강화로직

    private void ProcessMerge(UserData userData, GameObject champion)
    {
        userData.NonBattleChampionObject.Add(champion);

        ChampionBase cBase = champion.GetComponent<ChampionBase>();

        // 2성 합성 로직
        int sameChampionCount = userData.TotalChampionObject.Count(obj =>
        {
            ChampionBase championBase = obj.GetComponent<ChampionBase>();
            return championBase != null && cBase != null &&
                   championBase.ChampionName == cBase.ChampionName &&
                   championBase.ChampionLevel == cBase.ChampionLevel;
        });

        GameObject championToEnhance = null;

        if (sameChampionCount >= 3)
        {
            championToEnhance = MergeChampion(userData, champion);

            SettingAllChampion(userData);
        }

        if (championToEnhance == null)
            return;

        // 3성 합성 로직
        ChampionBase tempChampionBase = championToEnhance.GetComponent<ChampionBase>();
        string championName = tempChampionBase.ChampionName;
        int sameChampionAfterMergeCount = userData.TotalChampionObject.Count(obj =>
        {
            ChampionBase championBase = obj.GetComponent<ChampionBase>();
            return championBase != null &&
                   championBase.ChampionName == championName &&
                   championBase.ChampionLevel == 2;
        });

        if (sameChampionAfterMergeCount >= 3)
        {
            championToEnhance = MergeChampion(userData, championToEnhance);
            SettingAllChampion(userData);
        }
    }

    private void ProcessMergeQueue(UserData userData)
    {
        while (userData.MergeChampionQueue.Count > 0)
        {
            GameObject champion = userData.MergeChampionQueue.Dequeue();
            ProcessMerge(userData, champion);
        }
    }

    private GameObject MergeChampion(UserData userData, GameObject champion)
    {
        List<ItemBlueprint> itemList = new List<ItemBlueprint>();

        int countToRemove = 2; 
        GameObject championToEnhance = champion;

        ChampionBase targetChampionBase = championToEnhance.GetComponent<ChampionBase>();
        string targetName = targetChampionBase.ChampionName;
        int targetLevel = targetChampionBase.ChampionLevel;


        // 리스트를 뒤에서부터 순회하며 중복 챔피언 제거
        for (int i = userData.TotalChampionObject.Count - 1; i >= 0 && countToRemove > 0; i--)
        {
            GameObject currentChampionObj = userData.TotalChampionObject[i];
            ChampionBase currentChampionBase = currentChampionObj.GetComponent<ChampionBase>();

            if (currentChampionBase != null
                && currentChampionBase.ChampionName == targetName 
                && currentChampionBase.ChampionLevel == targetLevel)
            {
                if (currentChampionBase == targetChampionBase)
                {
                    continue; 
                }

                itemList.AddRange(currentChampionBase.EquipItem);

                HexTile parentTile = currentChampionObj.GetComponentInParent<HexTile>();
                if (parentTile != null)
                {
                    parentTile.championOnTile.Remove(currentChampionObj);
                }

                Utilities.Destroy(currentChampionObj);
                userData.TotalChampionObject.RemoveAt(i);

                countToRemove--;
            }
        }

        if (itemList.Count > 0)
        {
            Manager.Item.StartCreatingItems(itemList);
        }

        itemList.Clear();
        EnhanceChampion(championToEnhance);
        championToEnhance.GetComponent<ChampionBase>().ChampionFrame.SetChampionLevel();

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

    public void OnBattleEnd(UserData user)
    {
        ProcessMergeQueue(user);
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
[System.Serializable]
public class ChampionOriginalState
{
    public Vector3 originalPosition;
    public Transform originalParent;
    public HexTile originalTile;
    public bool wasActive;
    public MapGenerator.MapInfo originalMapInfo;
}