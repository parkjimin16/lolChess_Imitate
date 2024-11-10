using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class AIPlayer
{
    private Player aiPlayerComponent; // AI 플레이어의 Player 컴포넌트
    private UserData aiUserData;      // AI 플레이어의 UserData
    private GameDataBlueprint gameDataBlueprint;

    public void InitAIPlayer(Player player, GameDataBlueprint gameData)
    {
        aiPlayerComponent = player;
        aiUserData = player.UserData;
        gameDataBlueprint = gameData;
    }


    public void PerformActions()
    {
        BuyChampions();
        // 필요에 따라 추가 행동 (레벨업, 리롤 등)
    }

    #region 챔피언구매로직
    private void BuyChampions()
    {
        //int level = aiUserData.Level;

        // 챔피언 구매 로직
        List<string> shopChampionList = GetRandomChampions(1);
        string selectedChampionName = shopChampionList[Random.Range(0, shopChampionList.Count)];
        
        // 챔피언 블루프린트 가져오기
        ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint(selectedChampionName) as ChampionBlueprint;

        InstantiateAIChampion(championBlueprint);

        // 골드 체크 후 구매
        /*int championCost = championBlueprint.ChampionCost;
        if (aiUserData.GetGold() >= championCost)
        {
            aiUserData.SetGold(aiUserData.GetGold() - championCost);

            // 챔피언 인스턴스 생성 및 배치
            InstantiateAIChampion(championBlueprint);
        }*/
    }

    private void InstantiateAIChampion(ChampionBlueprint cBlueprint)
    {
        // AI 플레이어의 맵 정보를 가져옵니다.
        MapGenerator.MapInfo aiMapInfo = aiUserData.MapInfo;

        if (aiMapInfo == null)
        {
            Debug.LogWarning($"AI 플레이어 {aiUserData.UserName}의 MapInfo를 찾을 수 없습니다.");
            return;
        }

        // 빈 타일 찾기
        HexTile emptyTile = FindEmptyTile(aiMapInfo);

        if (emptyTile != null)
        {
            GameObject newChampionObject = Manager.Asset.InstantiatePrefab(cBlueprint.ChampionInstantiateName);
            newChampionObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            GameObject frame = Manager.Asset.InstantiatePrefab("ChampionFrame");
            frame.transform.SetParent(newChampionObject.transform, false);
            newChampionObject.transform.position = emptyTile.transform.position + new Vector3(0, 0.5f, 0);

            newChampionObject.transform.SetParent(emptyTile.transform);
            emptyTile.championOnTile.Add(newChampionObject);

            ChampionBase cBase = newChampionObject.GetComponent<ChampionBase>();
            ChampionFrame cFrame = frame.GetComponentInChildren<ChampionFrame>();

            cBase.SetChampion(cBlueprint);
            cBase.InitChampion(cFrame);

            //Manager.Champion.SettingNonBattleChampion(Manager.User.User1_Data);

            // AI 플레이어의 챔피언 리스트에 추가
            aiUserData.NonBattleChampionObject.Add(newChampionObject);
        }
        else
        {
            Debug.LogWarning($"AI 플레이어 {aiUserData.UserName}의 맵에 빈 타일이 없습니다.");
        }
    }

    private HexTile FindEmptyTile(MapGenerator.MapInfo mapInfo)
    {
        foreach (var tileEntry in mapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (!tile.isOccupied)
            {
                return tile;
            }
        }
        return null;
    }

    // 챔피언 선택 로직 (기존 UIShopPanel에서 가져온 메서드 활용)
    private List<string> GetRandomChampions(int level)
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
    #endregion


}
