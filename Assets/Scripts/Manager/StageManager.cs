using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static MapGenerator;

public class StageManager
{
    public GameObject[] AllPlayers; // 총 8명의 플레이어 (자기 자신 포함)
    private List<GameObject> opponents; // 상대 플레이어 목록 (자기 자신 제외)
    private GameObject selfPlayer; // 자기 자신
    private GameObject currentOpponent; // 현재 상대

    private MapGenerator _mapGenerator;

    public int currentStage = 1;
    public int currentRound = 1;

    // 스테이지별 기본 피해량과 생존한 적 유닛당 피해량
    public int[] baseDamages = new int[] { 0, 2, 5, 8, 10, 12, 17 }; // 인덱스는 스테이지 번호 - 1
    public int[] damagePerEnemyUnit = new int[] { 1, 2, 3, 4, 5, 6, 7 }; // 인덱스는 스테이지 번호 - 1

    // 라운드 대기시간 설정
    private int normalWaitTime = 3; //라운드 전 대기시간
    private int augmentWaitTime = 3; //증강 선택 라운드 시간
    private int postMatchWaitTime = 3; //매치 후 대기시간
    private int roundDuration = 3; //일반 라운드 진행시간

    private bool isAugmentRound = false;

    private Coroutine roundCoroutine;

    private GameObject CripPrefab;
    #region Init

    public void InitStage(GameObject[] playerData, MapGenerator mapGenerator)
    {
        AllPlayers = playerData;
        _mapGenerator = mapGenerator;
        InitializePlayers();
        StartStage(currentStage);
        
    }
    #endregion

    private void InitializePlayers()
    {
        // 자기 자신과 상대 플레이어 분리
        // 여기서는 첫 번째 플레이어를 자기 자신으로 가정
        
        selfPlayer = AllPlayers[0];
        
        opponents = new List<GameObject>(AllPlayers);
        opponents.Remove(selfPlayer);

    }

    void StartStage(int stageNumber)
    {
        currentRound = 1;
        ShuffleOpponents(); // 상대 리스트를 섞음

        if (roundCoroutine != null)
            CoroutineHelper.StopCoroutine(roundCoroutine);

        roundCoroutine = CoroutineHelper.StartCoroutine(StartRoundCoroutine());
    }

    IEnumerator StartRoundCoroutine()
    {
        // UI 업데이트
        UIManager.Instance.UpdateStageRoundUI(currentStage, currentRound);

        // 증강 선택 라운드 여부 확인
        isAugmentRound = IsAugmentRound(currentStage, currentRound);

        // **공동 선택 라운드 여부 확인**
        bool isCarouselRound = IsCarouselRound(currentStage, currentRound);

        // 크립 라운드 여부 확인
        bool isCripRound = IsCripRound(currentStage, currentRound);

        // 대기시간 설정
        int waitTime = isAugmentRound ? augmentWaitTime : normalWaitTime;

      //  Debug.Log($"라운드 시작 전 대기시간: {waitTime}초");

        // 대기시간 타이머 시작
        UIManager.Instance.StartTimer(waitTime);

        yield return new WaitForSeconds(waitTime);

        if (isCarouselRound)
        {
            // **공동 선택 라운드 처리**
            CoroutineHelper.StartCoroutine(StartCarouselRound());
        }
        else if (isCripRound)
        {
            // 크립 라운드 처리
            CoroutineHelper.StartCoroutine(StartCripRound());
        }
        else
        {
            // **일반 라운드 처리**

            // 상대 매칭
            int opponentIndex = (currentRound - 1) % opponents.Count;
            currentOpponent = opponents[opponentIndex];

          //  Debug.Log($"{currentOpponent.GetComponent<Player>().PlayerName}와 매칭되었습니다.");

            // 매칭 후 대기시간
           // Debug.Log($"매칭 후 대기시간: {postMatchWaitTime}초");

            // 매칭 후 대기시간 타이머 시작
            UIManager.Instance.StartTimer(postMatchWaitTime);

            yield return new WaitForSeconds(postMatchWaitTime);

            //Debug.Log("라운드가 시작됩니다!");

            // 전투 시작
            Manager.Battle.StartBattle(selfPlayer, currentOpponent, roundDuration);

            // 라운드 진행 시간 타이머 시작
            UIManager.Instance.StartTimer(roundDuration);
        }
    }

