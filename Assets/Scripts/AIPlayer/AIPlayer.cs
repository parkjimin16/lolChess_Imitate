using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class AIPlayer
{
    private Player aiPlayerComponent; // AI �÷��̾��� Player ������Ʈ
    private UserData aiUserData;      // AI �÷��̾��� UserData
    private GameDataBlueprint gameDataBlueprint;

    public void InitAIPlayer(Player player, GameDataBlueprint gameData)
    {
        aiPlayerComponent = player;
        aiUserData = player.UserData;
        gameDataBlueprint = gameData;
    }


    public void PerformActions()
    {
        BuyChampions(); //è�Ǿ� ����
        // �ʿ信 ���� �߰� �ൿ (������, ���� ��)
        DecideAndPlaceChampions(); // è�Ǿ� ��ġ
    }

    #region è�Ǿ𱸸ŷ���
    private void BuyChampions()
    {
        //int level = aiUserData.Level;

        // è�Ǿ� ���� ����
        List<string> shopChampionList = GetRandomChampions(1);
        string selectedChampionName = shopChampionList[Random.Range(0, shopChampionList.Count)];
        
        // è�Ǿ� �������Ʈ ��������
        ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint(selectedChampionName) as ChampionBlueprint;

        InstantiateAIChampion(championBlueprint);

        /* ��� üũ �� ����
        int championCost = championBlueprint.ChampionCost;
        if (aiUserData.GetGold() >= championCost)
        {
            aiUserData.SetGold(aiUserData.GetGold() - championCost);

            // è�Ǿ� �ν��Ͻ� ���� �� ��ġ
            InstantiateAIChampion(championBlueprint);
        }*/
    }

    private void InstantiateAIChampion(ChampionBlueprint cBlueprint)
    {
        // AI �÷��̾��� �� ������ �����ɴϴ�.
        MapGenerator.MapInfo aiMapInfo = aiUserData.MapInfo;

        if (aiMapInfo == null)
        {
            Debug.LogWarning($"AI �÷��̾� {aiUserData.UserName}�� MapInfo�� ã�� �� �����ϴ�.");
            return;
        }

        // �� Ÿ�� ã��
        HexTile emptyTile = FindEmptyRectTile(aiMapInfo);

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

            // AI �÷��̾��� è�Ǿ� ����Ʈ�� �߰�
            //aiUserData.NonBattleChampionObject.Add(newChampionObject);
            Manager.Champion.SettingNonBattleChampion(aiUserData);
        }
        else
        {
            Debug.LogWarning($"AI �÷��̾� {aiUserData.UserName}�� �ʿ� �� Ÿ���� �����ϴ�.");
        }
    }

    private HexTile FindEmptyRectTile(MapGenerator.MapInfo mapInfo)
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

    // è�Ǿ� ���� ���� (���� UIShopPanel���� ������ �޼��� Ȱ��)
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

    #region è�Ǿ� ��ġ ����
    private void DecideAndPlaceChampions()
    {
        // ���� ��Ʋ�ʵ忡 ��ġ�� è�Ǿ� �� Ȯ��
        int currentBattleChampions = aiUserData.BattleChampionObject.Count;

        // AI �÷��̾��� ������ ���� �ִ� ��ġ ���� è�Ǿ� ��
        int maxBattleChampions = 1; //aiUserData.Level; // �ӽ÷� 6���� ��

        // ��ġ ������ ���� �� ���
        int availableSlots = maxBattleChampions - currentBattleChampions;

        if (availableSlots <= 0)
        {
            // ��ġ ������ ������ ������ ��ȯ
            return;
        }

        // RectTile�� �ִ� è�Ǿ�� �߿��� ��ġ�� è�Ǿ� ����
        List<GameObject> championsOnBench = new List<GameObject>(aiUserData.NonBattleChampionObject);

        // ��ġ ������ ����ŭ è�Ǿ� ����
        for (int i = 0; i < availableSlots && championsOnBench.Count > 0; i++)
        {
            // ���� è�Ǿ� ����
            int randomIndex = Random.Range(0, championsOnBench.Count);
            GameObject championToPlace = championsOnBench[randomIndex];

            PlaceChampionOnHexTile(championToPlace);
            championsOnBench.RemoveAt(randomIndex);
        }
    }

    private void PlaceChampionOnHexTile(GameObject champion)
    {
        // AI �÷��̾��� �� ������ �����ɴϴ�.
        MapGenerator.MapInfo aiMapInfo = aiUserData.MapInfo;

        if (aiMapInfo == null)
        {
            Debug.LogWarning($"AI �÷��̾� {aiUserData.UserName}�� MapInfo�� ã�� �� �����ϴ�.");
            return;
        }

        // �� HexTile ã��
        HexTile emptyTile = FindEmptyHexTile(aiMapInfo);

        if (emptyTile != null)
        {
            // è�Ǿ��� ���� Ÿ�� ���� ��������
            HexTile currentTile = champion.transform.parent.GetComponent<HexTile>();
            if (currentTile != null)
            {
                currentTile.championOnTile.Remove(champion);
            }

            // è�Ǿ��� ��ġ�� �θ� ������Ʈ
            champion.transform.position = emptyTile.transform.position + new Vector3(0, 0.5f, 0);
            champion.transform.SetParent(emptyTile.transform);

            // Ÿ�� ���� ������Ʈ
            emptyTile.championOnTile.Add(champion);

            // AI �÷��̾��� ����Ʈ ������Ʈ
            //aiUserData.NonBattleChampionObject.Remove(champion);
            //aiUserData.BattleChampionObject.Add(champion);
            Manager.Champion.SettingBattleChampion(aiUserData);
        }
        else
        {
            Debug.LogWarning($"AI �÷��̾� {aiUserData.UserName}�� HexTile�� �� Ÿ���� �����ϴ�.");
        }
    }

    private HexTile FindEmptyHexTile(MapGenerator.MapInfo mapInfo)
    {
        List<HexTile> availableTiles = new List<HexTile>();

        // ���ǿ� �´� Ÿ�ϵ��� ����Ʈ�� �߰��մϴ�.
        foreach (var tileEntry in mapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (!tile.isOccupied && tile.CompareTag("PlayerTile"))
            {
                availableTiles.Add(tile);
            }
        }

        // ����Ʈ�� ������� ������ ������ Ÿ���� ��ȯ�մϴ�.
        if (availableTiles.Count > 0)
        {
            int randomIndex = Random.Range(0, availableTiles.Count);
            return availableTiles[randomIndex];
        }

        // ���ǿ� �´� Ÿ���� ������ null�� ��ȯ�մϴ�.
        return null;
    }
    #endregion

}
