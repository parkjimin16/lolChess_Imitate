using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChampionManager
{
    private GameDataBlueprint gameDataBlueprint;
    private Dictionary<string, int> runtimeChampionCounts = new Dictionary<string, int>();

    #region 초기화
    public void Init(GameDataBlueprint gameDataBlueprint)
    {
        this.gameDataBlueprint = gameDataBlueprint;
        runtimeChampionCounts.Clear();
        InitializeRuntimeChampionCounts();
    }

    private void InitializeRuntimeChampionCounts()
    {
        foreach (var countData in gameDataBlueprint.ChampionMaxCount)
        {
            foreach (var championName in GetChampionNamesByCost(countData.Cost))
            {
                string valueName = GetProcessedChampionName(championName);
                if (!runtimeChampionCounts.ContainsKey(valueName))
                {
                    runtimeChampionCounts[valueName] = countData.MaxCount;
                }
            }
        }
    }



  
    #endregion

    #region 런타임 데이터 제어

    /// <summary>
    /// 챔피언 기물 개수를 줄임.
    /// </summary>
    /// <param name="championName">감소시킬 챔피언 이름</param>
    /// <returns>감소 성공 여부</returns>
    public void DecreaseChampionCount(string championName)
    {
        if (runtimeChampionCounts.ContainsKey(championName) && runtimeChampionCounts[championName] > 0)
        {
            runtimeChampionCounts[championName]--;
        }
    }

    /// <summary>
    /// 챔피언 기물 개수를 늘림.
    /// </summary>
    /// <param name="championName">증가시킬 챔피언 이름</param>
    public void IncreaseChampionCount(string championName)
    {
        if (runtimeChampionCounts.ContainsKey(championName))
        {
            runtimeChampionCounts[championName]++;
        }
    }

    /// <summary>
    /// 챔피언 기물이 남아있는지 확인
    /// </summary>
    /// <param name="championName">확인할 챔피언 이름</param>
    /// <returns>기물이 남아있으면 true</returns>
    public bool HasAvailableChampion(string championName)
    {
        return runtimeChampionCounts.ContainsKey(championName) && runtimeChampionCounts[championName] > 0;
    }

    #endregion


    #region 챔피언 상점 로직
    public string GetProcessedChampionName(string championName)
    {
        int underscoreIndex = championName.IndexOf('_');

        if (underscoreIndex >= 0)
        {
            return championName.Substring(underscoreIndex + 1);
        }

        return championName;
    }
    private string[] GetChampionNamesByCost(int cost)
    {
        foreach (var championData in gameDataBlueprint.ChampionDataList)
        {
            if (championData.Cost == cost)
            {
                return championData.Names;
            }
        }
        return new string[0];
    }


    /// <summary>
    /// 상점 챔피언 갱신
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public List<string> GetRandomChampions(int level)
    {
        List<string> selectedChampions = new List<string>();

        ChampionRandomData currentData = gameDataBlueprint.ChampionRandomDataList[level - 1];

        while (selectedChampions.Count < 5)
        {
            int costIndex = GetCostIndex(currentData.Probability);
            int cost = costIndex + 1;

            ChampionData costChampionData = GetChampionDataByCost(cost);
            if (costChampionData != null && costChampionData.Names.Length > 0)
            {
                string candidate = costChampionData.Names[UnityEngine.Random.Range(0, costChampionData.Names.Length)];
                string valueName = GetProcessedChampionName(candidate);
                // 기물이 남아있는 챔피언만 추가
                if (HasAvailableChampion(valueName) && !selectedChampions.Contains(candidate))
                {
                    selectedChampions.Add(candidate);
                }
            }
        }

        if (selectedChampions.Count < 5)
        {
            Debug.LogWarning("Unable to fill all champion slots. Check available data and probabilities.");
        }


        /*
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
        */
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

        float randomValue = UnityEngine.Random.Range(0f, 1f);

        for (int i = 0; i < cumulativeProbabilities.Length; i++)
        {
            if (randomValue < cumulativeProbabilities[i])
            {
                return i;
            }
        }

        return probabilities.Length - 1;
    }
    #endregion

    #region 챔피언 로직




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


    /// <summary>
    /// 랜덤 챔피언 이름 가져오기
    /// </summary>
    /// <param name="cost"></param>
    /// <returns></returns>
    public string GetRandomChapmion(int cost)
    {
        string newChampion = string.Empty;

        int count =  gameDataBlueprint.ChampionDataList[cost - 1].Names.Length;
        int randomIndex = UnityEngine.Random.Range(0, count);
        newChampion = gameDataBlueprint.ChampionDataList[cost - 1].Names[randomIndex];

        return newChampion;
    }

    /// <summary>
    /// 챔피언 생성
    /// </summary>
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
            Player enemy = Manager.Stage.FindMyEnemy(user).GetComponent<Player>();

            if (enemy != null)
            {
                Manager.Champion.SettingNonBattleChampion_MoveUser(user, enemy.UserData);
            }
        }
        else
        {
            SettingNonBattleChampion(user);
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

            user.ChampionOriginState[newChampionObject] = originalState;
        }
    }

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


    /// <summary>
    /// 챔피언 제거
    /// </summary>
    /// <param name="user"></param>
    /// <param name="targetChampion"></param>
    public void RemoveChampion(UserData user, GameObject targetChampion)
    {
        ChampionBase cBase = targetChampion.GetComponent<ChampionBase>();

        if (cBase.EquipItem.Count > 0)
        {
            GameObject obj = Manager.ObjectPool.GetGo("Capsule");
            obj.transform.position = user.MapInfo.mapTransform.position + new Vector3(0, 1, 0);
            Capsule cap = obj.GetComponent<Capsule>();
            HexTile capHextile = new HexTile();
            user.MapInfo.HexDictionary.TryGetValue((3, 3), out capHextile);
            obj.transform.position = capHextile.transform.position + new Vector3(0, 1f, 0);
            obj.transform.SetParent(capHextile.transform);
            capHextile.capsuleOnTile.Add(obj);

            List<string> item = new List<string>();
            List<string> champion = new List<string>();

            for (int i = 0; i < cBase.EquipItem.Count; i++)
            {
                item.Add(cBase.EquipItem[i].ItemId);
                user.TotalItemBlueprint.Remove(cBase.EquipItem[i]);
            }

            cap.InitCapsule(cBase.Player.UserData, cBase.ChampionSellCost(Utilities.SetSlotCost(cBase.ChampionCost), cBase.ChampionLevel), item, champion);
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

        IncreaseChampionCount(cBase.ChampionBlueprint.ChampionInstantiateName);

        Manager.Synergy.UpdateSynergies(user);
    }


    #endregion





    #region 챔피언 구매 로직

    #region 공통 로직

    #region 전투 챔피언

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
        if (!userData.BattleChampionObject.Contains(champion))
            userData.BattleChampionObject.Add(champion);


        ChampionBase addChampionBase = champion.GetComponent<ChampionBase>();

        if (addChampionBase == null || userData.BattleChampionObject == null)
            return;


        Manager.Synergy.AddSynergyLine(userData, addChampionBase.ChampionName, Utilities.GetLineName(addChampionBase.ChampionLine_First));
        Manager.Synergy.AddSynergyLine(userData, addChampionBase.ChampionName, Utilities.GetLineName(addChampionBase.ChampionLine_Second));
        Manager.Synergy.AddSynergyJob(userData, addChampionBase.ChampionName, Utilities.GetJobName(addChampionBase.ChampionJob_First));
        Manager.Synergy.AddSynergyJob(userData, addChampionBase.ChampionName, Utilities.GetJobName(addChampionBase.ChampionJob_Second));
    }

    private List<GameObject> GetBattleChampion(List<GameObject> total, List<GameObject> none)
    {
        List<GameObject> result = total
       .Where(champion => !none.Contains(champion))
       .ToList();

        return result;
    }

    #endregion

    #endregion

    #region 상대가 넘어왔을 때 + 크립 전투

    #region 전체 챔피언
    public void SettingAllChampion(UserData userData)
    {
        userData.TotalChampionObject.Clear();
        userData.NonBattleChampionObject.Clear();
        userData.BattleChampionObject.Clear();

        foreach (var tileEntry in userData.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile.ToList())
                {
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    {
                        if (!userData.TotalChampionObject.Contains(champion))
                            userData.TotalChampionObject.Add(champion);


                        if (!userData.NonBattleChampionObject.Contains(champion))
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
                foreach (GameObject champion in tile.championOnTile.ToList())
                {
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    {
                        if (!userData.TotalChampionObject.Contains(champion))
                            userData.TotalChampionObject.Add(champion);


                        if (!userData.BattleChampionObject.Contains(champion))
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
                foreach (GameObject champion in tile.championOnTile.ToList())
                {
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    {
                        if (!userData.TotalChampionObject.Contains(champion))
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
                foreach (GameObject champion in tile.championOnTile.ToList())
                {
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    {
                        if (!userData.TotalChampionObject.Contains(champion))
                            userData.TotalChampionObject.Add(champion);
                    }
                }
            }
        }
    }

    #endregion

    #region 비전투 챔피언

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
                foreach (GameObject champion in tile.championOnTile.ToList())
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

        userData.BattleChampionObject.Clear();
        userData.BattleChampionObject = GetBattleChampion(userData.TotalChampionObject, userData.NonBattleChampionObject);
    }

    private void AddNonBattleChampion(UserData userData, GameObject champion)
    {
        if (!userData.NonBattleChampionObject.Contains(champion))
            userData.NonBattleChampionObject.Add(champion);




        if (Manager.Stage.IsBattleOngoing)
        {
            UserChampionMerge_NonBattle(userData);
        }
        else if (!Manager.Stage.IsBattleOngoing)
        {
            UserChampionMerge_Total(userData);
        }
    }

    #endregion

    #endregion

    #region 유저가 넘어갔을 때

    #region 전체 챔피언

    private void SettingAllChampion_MoveUser(UserData userData, UserData enemyData)
    {
        userData.TotalChampionObject.Clear();
        userData.NonBattleChampionObject.Clear();
        userData.BattleChampionObject.Clear();

        foreach (var tileEntry in enemyData.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile.ToList())
                {
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    {
                        if (!userData.TotalChampionObject.Contains(champion))
                            userData.TotalChampionObject.Add(champion);


                        if (!userData.NonBattleChampionObject.Contains(champion))
                            userData.NonBattleChampionObject.Add(champion);
                    }
                }
            }
        }

        foreach (var tileEntry in enemyData.MapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile.ToList())
                {
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    {
                        if (!userData.TotalChampionObject.Contains(champion))
                            userData.TotalChampionObject.Add(champion);


                        if (!userData.BattleChampionObject.Contains(champion))
                            userData.BattleChampionObject.Add(champion);
                    }
                }
            }
        }
    }
    private void SettingTotalChampion_MoveUser(UserData userData, UserData enemyData)
    {
        userData.TotalChampionObject.Clear();

        foreach (var tileEntry in enemyData.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile.ToList())
                {
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    {
                        if (!userData.TotalChampionObject.Contains(champion))
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
                foreach (GameObject champion in tile.championOnTile.ToList())
                {
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    {
                        if (!userData.TotalChampionObject.Contains(champion))
                            userData.TotalChampionObject.Add(champion);
                    }
                }
            }
        }
    }

    #endregion

    #region 비전투챔피언

    private void SettingNonBattleChampion_MoveUser(UserData userData, UserData enemyData)
    {
        SettingTotalChampion_MoveUser(userData, enemyData);
        userData.NonBattleChampionObject.Clear();

        foreach (var tileEntry in enemyData.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (tile.isOccupied)
            {
                foreach (GameObject champion in tile.championOnTile.ToList())
                {
                    if (champion == null || !champion.activeInHierarchy)
                        continue;

                    if (champion.CompareTag("Champion") && champion.GetComponent<ChampionBase>().Player.UserData == userData)
                    {
                        AddNonBattleChampion_MoveUser(userData, enemyData, champion);
                    }
                }
            }
        }

        userData.BattleChampionObject.Clear();
        userData.BattleChampionObject = GetBattleChampion(userData.TotalChampionObject, userData.NonBattleChampionObject);
    }

    private void AddNonBattleChampion_MoveUser(UserData userData, UserData enemyData, GameObject champion)
    {
        if (!userData.NonBattleChampionObject.Contains(champion))
            userData.NonBattleChampionObject.Add(champion);


        if (Manager.Stage.IsBattleOngoing)
        {
            UserChampionMerge_NonBattle_MoveUser(userData, enemyData);
        }
        else if (!Manager.Stage.IsBattleOngoing)
        {
            UserChampionMerge_Total_MoveUser(userData, enemyData);
        }

    }
    #endregion

    #endregion


    #endregion



    #region 챔피언 강화 로직

    #region 강화 공통 로직

    private Dictionary<string, List<GameObject>> GroupChampionsByMerge_TotalChampion(UserData userData)
    {
        var groupedChampions = new Dictionary<string, List<GameObject>>();

        foreach (var champion in userData.TotalChampionObject)
        {
            var championBase = champion.GetComponent<ChampionBase>();
            if (championBase == null)
            {
                continue;
            }

            string key = $"{championBase.ChampionName}_{championBase.ChampionLevel}";

            if (!groupedChampions.ContainsKey(key))
            {
                groupedChampions[key] = new List<GameObject>();
            }

            groupedChampions[key].Add(champion);
        }

        return groupedChampions;
    }

    private Dictionary<string, List<GameObject>> GroupChampionsByMerge_NonBattleChampion(UserData userData)

    {
        var groupedChampions = new Dictionary<string, List<GameObject>>();

        foreach (var champion in userData.NonBattleChampionObject)
        {
            var championBase = champion.GetComponent<ChampionBase>();
            if (championBase == null)
            {
                continue;
            }

            string key = $"{championBase.ChampionName}_{championBase.ChampionLevel}";

            if (!groupedChampions.ContainsKey(key))
            {
                groupedChampions[key] = new List<GameObject>();
            }

            groupedChampions[key].Add(champion);
        }

        return groupedChampions;
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
            Manager.Item.StartCreatingItems(userData, itemList);
        }

        itemList.Clear();
        EnhanceChampion(targetChampionBase);
        championToEnhance.GetComponent<ChampionBase>().ChampionFrame.SetChampionLevel();

        return championToEnhance;
    }

    private GameObject MergeChampion_None(UserData userData, GameObject champion)
    {
        List<ItemBlueprint> itemList = new List<ItemBlueprint>();

        int countToRemove = 2;
        GameObject championToEnhance = champion;

        ChampionBase targetChampionBase = championToEnhance.GetComponent<ChampionBase>();
        string targetName = targetChampionBase.ChampionName;
        int targetLevel = targetChampionBase.ChampionLevel;


        // 리스트를 뒤에서부터 순회하며 중복 챔피언 제거
        for (int i = userData.NonBattleChampionObject.Count - 1; i >= 0 && countToRemove > 0; i--)
        {
            GameObject currentChampionObj = userData.NonBattleChampionObject[i];
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
                userData.NonBattleChampionObject.RemoveAt(i);

                countToRemove--;
            }
        }

        if (itemList.Count > 0)
        {
            Manager.Item.StartCreatingItems(userData, itemList);
        }

        itemList.Clear();
        EnhanceChampion(targetChampionBase);
        championToEnhance.GetComponent<ChampionBase>().ChampionFrame.SetChampionLevel();

        return championToEnhance;
    }

    private void EnhanceChampion(ChampionBase cBase)
    {
        cBase.ChampionLevelUp();
    }

    #endregion

    #region 상대가 넘어왔을 때_강화

    public void UserChampionMerge_Total(UserData user)
    {
        var list = GroupChampionsByMerge_TotalChampion(user);

        foreach (var mergeList in list)
        {
            List<GameObject> champions = mergeList.Value;
            ProcessMerge_Total(user, champions);
        }
    }

    public void UserChampionMerge_NonBattle(UserData user)
    {
        var list = GroupChampionsByMerge_NonBattleChampion(user);

        foreach (var mergeList in list)
        {
            List<GameObject> champions = mergeList.Value;
            ProcessMerge_None(user, champions);
        }
    }

    private void ProcessMerge_Total(UserData userData, List<GameObject> championsToMerge)
    {
        if (championsToMerge == null || championsToMerge.Count < 3)
            return;

        var baseChampion = championsToMerge[0];

        if (championsToMerge.Count >= 3)
        {
            GameObject enhancedChampion = MergeChampion(userData, baseChampion);
            SettingAllChampion(userData);
            CheckAndProcessThirdStarMerge(userData, enhancedChampion);
        }

    }

    private void ProcessMerge_None(UserData userData, List<GameObject> championsToMerge)
    {
        if (championsToMerge == null || championsToMerge.Count < 3)
            return;

        List<GameObject> filteredChampions = championsToMerge
         .Where(champion => userData.NonBattleChampionObject.Contains(champion))
         .ToList();

        if (filteredChampions.Count == 3)
        {
            var baseChampion = filteredChampions[0];
            GameObject enhancedChampion = MergeChampion_None(userData, baseChampion);
            SettingAllChampion(userData);
            CheckAndProcessThirdStarMerge_None(userData, enhancedChampion);
        }

    }


    private void CheckAndProcessThirdStarMerge(UserData userData, GameObject championToEnhance)
    {
        if (championToEnhance == null)
            return;

        ChampionBase enhancedChampionBase = championToEnhance.GetComponent<ChampionBase>();
        if (enhancedChampionBase == null)
            return;

        string championName = enhancedChampionBase.ChampionName;
        int targetLevel = 2;

        // 2성 챔피언이 3개 이상인지 확인
        var eligibleChampions = userData.TotalChampionObject
         .Where(obj =>
         {
             ChampionBase champBase = obj.GetComponent<ChampionBase>();
             return champBase != null &&
                    champBase.ChampionName == championName &&
                    champBase.ChampionLevel == targetLevel;
         })
         .ToList();

        if (eligibleChampions.Count >= 3)
        {
            MergeChampion(userData, eligibleChampions[0]);
            SettingAllChampion(userData);
        }


    }

    private void CheckAndProcessThirdStarMerge_None(UserData userData, GameObject championToEnhance)
    {
        if (championToEnhance == null)
            return;

        ChampionBase enhancedChampionBase = championToEnhance.GetComponent<ChampionBase>();
        if (enhancedChampionBase == null)
            return;

        string championName = enhancedChampionBase.ChampionName;
        int targetLevel = 2;

        // 2성 챔피언이 3개 이상인지 확인
        var eligibleChampions = userData.NonBattleChampionObject
         .Where(obj =>
         {
             ChampionBase champBase = obj.GetComponent<ChampionBase>();
             return champBase != null &&
                    champBase.ChampionName == championName &&
                    champBase.ChampionLevel == targetLevel;
         })
         .ToList();

        if (eligibleChampions.Count >= 3)
        {
            MergeChampion_None(userData, eligibleChampions[0]);
            SettingAllChampion(userData);
        }
    }

    #endregion

    #region 유저가 넘어갔을 때_강화
    public void UserChampionMerge_Total_MoveUser(UserData user, UserData enemyData)
    {
        var list = GroupChampionsByMerge_TotalChampion(user);

        foreach (var mergeList in list)
        {
            List<GameObject> champions = mergeList.Value;
            ProcessMerge_Total_MoveUser(user, enemyData, champions);
        }
    }

    private void UserChampionMerge_NonBattle_MoveUser(UserData user, UserData enemyData)
    {
        var list = GroupChampionsByMerge_NonBattleChampion(user);

        foreach (var mergeList in list)
        {
            List<GameObject> champions = mergeList.Value;
            ProcessMerge_None_MoveUser(user, enemyData, champions);
        }
    }


    private void ProcessMerge_Total_MoveUser(UserData userData, UserData enemyData, List<GameObject> championsToMerge)
    {
        if (championsToMerge == null || championsToMerge.Count < 3)
            return;

            var baseChampion = championsToMerge[0];

            if (championsToMerge.Count >= 3)
            {
                GameObject enhancedChampion = MergeChampion(userData, baseChampion);
                SettingAllChampion_MoveUser(userData, enemyData);
                CheckAndProcessThirdStarMerge_MoveUser(userData, enemyData, enhancedChampion);
            }
    }
    private void ProcessMerge_None_MoveUser(UserData userData, UserData enemyData, List<GameObject> championsToMerge)
    {
        if (championsToMerge == null || championsToMerge.Count < 3)
            return;

        List<GameObject> filteredChampions = championsToMerge
           .Where(champion => !userData.BattleChampionObject.Contains(champion))
           .ToList();

        if (filteredChampions.Count >= 3)
        {
            var baseChampion = filteredChampions[0];
            GameObject enhancedChampion = MergeChampion_None(userData, baseChampion);
            SettingAllChampion_MoveUser(userData, enemyData);
            CheckAndProcessThirdStarMerge_MoveUser(userData, enemyData, enhancedChampion);
        }
    }
    private void CheckAndProcessThirdStarMerge_MoveUser(UserData userData, UserData enemyData, GameObject championToEnhance)
    {
        if (championToEnhance == null)
            return;

        ChampionBase enhancedChampionBase = championToEnhance.GetComponent<ChampionBase>();
        if (enhancedChampionBase == null)
            return;

        string championName = enhancedChampionBase.ChampionName;
        int targetLevel = 2;

        // 2성 챔피언이 3개 이상인지 확인
        var eligibleChampions = userData.TotalChampionObject
         .Where(obj =>
         {
             ChampionBase champBase = obj.GetComponent<ChampionBase>();
             return champBase != null &&
                    champBase.ChampionName == championName &&
                    champBase.ChampionLevel == targetLevel;
         })
         .ToList();

        if (eligibleChampions.Count >= 3)
        {
            MergeChampion_None(userData, eligibleChampions[0]);
            SettingAllChampion_MoveUser(userData, enemyData);
        }
    }
    #endregion


    #endregion
}



#region 클래스

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

#endregion