    public void OnRoundEnd(bool playerWon, int survivingEnemyUnits)
    {
        if (!playerWon)
        {
            ApplyDamage(survivingEnemyUnits);
        }

        // 라운드 증가
        currentRound++;

        int maxRounds = currentStage == 1 ? 3 : 7;

        if (currentRound > maxRounds)
        {
            // 다음 스테이지로 이동
            currentStage++;
            if (currentStage > 8)
            {
                // 게임 종료
               // Debug.Log("게임 클리어!");
                return;
            }
            StartStage(currentStage);
        }
        else
        {
            // 다음 라운드 시작
            if (roundCoroutine != null)
                CoroutineHelper.StopCoroutine(roundCoroutine);
            roundCoroutine = CoroutineHelper.StartCoroutine(StartRoundCoroutine());
        }
    }

    void ApplyDamage(int survivingEnemyUnits)
    {
        int index = currentStage - 1;
        int totalDamage = baseDamages[index] + (damagePerEnemyUnit[index] * survivingEnemyUnits);

        // 플레이어에게 데미지 적용
        int Hp = selfPlayer.GetComponent<Player>().UserData.UserHealth;
        Hp -= totalDamage;
        selfPlayer.GetComponent<Player>().UserData.UserHealth = Hp;
        //Debug.Log($"플레이어가 {totalDamage}의 피해를 입었습니다. 남은 체력: {selfPlayer.GetComponent<Player>().CurrentHealth}");

        // 체력바 업데이트
        Manager.UserHp.UpdateHealthBars();


        // 게임 오버 체크
        if (selfPlayer.GetComponent<Player>().UserData.UserHealth <= 0)
        {
           // Debug.Log("게임 오버!");
            // 게임 오버 로직 처리
        }
    }

    void DisplayCurrentStageAndRound()
    {
       // Debug.Log($"현재 스테이지: {currentStage}, 현재 라운드: {currentRound}");
    }

    void ShuffleOpponents()
    {
        for (int i = 0; i < opponents.Count; i++)
        {
            GameObject temp = opponents[i];
            int randomIndex = Random.Range(i, opponents.Count);
            opponents[i] = opponents[randomIndex];
            opponents[randomIndex] = temp;
        }
    }

    bool IsAugmentRound(int stage, int round)
    {
        // 증강 선택 라운드: 2-1, 3-2, 4-2
        if ((stage == 2 && round == 1) || (stage == 3 && round == 2) || (stage == 4 && round == 2))
        {
            return true;
        }
        return false;
    }

    #region 공동선택 라운드
    IEnumerator StartCarouselRound()
    {
        //Debug.Log("공동 선택 라운드가 시작됩니다!");

        // 카메라를 공동 선택 맵으로 이동
        

        // 모든 플레이어의 움직임을 일시적으로 비활성화
        DisableAllPlayerMovement();

        foreach (GameObject playerObj in AllPlayers)
        {
            PlayerMove playerMove = playerObj.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                playerMove.StartCarouselRound(_mapGenerator.sharedSelectionMapTransform, _mapGenerator.sharedMapInfo);
            }
        }

        // 플레이어들을 체력 순서대로 정렬
        List<GameObject> sortedPlayers = GetPlayersSortedByHealth();

        int playersPerInterval = 2; // 한 번에 움직일 수 있는 플레이어 수
        int intervalWaitTime = 6;   // 다음 그룹이 움직이기까지의 대기 시간

        // 플레이어들을 공동 선택 맵 위치로 이동
        

        for (int i = 0; i < sortedPlayers.Count; i += playersPerInterval)
        {
            // 현재 그룹의 플레이어들
            List<GameObject> currentGroup = sortedPlayers.GetRange(i, Mathf.Min(playersPerInterval, sortedPlayers.Count - i));

            // 해당 플레이어들의 움직임 활성화
            EnablePlayerMovement(currentGroup);

            // 대기 시간
            if (i + playersPerInterval < sortedPlayers.Count)
            {
                yield return new WaitForSeconds(intervalWaitTime);
            }
        }

        // 모든 플레이어가 챔피언을 선택할 때까지 대기 (간단하게 일정 시간 대기하도록 설정)
        float totalCarouselDuration = 30f; // 전체 공동 선택 라운드 시간
        yield return new WaitForSeconds(totalCarouselDuration);

