using System;
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

    private List<string> chosenSynergies;

    public Player AiPlayerComponent => aiPlayerComponent;

    public void InitAIPlayer(Player player, GameDataBlueprint gameData)
    {
        aiPlayerComponent = player;
        aiUserData = player.UserData;
        gameDataBlueprint = gameData;

        chosenSynergies = new List<string>();
        ChooseSynergies();
    }


    public void PerformActions(Player aiPlayer)
    {
        if (ShouldLevelUp())
        {
            BuyExperience(aiPlayer);
        }

        // ���� ���� ���� �� ���� ����
        if (ShouldReRollChampions())
        {
            ReRollChampions(aiPlayer);
        }
        // �ʿ信 ���� �߰� �ൿ (������, ���� ��)
        BuyChampions(aiPlayer); // è�Ǿ� ����
        DecideAndPlaceChampions(); // è�Ǿ� ��ġ
        CollectCapsules(aiPlayer);
        UseItems();
    }

    #region è�Ǿ� ���� ����
    private void BuyChampions(Player aiPlayer)
    {
        int maxRerollAttempts = 5; // �ִ� ���� �õ� Ƚ��
        int rerollAttempts = 0;

        while (rerollAttempts <= maxRerollAttempts)
        {
            // �������� è�Ǿ� ����� �����ɴϴ�.
            shopChampionList = Manager.Champion.GetRandomChampions(aiUserData.UserLevel);

            // �ó����� �´� è�Ǿ��� ã���ϴ�.
            List<string> preferredChampions = FindPreferredChampions();

            if (preferredChampions.Count > 0)
            {
                // �ó����� �´� è�Ǿ��� ������ ����
                string selectedChampionName = preferredChampions[UnityEngine.Random.Range(0, preferredChampions.Count)];
                ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint(selectedChampionName) as ChampionBlueprint;

                int championCost = Utilities.SetSlotCost(championBlueprint.ChampionCost);

                if (championBlueprint != null && aiUserData.UserGold >= championCost)
                {
                    aiUserData.UserGold -= championCost;
                    InstantiateAIChampion(championBlueprint, aiPlayer);
                    Debug.Log($"AI {aiUserData.UserName}�� {championCost} ��带 ����Ͽ� �ó����� �´� è�Ǿ� {selectedChampionName}�� �����߽��ϴ�. ���� ���: {aiUserData.UserGold}");
                    break; // ���� �� �ݺ��� ����
                }
            }
            else
            {
                // �ó����� �´� è�Ǿ��� ������ ���� ���� ����
                if (ShouldReRollChampions() && aiUserData.UserGold >= 2)
                {
                    ReRollChampions(aiPlayer);
                    rerollAttempts++;
                }
                else
                {
                    // �������� �ʰų� ��尡 �����ϸ� �������� �ʰ� ����
                    Debug.Log($"AI {aiUserData.UserName}�� �ó����� �´� è�Ǿ��� ã�� ���߽��ϴ�.");
                    break;
                }
            }
        }
    }

    private List<string> FindPreferredChampions()
    {
        List<string> preferredChampions = new List<string>();

        foreach (string championName in shopChampionList)
        {
            ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint(championName) as ChampionBlueprint;

            if (championBlueprint != null)
            {
                bool matchesSynergy = false;

                // è�Ǿ��� ���� �ó��� Ȯ��
                if (championBlueprint.ChampionLine_First != ChampionLine.None &&
                    chosenSynergies.Contains(championBlueprint.ChampionLine_First.ToString()))
                {
                    matchesSynergy = true;
                }
                else if (championBlueprint.ChampionLine_Second != ChampionLine.None &&
                    chosenSynergies.Contains(championBlueprint.ChampionLine_Second.ToString()))
                {
                    matchesSynergy = true;
                }

                // è�Ǿ��� ���� �ó��� Ȯ��
                if (championBlueprint.ChampionJob_First != ChampionJob.None &&
                    chosenSynergies.Contains(championBlueprint.ChampionJob_First.ToString()))
                {
                    matchesSynergy = true;
                }
                else if (championBlueprint.ChampionJob_Second != ChampionJob.None &&
                    chosenSynergies.Contains(championBlueprint.ChampionJob_Second.ToString()))
                {
                    matchesSynergy = true;
                }

                if (matchesSynergy)
                {
                    preferredChampions.Add(championName);
                }
            }
        }

        return preferredChampions;
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

    private void ChooseSynergies()
    {
        // ���ӿ� �����ϴ� ��� �ó��� ����� �����ɴϴ�.
        List<string> allSynergies = GetAllAvailableSynergies();

        // �����ϰ� �ó��� �� ���� �����մϴ�.
        int numberOfSynergiesToChoose = 3; // ���ϴ� �ó��� ����
        chosenSynergies = new List<string>();

        for (int i = 0; i < numberOfSynergiesToChoose; i++)
        {
            if (allSynergies.Count == 0)
                break;

            int randomIndex = UnityEngine.Random.Range(0, allSynergies.Count);
            chosenSynergies.Add(allSynergies[randomIndex]);
            allSynergies.RemoveAt(randomIndex);
        }

        Debug.Log($"AI {aiUserData.UserName}�� ������ �ó���: {string.Join(", ", chosenSynergies)}");
    }
    private List<string> GetAllAvailableSynergies()
    {
        List<string> allSynergies = new List<string>();

        // ��� �ó��� �̸��� �����ɴϴ�.
        // ChampionLine�� ChampionJob enum�� ��� ���� ���ڿ��� ��ȯ�Ͽ� ����Ʈ�� �߰��մϴ�.

        foreach (ChampionLine line in Enum.GetValues(typeof(ChampionLine)))
        {
            if (line != ChampionLine.None)
                allSynergies.Add(line.ToString());
        }

        foreach (ChampionJob job in Enum.GetValues(typeof(ChampionJob)))
        {
            if (job != ChampionJob.None)
                allSynergies.Add(job.ToString());
        }

        return allSynergies;
    }

    #endregion

    #region è�Ǿ� ��ġ ����
    private void DecideAndPlaceChampions()
    {
        // ��ġ ������ ���� �� ���
        int availableSlots = aiUserData.MaxPlaceChampion - aiUserData.CurrentPlaceChampion;

        if (availableSlots <= 0)
        {
            return;
        }

        // RectTile�� �ִ� è�Ǿ�� �߿��� ��ġ�� è�Ǿ� ����
        List<GameObject> championsOnBench = new List<GameObject>(aiUserData.NonBattleChampionObject);

        // �ó����� �´� è�Ǿ���� �켱������ ��ġ
        championsOnBench.Sort((a, b) =>
        {
            int aScore = GetChampionSynergyScore(a);
            int bScore = GetChampionSynergyScore(b);
            return bScore.CompareTo(aScore); // �������� ����
        });

        // ��ġ ������ ����ŭ è�Ǿ� ����
        for (int i = 0; i < availableSlots && championsOnBench.Count > 0; i++)
        {
            GameObject championToPlace = championsOnBench[0];
            PlaceChampionOnHexTile(championToPlace);
            championsOnBench.RemoveAt(0);
        }

        Manager.User.ClearSynergy(aiUserData);
        Manager.Champion.SettingAllChampion(aiUserData);
        Manager.Champion.SettingBattleChampion(aiUserData);
    }
    private int GetChampionSynergyScore(GameObject championObj)
    {
        ChampionBase championBase = championObj.GetComponent<ChampionBase>();
        int score = 0;

        if (championBase != null)
        {
            if (chosenSynergies.Contains(championBase.ChampionLine_First.ToString()))
                score += 1;
            if (chosenSynergies.Contains(championBase.ChampionLine_Second.ToString()))
                score += 1;
            if (chosenSynergies.Contains(championBase.ChampionJob_First.ToString()))
                score += 1;
            if (chosenSynergies.Contains(championBase.ChampionJob_Second.ToString()))
                score += 1;
        }

        return score;
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
            int randomIndex = UnityEngine.Random.Range(0, championsOnBench.Count);
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
            Manager.Champion.SettingAllChampion(aiUserData);
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
            int randomIndex = UnityEngine.Random.Range(0, availableTiles.Count);
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

    #region ĸ�� Ž�� ����
    private void CollectCapsules(Player aiPlayer)
    {
        // AI �÷��̾��� �� ������ �����ɴϴ�.
        MapInfo aiMapInfo = aiPlayer.UserData.MapInfo;

        if (aiMapInfo == null)
        {
            Debug.LogWarning($"AI �÷��̾� {aiPlayer.UserData.UserName}�� MapInfo�� ã�� �� �����ϴ�.");
            return;
        }

        // ���� ��� HexTile���� ĸ���� �����մϴ�.
        List<GameObject> capsulesToCollect = new List<GameObject>();

        foreach (var tileEntry in aiMapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;

            if (tile.capsuleOnTile != null && tile.capsuleOnTile.Count > 0)
            {
                capsulesToCollect.AddRange(tile.capsuleOnTile);
            }
        }

        // ĸ���� ������ ������ �����մϴ�.
        if (capsulesToCollect.Count > 0)
        {
            // ĸ���� ������� ó���ϱ� ���� ť�� �ֽ��ϴ�.
            Queue<GameObject> capsuleQueue = new Queue<GameObject>(capsulesToCollect);

            // ĸ���� �ϳ��� ó���ϴ� �ڷ�ƾ�� �����մϴ�.
            aiPlayer.StartCoroutine(PickUpCapsulesSequentially(capsuleQueue, aiPlayer));
        }
    }
    private IEnumerator PickUpCapsulesSequentially(Queue<GameObject> capsuleQueue, Player aiPlayer)
    {
        while (capsuleQueue.Count > 0)
        {
            GameObject capsule = capsuleQueue.Dequeue();

            if (capsule == null)
                continue;

            // ĸ���� ��ġ�� �̵��մϴ�.
            yield return aiPlayer.StartCoroutine(MoveToPosition(aiPlayer.transform, capsule.transform.position));

            // ĸ���� �浹�Ͽ� �������� ȹ���մϴ�.
            // �浹 ó���� OnTriggerEnter �Ǵ� OnCollisionEnter�� ���� ó���Ǿ��ٰ� �����մϴ�.
            // ���� �׷��� �ʴٸ�, ���⼭ ĸ���� ������ ȹ�� ������ ���� ȣ���ؾ� �մϴ�.

            // ��� ����Ͽ� ���� ĸ���� �̵��ϱ� ���� �ð��� �ݴϴ�.
            yield return new WaitForSeconds(0.2f);
        }
    }
    private IEnumerator MoveToPosition(Transform aiTransform, Vector3 targetPosition)
    {
        float moveSpeed = 5f; // �̵� �ӵ� ����

        while (Vector3.Distance(aiTransform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - aiTransform.position).normalized;
            aiTransform.position = Vector3.MoveTowards(aiTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            aiTransform.rotation = Quaternion.LookRotation(direction);
            yield return null;
        }
    }
    #endregion

    #region ������ ��� ����
    private void UseItems()
    {
        // AI �÷��̾��� ������ ����Ʈ�� �����ɴϴ�.
        List<GameObject> items = new List<GameObject>(aiUserData.UserItemObject);

        // �������� ���� ��� ���
        while (items.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, items.Count);
            GameObject itemObj = items[randomIndex]; // ����Ʈ�� ù ��° ������
            ItemFrame itemFrame = itemObj.GetComponent<ItemFrame>();

            if (itemFrame != null)
            {
                // ������ �������Ʈ ��������
                ItemBlueprint itemBlueprint = itemFrame.ItemBlueprint;

                // �������� è�Ǿ𿡰� �ݴϴ�.
                GiveItemToRandomChampion(itemBlueprint);

                itemObj.transform.parent.GetComponent<HexTile>().isItemTile = false;
                itemObj.transform.parent.GetComponent<HexTile>().itemOnTile = null;

                // ������ ������Ʈ�� �����ϰ� ����Ʈ���� ����
                GameObject.Destroy(itemObj);
                aiUserData.UserItemObject.Remove(itemObj);
                items.RemoveAt(0);
            }
            else
            {
                items.RemoveAt(0);
            }
        }
    }
    private void GiveItemToRandomChampion(ItemBlueprint itemBlueprint)
    {
        // AI �÷��̾��� BattleChampionObject ����Ʈ�� �����ɴϴ�.
        List<GameObject> battleChampions = aiUserData.BattleChampionObject;

        if (battleChampions.Count == 0)
        {
            Debug.Log($"AI �÷��̾� {aiUserData.UserName}�� ��Ʋ è�Ǿ��� �����ϴ�.");
            return;
        }

        // ������ è�Ǿ� ����
        int randomIndex = UnityEngine.Random.Range(0, battleChampions.Count);
        GameObject selectedChampion = battleChampions[randomIndex];

        ChampionBase championBase = selectedChampion.GetComponent<ChampionBase>();
        if (championBase != null)
        {
            // è�Ǿ𿡰� ������ ����
            championBase.GetItem(itemBlueprint);

            Debug.Log($"AI �÷��̾� {aiUserData.UserName}�� {championBase.ChampionName}���� ������ {itemBlueprint.ItemName}��(��) �����߽��ϴ�.");
        }
    }

    #endregion

    #region AI �ൿ �켱���� �� ���� ����

    private bool ShouldLevelUp()
    {
        // AI�� ���� ������ �ִ� ���� �� (��: �ִ� ���� 9)
        if (aiUserData.UserLevel >= 9)
        {
            return false; // �̹� �ִ� �����̹Ƿ� ���������� ����
        }

        // ��ġ ������ è�Ǿ� ���� ���� ��ġ�� è�Ǿ� �� ��
        if (aiUserData.CurrentPlaceChampion >= aiUserData.MaxPlaceChampion)
        {
            // ��ġ ������ è�Ǿ� ������ �� á�ٸ� ������ ���
            if (aiUserData.UserGold >= 4)
            {
                return true; // ��尡 ����ϸ� ������
            }
        }

        // ���� ��尡 ���� �������� ���� �������� �� ���� ����� è�Ǿ��� ��� ���� ��
        if (aiUserData.UserGold >= 50 && aiUserData.UserLevel < 8)
        {
            return true;
        }

        // ���� ���� �� �������� ���� ������ ����
        if (aiUserData.UserSuccessiveWin >= 3 && aiUserData.UserGold >= 8)
        {
            return true;
        }

        // ��Ÿ ������ �Ǵ� �߰� ����

        return false;
    }

    private bool ShouldReRollChampions()
    {
        // ���� ������ �ó��� ��
        int currentSynergyCount = GetCurrentSynergyCount();

        // ���� AI�� ��尡 ����ϰ�, �ó����� �´� è�Ǿ��� �����ϴٸ� ����
        if (aiUserData.UserGold >= 2 && currentSynergyCount < desiredSynergyThreshold)
        {
            return true;
        }

        // ���� AI�� ����� ��带 �����ϰ� �ְ�, è�Ǿ��� �����ϴٸ� ����
        if (aiUserData.UserGold >= 50)
        {
            return true;
        }

        // �� �ܿ��� �������� ����
        return false;
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
