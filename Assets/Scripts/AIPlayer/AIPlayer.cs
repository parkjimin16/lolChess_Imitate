using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class AIPlayer
{
    private Player aiPlayerComponent; // AI 플레이어의 Player 컴포넌트
    private UserData aiUserData;      // AI 플레이어의 UserData
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

        // 리롤 여부 결정 및 리롤 수행
        if (ShouldReRollChampions())
        {
            ReRollChampions(aiPlayer);
        }
        // 필요에 따라 추가 행동 (레벨업, 리롤 등)
        BuyChampions(aiPlayer); // 챔피언 구매
        DecideAndPlaceChampions(); // 챔피언 배치
        CollectCapsules(aiPlayer);
        UseItems();
    }

    #region 챔피언 구매 로직
    private void BuyChampions(Player aiPlayer)
    {
        int maxRerollAttempts = 5; // 최대 리롤 시도 횟수
        int rerollAttempts = 0;

        while (rerollAttempts <= maxRerollAttempts)
        {
            // 상점에서 챔피언 목록을 가져옵니다.
            shopChampionList = Manager.Champion.GetRandomChampions(aiUserData.UserLevel);

            // 시너지에 맞는 챔피언을 찾습니다.
            List<string> preferredChampions = FindPreferredChampions();

            if (preferredChampions.Count > 0)
            {
                // 시너지에 맞는 챔피언이 있으면 구매
                string selectedChampionName = preferredChampions[UnityEngine.Random.Range(0, preferredChampions.Count)];
                ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint(selectedChampionName) as ChampionBlueprint;

                int championCost = Utilities.SetSlotCost(championBlueprint.ChampionCost);

                if (championBlueprint != null && aiUserData.UserGold >= championCost)
                {
                    aiUserData.UserGold -= championCost;
                    InstantiateAIChampion(championBlueprint, aiPlayer);
                    Debug.Log($"AI {aiUserData.UserName}가 {championCost} 골드를 사용하여 시너지에 맞는 챔피언 {selectedChampionName}을 구매했습니다. 남은 골드: {aiUserData.UserGold}");
                    break; // 구매 후 반복문 종료
                }
            }
            else
            {
                // 시너지에 맞는 챔피언이 없으면 리롤 여부 결정
                if (ShouldReRollChampions() && aiUserData.UserGold >= 2)
                {
                    ReRollChampions(aiPlayer);
                    rerollAttempts++;
                }
                else
                {
                    // 리롤하지 않거나 골드가 부족하면 구매하지 않고 종료
                    Debug.Log($"AI {aiUserData.UserName}는 시너지에 맞는 챔피언을 찾지 못했습니다.");
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

                // 챔피언의 라인 시너지 확인
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

                // 챔피언의 직업 시너지 확인
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
        // AI 플레이어의 맵 정보를 가져옵니다.
        MapGenerator.MapInfo aiMapInfo = aiUserData.MapInfo;

        if (aiMapInfo == null)
        {
            Debug.LogWarning($"AI 플레이어 {aiUserData.UserName}의 MapInfo를 찾을 수 없습니다.");
            return;
        }

        // 빈 타일 찾기
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

            // AI 플레이어의 챔피언 리스트에 추가
            //aiUserData.NonBattleChampionObject.Add(newChampionObject);
            Manager.Champion.SettingNonBattleChampion(aiUserData);
        }
        else
        {
            Debug.LogWarning($"AI 플레이어 {aiUserData.UserName}의 맵에 빈 타일이 없습니다.");
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
        // 게임에 존재하는 모든 시너지 목록을 가져옵니다.
        List<string> allSynergies = GetAllAvailableSynergies();

        // 랜덤하게 시너지 몇 개를 선택합니다.
        int numberOfSynergiesToChoose = 3; // 원하는 시너지 개수
        chosenSynergies = new List<string>();

        for (int i = 0; i < numberOfSynergiesToChoose; i++)
        {
            if (allSynergies.Count == 0)
                break;

            int randomIndex = UnityEngine.Random.Range(0, allSynergies.Count);
            chosenSynergies.Add(allSynergies[randomIndex]);
            allSynergies.RemoveAt(randomIndex);
        }

        Debug.Log($"AI {aiUserData.UserName}가 선택한 시너지: {string.Join(", ", chosenSynergies)}");
    }
    private List<string> GetAllAvailableSynergies()
    {
        List<string> allSynergies = new List<string>();

        // 모든 시너지 이름을 가져옵니다.
        // ChampionLine과 ChampionJob enum의 모든 값을 문자열로 변환하여 리스트에 추가합니다.

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

    #region 챔피언 배치 로직
    private void DecideAndPlaceChampions()
    {
        // 배치 가능한 슬롯 수 계산
        int availableSlots = aiUserData.MaxPlaceChampion - aiUserData.CurrentPlaceChampion;

        if (availableSlots <= 0)
        {
            return;
        }

        // RectTile에 있는 챔피언들 중에서 배치할 챔피언 선택
        List<GameObject> championsOnBench = new List<GameObject>(aiUserData.NonBattleChampionObject);

        // 시너지에 맞는 챔피언들을 우선적으로 배치
        championsOnBench.Sort((a, b) =>
        {
            int aScore = GetChampionSynergyScore(a);
            int bScore = GetChampionSynergyScore(b);
            return bScore.CompareTo(aScore); // 내림차순 정렬
        });

        // 배치 가능한 수만큼 챔피언 선택
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
        // 현재 배틀필드에 배치된 챔피언 수 확인
        int currentBattleChampions = aiUserData.CurrentPlaceChampion;

        // AI 플레이어의 최대 배치 가능 챔피언 수
        int maxBattleChampions = aiUserData.MaxPlaceChampion;

        // 배치 가능한 슬롯 수 계산
        int availableSlots = maxBattleChampions - currentBattleChampions;

        if (availableSlots <= 0)
        {
            // 배치 가능한 슬롯이 없으면 반환
            return;
        }

        // NonBattleChampionObject 리스트에서 챔피언을 가져옵니다.
        List<GameObject> championsOnBench = new List<GameObject>(aiUserData.NonBattleChampionObject);

        // 배치 가능한 수만큼 챔피언 선택
        for (int i = 0; i < availableSlots && championsOnBench.Count > 0; i++)
        {
            // 랜덤 챔피언 선택
            int randomIndex = UnityEngine.Random.Range(0, championsOnBench.Count);
            GameObject championToPlace = championsOnBench[randomIndex];

            PlaceChampionOnHexTile(championToPlace);
            championsOnBench.RemoveAt(randomIndex);
        }
    }

    private void PlaceChampionOnHexTile(GameObject champion)
    {
        // AI 플레이어의 맵 정보를 가져옵니다.
        MapGenerator.MapInfo aiMapInfo = aiUserData.MapInfo;

        if (aiMapInfo == null)
        {
            Debug.LogWarning($"AI 플레이어 {aiUserData.UserName}의 MapInfo를 찾을 수 없습니다.");
            return;
        }

        // 빈 HexTile 찾기
        HexTile emptyTile = FindEmptyHexTile(aiMapInfo);

        if (emptyTile != null)
        {
            // 챔피언의 현재 타일 정보 가져오기
            HexTile currentTile = champion.transform.parent.GetComponent<HexTile>();
            if (currentTile != null)
            {
                currentTile.championOnTile.Remove(champion);
            }

            // 챔피언의 위치와 부모를 업데이트
            champion.transform.position = emptyTile.transform.position;
            champion.transform.SetParent(emptyTile.transform);

            // 타일 상태 업데이트
            emptyTile.championOnTile.Add(champion);

            // AI 플레이어의 리스트 업데이트
            Manager.Champion.SettingAllChampion(aiUserData);
            Manager.Champion.SettingBattleChampion(aiUserData);
        }
        else
        {
            Debug.LogWarning($"AI 플레이어 {aiUserData.UserName}의 HexTile에 빈 타일이 없습니다.");
        }
    }

    private HexTile FindEmptyHexTile(MapGenerator.MapInfo mapInfo)
    {
        List<HexTile> availableTiles = new List<HexTile>();

        // 조건에 맞는 타일들을 리스트에 추가합니다.
        foreach (var tileEntry in mapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (!tile.isOccupied && tile.CompareTag("PlayerTile"))
            {
                availableTiles.Add(tile);
            }
        }

        // 리스트가 비어있지 않으면 랜덤한 타일을 반환합니다.
        if (availableTiles.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableTiles.Count);
            return availableTiles[randomIndex];
        }

        // 조건에 맞는 타일이 없으면 null을 반환합니다.
        return null;
    }
    #endregion

    #region 챔피언 슬롯 리롤 로직
    private void ReRollChampions(Player aiPlayer)
    {
        // 챔피언 슬롯 리롤 비용: 2골드
        int rerollCost = 2;

        if (aiUserData.UserGold >= rerollCost)
        {
            // 골드 소모
            aiUserData.UserGold -= rerollCost;
            //UIManager.Instance.UpdateUserGoldUI(aiUserData); // UI 업데이트 (필요 시)

            // 새로운 챔피언 리스트 생성
            shopChampionList = Manager.Champion.GetRandomChampions(aiUserData.UserLevel);//GetRandomChampions(aiUserData.UserLevel);

            Debug.Log($"AI {aiUserData.UserName}가 2 골드를 사용하여 챔피언 슬롯을 리롤했습니다. 남은 골드: {aiUserData.UserGold}");
        }
        else
        {
            Debug.Log($"AI {aiUserData.UserName}에게는 챔피언 슬롯을 리롤할 충분한 골드가 없습니다.");
        }
    }
    #endregion

    #region 경험치 구매 로직
    private void BuyExperience(Player aiPlayer)
    {
        // 경험치 구매 비용: 4골드당 4 EXP
        int experienceCost = 4;
        int experienceAmount = 4;

        if (aiUserData.UserGold >= experienceCost)
        {
            // 골드 소모
            aiUserData.UserGold -= experienceCost;
            //UIManager.Instance.UpdateUserGoldUI(aiUserData); // UI 업데이트 (필요 시)

            // 경험치 추가
            Manager.Level.AddExperience(aiUserData, experienceAmount);
            // 또는 LevelManager.Instance.AddExperience(aiUserData, experienceAmount);

            Debug.Log($"AI {aiUserData.UserName}가 {experienceCost} 골드를 사용하여 {experienceAmount} EXP를 구매했습니다. 남은 골드: {aiUserData.UserGold}");
        }
        else
        {
            Debug.Log($"AI {aiUserData.UserName}에게는 경험치를 구매할 충분한 골드가 없습니다.");
        }
    }
    #endregion

    #region 캡슐 탐지 로직
    private void CollectCapsules(Player aiPlayer)
    {
        // AI 플레이어의 맵 정보를 가져옵니다.
        MapInfo aiMapInfo = aiPlayer.UserData.MapInfo;

        if (aiMapInfo == null)
        {
            Debug.LogWarning($"AI 플레이어 {aiPlayer.UserData.UserName}의 MapInfo를 찾을 수 없습니다.");
            return;
        }

        // 맵의 모든 HexTile에서 캡슐을 수집합니다.
        List<GameObject> capsulesToCollect = new List<GameObject>();

        foreach (var tileEntry in aiMapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;

            if (tile.capsuleOnTile != null && tile.capsuleOnTile.Count > 0)
            {
                capsulesToCollect.AddRange(tile.capsuleOnTile);
            }
        }

        // 캡슐이 있으면 수집을 시작합니다.
        if (capsulesToCollect.Count > 0)
        {
            // 캡슐을 순서대로 처리하기 위해 큐에 넣습니다.
            Queue<GameObject> capsuleQueue = new Queue<GameObject>(capsulesToCollect);

            // 캡슐을 하나씩 처리하는 코루틴을 시작합니다.
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

            // 캡슐의 위치로 이동합니다.
            yield return aiPlayer.StartCoroutine(MoveToPosition(aiPlayer.transform, capsule.transform.position));

            // 캡슐과 충돌하여 아이템을 획득합니다.
            // 충돌 처리는 OnTriggerEnter 또는 OnCollisionEnter를 통해 처리되었다고 가정합니다.
            // 만약 그렇지 않다면, 여기서 캡슐의 아이템 획득 로직을 직접 호출해야 합니다.

            // 잠시 대기하여 다음 캡슐로 이동하기 전에 시간을 줍니다.
            yield return new WaitForSeconds(0.2f);
        }
    }
    private IEnumerator MoveToPosition(Transform aiTransform, Vector3 targetPosition)
    {
        float moveSpeed = 5f; // 이동 속도 설정

        while (Vector3.Distance(aiTransform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - aiTransform.position).normalized;
            aiTransform.position = Vector3.MoveTowards(aiTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            aiTransform.rotation = Quaternion.LookRotation(direction);
            yield return null;
        }
    }
    #endregion

    #region 아이템 사용 로직
    private void UseItems()
    {
        // AI 플레이어의 아이템 리스트를 가져옵니다.
        List<GameObject> items = new List<GameObject>(aiUserData.UserItemObject);

        // 아이템이 있을 경우 사용
        while (items.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, items.Count);
            GameObject itemObj = items[randomIndex]; // 리스트의 첫 번째 아이템
            ItemFrame itemFrame = itemObj.GetComponent<ItemFrame>();

            if (itemFrame != null)
            {
                // 아이템 블루프린트 가져오기
                ItemBlueprint itemBlueprint = itemFrame.ItemBlueprint;

                // 아이템을 챔피언에게 줍니다.
                GiveItemToRandomChampion(itemBlueprint);

                itemObj.transform.parent.GetComponent<HexTile>().isItemTile = false;
                itemObj.transform.parent.GetComponent<HexTile>().itemOnTile = null;

                // 아이템 오브젝트를 제거하고 리스트에서 삭제
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
        // AI 플레이어의 BattleChampionObject 리스트를 가져옵니다.
        List<GameObject> battleChampions = aiUserData.BattleChampionObject;

        if (battleChampions.Count == 0)
        {
            Debug.Log($"AI 플레이어 {aiUserData.UserName}의 배틀 챔피언이 없습니다.");
            return;
        }

        // 랜덤한 챔피언 선택
        int randomIndex = UnityEngine.Random.Range(0, battleChampions.Count);
        GameObject selectedChampion = battleChampions[randomIndex];

        ChampionBase championBase = selectedChampion.GetComponent<ChampionBase>();
        if (championBase != null)
        {
            // 챔피언에게 아이템 적용
            championBase.GetItem(itemBlueprint);

            Debug.Log($"AI 플레이어 {aiUserData.UserName}가 {championBase.ChampionName}에게 아이템 {itemBlueprint.ItemName}을(를) 장착했습니다.");
        }
    }

    #endregion

    #region AI 행동 우선순위 및 조건 설정

    private bool ShouldLevelUp()
    {
        // AI의 현재 레벨과 최대 레벨 비교 (예: 최대 레벨 9)
        if (aiUserData.UserLevel >= 9)
        {
            return false; // 이미 최대 레벨이므로 레벨업하지 않음
        }

        // 배치 가능한 챔피언 수와 실제 배치된 챔피언 수 비교
        if (aiUserData.CurrentPlaceChampion >= aiUserData.MaxPlaceChampion)
        {
            // 배치 가능한 챔피언 슬롯이 꽉 찼다면 레벨업 고려
            if (aiUserData.UserGold >= 4)
            {
                return true; // 골드가 충분하면 레벨업
            }
        }

        // 현재 골드가 많고 레벨업을 통해 상점에서 더 높은 등급의 챔피언을 얻고 싶을 때
        if (aiUserData.UserGold >= 50 && aiUserData.UserLevel < 8)
        {
            return true;
        }

        // 연승 중일 때 레벨업을 통해 우위를 유지
        if (aiUserData.UserSuccessiveWin >= 3 && aiUserData.UserGold >= 8)
        {
            return true;
        }

        // 기타 전략적 판단 추가 가능

        return false;
    }

    private bool ShouldReRollChampions()
    {
        // 현재 보유한 시너지 수
        int currentSynergyCount = GetCurrentSynergyCount();

        // 만약 AI의 골드가 충분하고, 시너지에 맞는 챔피언이 부족하다면 리롤
        if (aiUserData.UserGold >= 2 && currentSynergyCount < desiredSynergyThreshold)
        {
            return true;
        }

        // 만약 AI가 충분한 골드를 보유하고 있고, 챔피언이 부족하다면 리롤
        if (aiUserData.UserGold >= 50)
        {
            return true;
        }

        // 그 외에는 리롤하지 않음
        return false;
    }

    private int GetCurrentSynergyCount()
    {
        // 현재 AI의 챔피언 슬롯에 있는 시너지의 개수를 반환
        // 예시로 단순히 현재 시너지 수를 반환하도록 구현
        return aiUserData.ChampionSynergies_Line.Count + aiUserData.ChampionSynergies_Job.Count;
    }

    private int desiredSynergyThreshold = 3; // 원하는 시너지 수 설정
    #endregion

}
