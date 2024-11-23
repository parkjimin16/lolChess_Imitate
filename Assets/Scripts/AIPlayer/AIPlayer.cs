using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class AIPlayer
{
    private Player aiPlayerComponent; // AI �÷��̾��� Player ������Ʈ
    private UserData aiUserData;      // AI �÷��̾��� UserData
    private GameDataBlueprint gameDataBlueprint;
    private List<string> shopChampionList;

    public Player AiPlayerComponent => aiPlayerComponent;

    public void InitAIPlayer(Player player, GameDataBlueprint gameData)
    {
        aiPlayerComponent = player;
        aiUserData = player.UserData;
        gameDataBlueprint = gameData;
    }


    public void PerformActions(Player aiPlayer)
    {
        BuyChampions(aiPlayer); //è�Ǿ� ����
        // �ʿ信 ���� �߰� �ൿ (������, ���� ��)
        DecideAndPlaceChampions(); // è�Ǿ� ��ġ
    }

    #region è�Ǿ𱸸ŷ���
    private void BuyChampions(Player aiPlayer)
    {
        shopChampionList = Manager.Champion.GetRandomChampions(1);
        string selectedChampionName = shopChampionList[Random.Range(0, shopChampionList.Count)];

        //ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint(selectedChampionName) as ChampionBlueprint;
        ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint("ChampionBlueprint_Zoe") as ChampionBlueprint;

        InstantiateAIChampion(championBlueprint, aiPlayer);
    }

    private void InstantiateAIChampion(ChampionBlueprint cBlueprint, Player aiPlayer)
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
            GameObject frame = Manager.ObjectPool.GetGo("ChampionFrame");
            frame.transform.SetParent(newChampionObject.transform, false);
            newChampionObject.transform.position = emptyTile.transform.position;

            newChampionObject.transform.SetParent(emptyTile.transform);
            emptyTile.championOnTile.Add(newChampionObject);

            ChampionBase cBase = newChampionObject.GetComponent<ChampionBase>();
            ChampionFrame cFrame = frame.GetComponentInChildren<ChampionFrame>();

            cBase.SetChampion(cBlueprint, aiPlayer);
            cBase.InitChampion(cFrame);

            cBase.BattleStageIndex = aiPlayer.UserData.UserId;
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
    #endregion

    #region è�Ǿ� ��ġ ����
    private void DecideAndPlaceChampions()
    {
        // ���� ��Ʋ�ʵ忡 ��ġ�� è�Ǿ� �� Ȯ��
        int currentBattleChampions = aiUserData.CurrentPlaceChampion;

        // AI �÷��̾��� �ִ� ��ġ ���� è�Ǿ� ��
        int maxBattleChampions = aiUserData.MaxPlaceChampion;

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
        Manager.User.ClearSynergy(aiUserData);
        Manager.Champion.SettingAllChampion(aiUserData);
        Manager.Champion.SettingBattleChampion(aiUserData);

        //uiMain?.UISynergyPanel.UpdateSynergy(Manager.User.GetHumanUserData());
    }
    private void AutoPlaceAIChampions()
    {
        // ���� ��Ʋ�ʵ忡 ��ġ�� è�Ǿ� �� Ȯ��
        int currentBattleChampions = aiUserData.CurrentPlaceChampion;

        // AI �÷��̾��� �ִ� ��ġ ���� è�Ǿ� ��
        int maxBattleChampions = aiUserData.MaxPlaceChampion;

        // ��ġ ������ ���� �� ���
        int availableSlots = maxBattleChampions - currentBattleChampions;

        if (availableSlots <= 0)
        {
            // ��ġ ������ ������ ������ ��ȯ
            return;
        }

        // NonBattleChampionObject ����Ʈ���� è�Ǿ��� �����ɴϴ�.
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
            champion.transform.position = emptyTile.transform.position;
            champion.transform.SetParent(emptyTile.transform);

            // Ÿ�� ���� ������Ʈ
            emptyTile.championOnTile.Add(champion);

            // AI �÷��̾��� ����Ʈ ������Ʈ
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

    #region è�Ǿ� ���� ���� ����
    private void ReRollChampions(Player aiPlayer)
    {
        // è�Ǿ� ���� ���� ���: 2���
        int rerollCost = 2;

        if (aiUserData.UserGold >= rerollCost)
        {
            // ��� �Ҹ�
            aiUserData.UserGold -= rerollCost;
            //UIManager.Instance.UpdateUserGoldUI(aiUserData); // UI ������Ʈ (�ʿ� ��)

            // ���ο� è�Ǿ� ����Ʈ ����
            shopChampionList = Manager.Champion.GetRandomChampions(aiUserData.UserLevel);//GetRandomChampions(aiUserData.UserLevel);

            Debug.Log($"AI {aiUserData.UserName}�� 2 ��带 ����Ͽ� è�Ǿ� ������ �����߽��ϴ�. ���� ���: {aiUserData.UserGold}");
        }
        else
        {
            Debug.Log($"AI {aiUserData.UserName}���Դ� è�Ǿ� ������ ������ ����� ��尡 �����ϴ�.");
        }
    }
    #endregion

    #region ����ġ ���� ����
    private void BuyExperience(Player aiPlayer)
    {
        // ����ġ ���� ���: 4���� 4 EXP
        int experienceCost = 4;
        int experienceAmount = 4;

        if (aiUserData.UserGold >= experienceCost)
        {
            // ��� �Ҹ�
            aiUserData.UserGold -= experienceCost;
            //UIManager.Instance.UpdateUserGoldUI(aiUserData); // UI ������Ʈ (�ʿ� ��)

            // ����ġ �߰�
            Manager.Level.AddExperience(aiUserData, experienceAmount);
            // �Ǵ� LevelManager.Instance.AddExperience(aiUserData, experienceAmount);

            Debug.Log($"AI {aiUserData.UserName}�� {experienceCost} ��带 ����Ͽ� {experienceAmount} EXP�� �����߽��ϴ�. ���� ���: {aiUserData.UserGold}");
        }
        else
        {
            Debug.Log($"AI {aiUserData.UserName}���Դ� ����ġ�� ������ ����� ��尡 �����ϴ�.");
        }
    }
    #endregion

    #region AI �ൿ �켱���� �� ���� ����
    /*private bool ShouldBuyExperience()
    {
        // ���� �������� ����ġ�� ���� ������ �� ����
        int nextLevelExp = Manager.Level.GetNextLevelExp(aiUserData);
        return aiUserData.UserExp + 4 >= nextLevelExp; // 4 EXP�� �߰��� �� �������� �� ���
    }*/

    private bool ShouldReRollChampions()
    {
        // ���� ������ �ó����� ������ ���ϴ� ���غ��� ���� ��
        int currentSynergyCount = GetCurrentSynergyCount();
        return currentSynergyCount < desiredSynergyThreshold && aiUserData.UserGold >= 2;
    }

    private int GetCurrentSynergyCount()
    {
        // ���� AI�� è�Ǿ� ���Կ� �ִ� �ó����� ������ ��ȯ
        // ���÷� �ܼ��� ���� �ó��� ���� ��ȯ�ϵ��� ����
        return aiUserData.ChampionSynergies_Line.Count + aiUserData.ChampionSynergies_Job.Count;
    }

    private int desiredSynergyThreshold = 3; // ���ϴ� �ó��� �� ����
    #endregion

}