        // 공동 선택 라운드 종료 처리
        EndCarouselRound();
    }

    bool IsCarouselRound(int stage, int round)
    {
        // 2스테이지부터 매 4라운드마다 공동 선택 라운드
        if (stage >= 2 && round % 4 == 0)
        {
            DisableAllPlayerMovement();
            _mapGenerator.PlacePlayersInSharedMap(_mapGenerator.sharedSelectionMapTransform);
            CameraManager.Instance.MoveCameraToSharedSelectionMap();
            return true;
        }
        return false;
    }

    void DisableAllPlayerMovement()
    {
        foreach (GameObject playerObj in AllPlayers)
        {
            PlayerMove playerMove = playerObj.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                playerMove.SetCanMove(false);
            }
        }
    }

    void EnablePlayerMovement(List<GameObject> players)
    {
        foreach (GameObject playerObj in players)
        {
            PlayerMove playerMove = playerObj.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                playerMove.SetCanMove(true);

                // AI 플레이어의 경우 자동으로 챔피언을 선택하도록 처리
                Player playerComponent = playerObj.GetComponent<Player>();
                if (playerComponent.UserData.PlayerType != PlayerType.Player1)
                {
                    playerMove.MoveToRandomChampion();
                }
            }
        }
    }
    List<GameObject> GetPlayersSortedByHealth()
    {
        List<GameObject> sortedPlayers = new List<GameObject>(AllPlayers);

        sortedPlayers.Sort((a, b) =>
        {
            int healthA = a.GetComponent<Player>().UserData.UserHealth;
            int healthB = b.GetComponent<Player>().UserData.UserHealth;

            // 체력이 낮은 순서대로 정렬
            return healthA.CompareTo(healthB);
        });

        return sortedPlayers;
    }
    void EndCarouselRound()
    {
       // Debug.Log("공동 선택 라운드가 종료되었습니다.");

        // 모든 플레이어들에게 공동 선택 라운드 종료를 알리고 원래 위치로 복귀
        foreach (GameObject playerObj in AllPlayers)
        {
            PlayerMove playerMove = playerObj.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                // 부모를 다시 설정 (최상위 또는 원래 부모로)
                playerObj.transform.SetParent(null);

                // 원래 위치로 복귀
                playerMove.ReturnToOriginalPosition();

                // 필요한 경우 추가적인 초기화 로직
                playerMove.EndCarouselRound();
                
            }
        }
        CameraManager.Instance.MoveCameraToPlayer(AllPlayers[0].GetComponent<Player>());
        // 이후 라운드 진행 로직
        // 다음 라운드로 이동
        currentRound++;

        int maxRounds = currentStage == 1 ? 3 : 7;

        if (currentRound > maxRounds)
        {
            // 다음 스테이지로 이동
            currentStage++;
            if (currentStage > 8)
            {
                // 게임 종료
               // Debug.Log("게임 클리어!");
                return;
            }
            StartStage(currentStage);
        }
        else
        {
            // 다음 라운드 시작
            if (roundCoroutine != null)
                CoroutineHelper.StopCoroutine(roundCoroutine);
            roundCoroutine = CoroutineHelper.StartCoroutine(StartRoundCoroutine());
        }
    }
    #endregion

    #region 한칸이내 챔피언 찾기
    public List<GameObject> GetChampionsWithinOneTile(GameObject champion)
    {
        List<GameObject> champions = new List<GameObject>();

        // 챔피언의 위치 가져오기
        Vector3 championPosition = champion.transform.position;

        // 가장 가까운 타일 찾기
        HexTile nearestTile = null;
        float minDistance = float.MaxValue;

        foreach (HexTile tile in _mapGenerator.mapInfos[0].HexDictionary.Values)
        {
            float distance = Vector3.Distance(championPosition, tile.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTile = tile;
            }
        }

        if (nearestTile == null)
        {
            //Debug.LogWarning("근처에 타일이 없습니다.");
            return champions;
        }

        int q = nearestTile.q;
        int r = nearestTile.r;

        // 짝수 행인지 여부 판단
        bool isEvenRow = (r % 2) == 0;
        (int dq, int dr)[] directions;

        if (isEvenRow)
        {
            // 짝수 행의 방향
            directions = new (int, int)[]
            {
            (1, 0),    // 동쪽
            (1, -1),   // 남동쪽
            (0, -1),   // 남서쪽
            (-1, 0),   // 서쪽
            (0, 1),    // 북서쪽
            (1, 1)     // 북동쪽
            };
        }
        else
        {
            // 홀수 행의 방향
            directions = new (int, int)[]
            {
            (1, 0),    // 동쪽
            (0, -1),   // 남동쪽
            (-1, -1),  // 남서쪽
            (-1, 0),   // 서쪽
            (-1, 1),   // 북서쪽
            (0, 1)     // 북동쪽
            };
        }

        foreach (var dir in directions)
        {
            int neighborQ = q + dir.dq;
            int neighborR = r + dir.dr;

            // 인접 타일이 존재하는지 확인
            if (_mapGenerator.mapInfos[0].HexDictionary.TryGetValue((neighborQ, neighborR), out HexTile neighborTile))
            {
                if (neighborTile.itemOnTile != null && neighborTile.itemOnTile != champion)
                {
                    champions.Add(neighborTile.itemOnTile);
                }
            }
        }

        return champions;
    }
    public void DebugChampionList(List<GameObject> champions)
    {
        if (champions == null || champions.Count == 0)
        {
            //   Debug.Log("주변에 챔피언이 없습니다.");
            return;
        }

        // Debug.Log($"주변에 있는 챔피언 수: {champions.Count}");
        foreach (var champion in champions)
        {
            if (champion != null)
            {
                //  Debug.Log($"챔피언 이름: {champion.name}, 위치: {champion.transform.position}");
            }
            else
            {
                // Debug.Log("챔피언이 null입니다.");
            }
        }
    }
    #endregion

    #region 플레이어 맵 정보 받아오기
    private MapInfo GetPlayerMapInfo(Player playerComponent)
    {
        // MapGenerator에서 플레이어의 MapInfo를 찾습니다.
        foreach (var mapInfo in _mapGenerator.mapInfos)
        {
            if (mapInfo.playerData == playerComponent)
            {
                return mapInfo;
            }
        }

        // 찾지 못한 경우 null 반환
        return null;
    }
    #endregion

    #region 크립라운드
    bool IsCripRound(int stage, int round)
    {
        // 스테이지 1의 1, 2, 3 라운드와 2스테이지부터 매 7라운드마다 크립 라운드
        if (stage == 1 && (round == 1 || round == 2 || round == 3))
            return true;
        else if (stage >= 2 && round == 7)
            return true;
        else
            return false;
    }
    IEnumerator StartCripRound()
    {
        // 크립 생성
        SpawnCrips();

        // 라운드 진행 시간 타이머 시작
        UIManager.Instance.StartTimer(roundDuration);

        // 전투 시작 (크립과의 전투 로직을 시작해야 합니다)
        //Manager.Battle.StartCripBattle(selfPlayer, roundDuration);

        // 라운드 진행 시간만큼 대기
        yield return new WaitForSeconds(roundDuration);

        // 라운드 종료 처리
        EndCripRound();
    }
    void SpawnCrips()
    {
        foreach (GameObject player in AllPlayers)
        {
            Player playerComponent = player.GetComponent<Player>();
            MapInfo playerMapInfo = GetPlayerMapInfo(playerComponent);

            if (playerMapInfo == null)
            {
                Debug.LogWarning($"플레이어 {playerComponent.UserData.UserName}의 MapInfo를 찾을 수 없습니다.");
                continue;
            }

            // 사용할 수 있는 타일 목록을 생성합니다.
            List<HexTile> availableTiles = new List<HexTile>();

            foreach (var tileEntry in playerMapInfo.HexDictionary)
            {
                HexTile tile = tileEntry.Value;
                // 타일이 "EnemyTile" 태그를 가지고 있고, 비어있는 경우에만 추가
                if (tile.CompareTag("EnemyTile") && !tile.isOccupied)
                {
                    availableTiles.Add(tile);
                }
            }

            if (availableTiles.Count == 0)
            {
                Debug.LogWarning($"플레이어 {playerComponent.UserData.UserName}의 맵에 크립을 생성할 수 있는 타일이 없습니다.");
                continue;
            }

            // 생성할 크립의 수를 결정합니다.
            int numCripsToSpawn = GetNumberOfCripsToSpawn(currentStage, currentRound);

            // 생성할 수 있는 최대 크립 수를 계산합니다.
            int cripsToSpawn = Mathf.Min(numCripsToSpawn, availableTiles.Count);

            // 타일 목록을 섞습니다.
            for (int i = 0; i < availableTiles.Count; i++)
            {
                HexTile temp = availableTiles[i];
                int randomIndex = Random.Range(i, availableTiles.Count);
                availableTiles[i] = availableTiles[randomIndex];
                availableTiles[randomIndex] = temp;
            }

            // 필요한 수만큼의 타일에 크립을 생성합니다.
            for (int i = 0; i < cripsToSpawn; i++)
            {
                HexTile tile = availableTiles[i];

                CripPrefab = Manager.Asset.InstantiatePrefab("Crip", tile.transform);
                CripPrefab.transform.position = tile.transform.position + new Vector3(0, 0.5f, 0);
                

                // 타일에 크립을 설정합니다.
                tile.championOnTile = CripPrefab;
                tile.isOccupied = true;

                // 크립에 현재 타일 정보를 설정합니다.
                Crip cripComponent = CripPrefab.GetComponent<Crip>();
                if (cripComponent != null)
                {
                    cripComponent.currentTile = tile;
                    cripComponent.playerMapInfo = playerMapInfo;
                }

                // 크립의 부모를 설정하여 맵 구조에 포함되도록 합니다.
                CripPrefab.transform.SetParent(tile.transform);
            }
        }
    }

    int GetNumberOfCripsToSpawn(int stage, int round)
    {
        // 스테이지와 라운드에 따라 생성할 크립의 수를 설정합니다.

        if (stage == 1)
            return 3; // 예시로 스테이지 1에서는 3마리 생성
        else if (stage == 2)
            return 4; // 스테이지 2에서는 4마리 생성
        else
            return 5; // 그 외에는 5마리 생성
    }

    void EndCripRound()
    {
        foreach (GameObject player in AllPlayers)
        {
            Player playerComponent = player.GetComponent<Player>();
            MapInfo playerMapInfo = GetPlayerMapInfo(playerComponent);

            if (playerMapInfo == null)
            {
                Debug.LogWarning($"플레이어 {playerComponent.UserData.UserName}의 MapInfo를 찾을 수 없습니다.");
                continue;
            }

            // 해당 플레이어의 맵에 남아있는 크립을 찾습니다.
            List<Crip> remainingCrips = new List<Crip>();

            // 플레이어의 맵에 있는 모든 타일을 검사합니다.
            foreach (var tileEntry in playerMapInfo.HexDictionary)
            {
                HexTile tile = tileEntry.Value;
                if (tile.championOnTile != null)
                {
                    Crip crip = tile.championOnTile.GetComponent<Crip>();
                    if (crip != null)
                    {
                        remainingCrips.Add(crip);
                    }
                }
            }
            int survivingCrips = remainingCrips.Count;

            // 남아있는 크립 파괴 및 타일 상태 업데이트
            foreach (Crip crip in remainingCrips)
            {
                if (crip.currentTile != null)
                {
                    crip.currentTile.isOccupied = false;
                    crip.currentTile.championOnTile = null;
                }
                crip.Death(); // OnDeath() 함수를 호출하여 아이템 생성 및 크립 파괴 처리
            }

            // 플레이어 승리 여부 판단
            bool playerWon = survivingCrips == 0;

            // 각 플레이어별로 라운드 종료 처리
            //OnRoundEndForPlayer(playerComponent, playerWon, survivingCrips);
        }
        currentRound++;

        int maxRounds = currentStage == 1 ? 3 : 7;

        if (currentRound > maxRounds)
        {
            // 다음 스테이지로 이동
            currentStage++;
            if (currentStage > 8)
            {
                // 게임 종료
                // Debug.Log("게임 클리어!");
                return;
            }
            StartStage(currentStage);
        }
        else
        {
            // 다음 라운드 시작
            if (roundCoroutine != null)
                CoroutineHelper.StopCoroutine(roundCoroutine);
            roundCoroutine = CoroutineHelper.StartCoroutine(StartRoundCoroutine());
        }

    }

    #endregion
}