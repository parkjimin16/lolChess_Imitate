using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static MapGenerator;

public class StageManager
{
    public GameObject[] AllPlayers; // 총 8명의 플레이어 (자기 자신 포함)
    private List<GameObject> players; // 모든 플레이어 목록
    private List<(GameObject, GameObject)> matchups; // 매칭된 플레이어 페어 목록
    private int ongoingBattles = 0; // 진행 중인 전투 수를 추적하는 변수 추가

    private MapGenerator _mapGenerator;

    public int currentStage = 1;
    public int currentRound = 1;

    // 스테이지별 기본 피해량과 생존한 적 유닛당 피해량
    public int[] baseDamages = new int[] { 0, 2, 5, 8, 10, 12, 17 }; // 인덱스는 스테이지 번호 - 1
    public int[] damagePerEnemyUnit = new int[] { 1, 2, 3, 4, 5, 6, 7 }; // 인덱스는 스테이지 번호 - 1

    // 라운드 대기시간 설정
    private int normalWaitTime = 3; //라운드 전 대기시간
    private int augmentWaitTime = 5; //증강 선택 라운드 시간
    private int postMatchWaitTime = 3; //매치 후 대기시간
    private int roundDuration = 3; //일반 라운드 진행시간

    private bool isAugmentRound = false;

    private Coroutine roundCoroutine;

    private GameObject CripPrefab;

    private List<AIPlayer> aiPlayers = new List<AIPlayer>(); // AIPlayer 인스턴스 리스트
    private GameDataBlueprint gameDataBlueprint;

    private MapInfo battleMap;

    #region Init

    public void InitStage(GameObject[] playerData, MapGenerator mapGenerator, GameDataBlueprint gameData)
    {
        AllPlayers = playerData;
        _mapGenerator = mapGenerator;
        gameDataBlueprint = gameData;

        InitializePlayers();
        StartStage(currentStage);
        
    }


    private void InitializePlayers()
    {
        // 자기 자신과 상대 플레이어 분리
        // 여기서는 첫 번째 플레이어를 자기 자신으로 가정

        players = new List<GameObject>(AllPlayers);

        // AI 플레이어 초기화
        foreach (GameObject playerObj in players)
        {
            Player playerComponent = playerObj.GetComponent<Player>();
            if (playerComponent != null && playerComponent.UserData.PlayerType != PlayerType.Player1)
            {
                AIPlayer aiPlayer = new AIPlayer();
                aiPlayer.InitAIPlayer(playerComponent, gameDataBlueprint);
                aiPlayers.Add(aiPlayer);
            }
        }
    }
    #endregion

    #region 스테이지 로직
    private void StartStage(int stageNumber)
    {
        currentRound = 1;

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

        // 공동 선택 라운드 여부 확인
        bool isCarouselRound = IsCarouselRound(currentStage, currentRound);

        // 크립 라운드 여부 확인
        bool isCripRound = IsCripRound(currentStage, currentRound);

        // 라운드 전 대기시간 설정
        int preWaitTime = isAugmentRound ? augmentWaitTime : normalWaitTime;

        // **1. 라운드 전 대기시간**

        // 대기 시간 동안 AI 행동 실행
        PerformAIActions();

        // 라운드 전 대기시간 타이머 시작
        UIManager.Instance.StartTimer(preWaitTime);

        // 라운드 전 대기시간 동안 대기
        yield return new WaitForSeconds(preWaitTime);

        // **2. 매치 후 대기시간 및 라운드 진행**

        if (isCarouselRound)
        {
            // 공동 선택 라운드 처리
            yield return CoroutineHelper.StartCoroutine(StartCarouselRound());
            yield break; // 공동 선택 라운드는 별도의 흐름이므로 여기서 종료
        }
        else if (isCripRound)
        {
            // 크립 라운드의 경우

            // 크립 생성
            SpawnCrips();

            // 매치 후 대기시간 타이머 시작
            UIManager.Instance.StartTimer(postMatchWaitTime);

            // 매치 후 대기시간 동안 대기
            yield return new WaitForSeconds(postMatchWaitTime);

            // **3. 일반 라운드 진행시간**

            // 크립 라운드 진행
            //StartAllCripBattles();

            // 라운드 진행 시간 타이머 시작
            UIManager.Instance.StartTimer(roundDuration);

            // 라운드 진행 시간 동안 대기
            yield return new WaitForSeconds(roundDuration);

            // 라운드 종료 처리
            EndCripRound();
        }
        else if (isAugmentRound)
        {
            // 증강 선택 라운드의 경우

            // 매칭 및 유닛 이동 수행
            GenerateMatchups();

            // 매치 후 대기시간 타이머 시작
            UIManager.Instance.StartTimer(postMatchWaitTime);

            // 매치 후 대기시간 동안 대기
            yield return new WaitForSeconds(postMatchWaitTime);

            // **3. 일반 라운드 진행시간**

            // 전투 시작
            StartAllBattles();

            // 라운드 진행 시간 타이머 시작
            UIManager.Instance.StartTimer(roundDuration);

            // 라운드 진행 시간 동안 대기
            yield return new WaitForSeconds(roundDuration);

            // 전투 종료는 각 플레이어의 전투가 종료될 때마다 `OnBattleEnd`에서 처리됩니다.
        }
        else
        {
            // 일반 라운드의 경우

            // 매칭 및 유닛 이동 수행
            GenerateMatchups();

            // 매치 후 대기시간 타이머 시작
            UIManager.Instance.StartTimer(postMatchWaitTime);

            // 매치 후 대기시간 동안 대기
            yield return new WaitForSeconds(postMatchWaitTime);

            // **3. 일반 라운드 진행시간**

            // 전투 시작
            StartAllBattles();

            // 라운드 진행 시간 타이머 시작
            UIManager.Instance.StartTimer(roundDuration);

            // 라운드 진행 시간 동안 대기
            yield return new WaitForSeconds(roundDuration);

            // 전투 종료는 각 플레이어의 전투가 종료될 때마다 `OnBattleEnd`에서 처리됩니다.
        }
    }

    private void ProceedToNextRound()
    {
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

    bool IsAugmentRound(int stage, int round)
    {
        // 증강 선택 라운드: 2-1, 3-2, 4-2
        if ((stage == 2 && round == 1) || (stage == 3 && round == 2) || (stage == 4 && round == 2))
        {
            return true;
        }
        return false;
    }
    #endregion

    #region 매칭 로직
    private void GenerateMatchups()
    {
        // 플레이어 리스트를 섞습니다.
        ShufflePlayers();

        matchups = new List<(GameObject, GameObject)>();

        // 플레이어 수에 따라 매칭을 생성합니다.
        int playerCount = players.Count;

        // 플레이어 수가 홀수인 경우 유령 플레이어 또는 바이(bye)를 처리해야 합니다.
        for (int i = 0; i < playerCount - 1; i += 2)
        {
            GameObject player1 = players[i];
            GameObject player2 = players[i + 1];
            matchups.Add((player1, player2));
            Manager.Battle.MovePlayer(player1, player2);
        }

        // 플레이어 수가 홀수인 경우 마지막 플레이어 처리
        if (playerCount % 2 != 0)
        {
            GameObject lastPlayer = players[playerCount - 1];
            // 유령 플레이어와 매칭하거나, 랜덤으로 다른 플레이어와 매칭
            // 여기서는 간단히 랜덤 플레이어와 다시 매칭
            int randomIndex = Random.Range(0, playerCount - 1);
            GameObject randomPlayer = players[randomIndex];
            matchups.Add((lastPlayer, randomPlayer));
        }

        AllPlayerStartBattle();
    }

    private void ShufflePlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            GameObject temp = players[i];
            int randomIndex = Random.Range(i, players.Count);
            players[i] = players[randomIndex];
            players[randomIndex] = temp;
        }
    }

    private void AllPlayerStartBattle()
    {
        foreach (var player in Manager.Game.PlayerListObject)
        {
            Player playerScript = player.GetComponent<Player>();

            if (playerScript != null)
                playerScript.SetBattleUser();
        }
    }

    #endregion

    #region 전투 스테이지 로직
    private void StartAllBattles()
    {
        ongoingBattles = 0; // 전투 시작 전에 초기화

        foreach (var matchup in matchups)
        {
            GameObject player1 = matchup.Item1;
            GameObject player2 = matchup.Item2;

            // 각 플레이어의 전투를 시작
            Manager.Battle.StartBattle(player1, player2, roundDuration);

            // 진행 중인 전투 수 증가
            ongoingBattles++;
            //Debug.Log($"전투 시작: {player1.GetComponent<Player>().UserData.UserName} vs {player2.GetComponent<Player>().UserData.UserName}, 진행 중인 전투 수: {ongoingBattles}");
            Debug.Log($"전투 시작: {player1.name} vs {player2.name} , 진행 중인 전투 수: {ongoingBattles}");

            Player p1 = player1.GetComponent<Player>();
            Player p2 = player2.GetComponent<Player>();

            foreach(var champion in p1.UserData.BattleChampionObject)
            {
                ChampionBase cBase = champion.GetComponent<ChampionBase>();

                cBase.ChampionAttackController.EnemyPlayer = p2;
                cBase.ChampionStateController.ChangeState(ChampionState.Move, cBase);
            }

            foreach (var champion in p2.UserData.BattleChampionObject)
            {
                ChampionBase cBase = champion.GetComponent<ChampionBase>();

                cBase.ChampionAttackController.EnemyPlayer = p1;
                cBase.ChampionStateController.ChangeState(ChampionState.Move, cBase);

            }


            MergeScene.BatteStart = true;
        }
    }

    public void OnBattleEnd(GameObject player1, GameObject player2, bool player1Won, int survivingEnemyUnits)
    {
        // 패배한 플레이어 결정
        GameObject losingPlayer = player1Won ? player2 : player1;
        GameObject winningPlayer = player1Won ? player1 : player2;

        // 패배한 플레이어에게 데미지 적용
        ApplyDamage(losingPlayer, survivingEnemyUnits);
        // 플레이어의 체력이 0 이하인지 확인하고 탈락 처리
        CheckPlayerElimination(losingPlayer);

        // 진행 중인 전투 수 감소 (전투당 한 번만 감소)
        ongoingBattles--;
        //Debug.Log($"전투 종료: {player1.GetComponent<Player>().UserData.UserName} vs {player2.GetComponent<Player>().UserData.UserName}, 진행 중인 전투 수: {ongoingBattles}");

        // 모든 전투가 종료되었는지 확인
        if (AllBattlesFinished())
        {
            DistributeGoldToPlayers();
            // 라운드 증가 및 다음 라운드 진행
            ProceedToNextRound();
        }

        Player p1 = player1.GetComponent<Player>();
        Player p2 = player2.GetComponent<Player>();

        foreach (var champion in p1.UserData.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
            cBase.ChampionAttackController.EnemyPlayer = null;
        }

        foreach (var champion in p2.UserData.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
            cBase.ChampionAttackController.EnemyPlayer = null;
        }
        CameraManager.Instance.MoveCameraToPlayer(AllPlayers[0].GetComponent<Player>());
    }


    private bool AllBattlesFinished()
    {
        return ongoingBattles <= 0;
    }
    #endregion

    #region 유저 로직
    private void CheckPlayerElimination(GameObject player)
    {
        int playerHealth = player.GetComponent<Player>().UserData.UserHealth;
        if (playerHealth <= 0)
        {
            // 플레이어 탈락 처리
            // 탈락한 플레이어를 리스트에서 제거
            players.Remove(player);
        }
    }

    void ApplyDamage(GameObject player, int survivingEnemyUnits)
    {
        int index = currentStage - 1;
        int totalDamage = baseDamages[index] + (damagePerEnemyUnit[index] * survivingEnemyUnits);

        // 플레이어에게 데미지 적용
        int hp = player.GetComponent<Player>().UserData.UserHealth;
        hp -= totalDamage;
        player.GetComponent<Player>().UserData.UserHealth = hp;

        // 체력바 업데이트
        Manager.UserHp.UpdateHealthBars();

        // 게임 오버 체크
        if (hp <= 0)
        {
            // Debug.Log($"{player.GetComponent<Player>().PlayerName} 탈락!");
            // 플레이어 탈락 처리
        }
    }



    void DisplayCurrentStageAndRound()
    {
       // Debug.Log($"현재 스테이지: {currentStage}, 현재 라운드: {currentRound}");
    }


    public GameObject FindMatchup(GameObject player)
    {
        foreach (var (player1, player2) in matchups)
        {
            if (player1 == player)
                return player2;

            if (player2 == player)
                return player1;

        }
        return null;
    }


    public Player GetOpponentData(GameObject player)
    {
        GameObject opponent = FindMatchup(player);
        if (opponent != null)
        {
            Player playerScript = opponent.GetComponent<Player>();
            return playerScript;
        }
        return null;
    }


    #endregion

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
                //playerObj.transform.SetParent(null);

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
                tile.championOnTile.Add(CripPrefab);

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
                if (tile.championOnTile != null && tile.isOccupied)
                {
                    foreach (GameObject unit in tile.championOnTile)
                    {
                        if (unit != null)
                        {
                            Crip crip = unit.GetComponent<Crip>();
                            if (crip != null)
                            {
                                remainingCrips.Add(crip);
                            }
                        }
                    }
                }
            }
            int survivingCrips = remainingCrips.Count;

            // 남아있는 크립 파괴 및 타일 상태 업데이트
            foreach (Crip crip in remainingCrips)
            {
                if (crip.currentTile != null)
                {
                    HexTile currentTile = crip.currentTile;

                    // 타일의 championsOnTile 리스트에서 크립 제거
                    currentTile.championOnTile.Remove(crip.gameObject);

                    // 필요에 따라 타일의 점유 상태 업데이트 (IsOccupied 프로퍼티 사용 시 생략 가능)
                    // currentTile.isOccupied = currentTile.championsOnTile.Count > 0;
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
        DistributeGoldToPlayers();
        DistributeExp();
    }

    #endregion

    #region AI실행
    private void PerformAIActions()
    {
        foreach (AIPlayer aiPlayer in aiPlayers)
        {
            aiPlayer.PerformActions(aiPlayer.AiPlayerComponent);
        }
    }
    #endregion

    #region 그리드 로직
    public List<GameObject> GetChampionsWithinOneTile(GameObject champion, UserData user)
    {
        List<GameObject> champions = new List<GameObject>();

        Vector3 championPosition = champion.transform.position;
        ChampionBase cBase = champion.GetComponent<ChampionBase>();

        HexTile nearestTile = null;
        float minDistance = float.MaxValue;

        foreach (HexTile tile in _mapGenerator.mapInfos[cBase.BattleStageIndex].HexDictionary.Values)
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
            return champions;
        }

        int q = nearestTile.q;
        int r = nearestTile.r;

        // 짝수 행
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
            if (_mapGenerator.mapInfos[cBase.BattleStageIndex].HexDictionary.TryGetValue((neighborQ, neighborR), out HexTile neighborTile))
            {
                if (neighborTile.itemOnTile != null && neighborTile.itemOnTile != champion)
                {
                    champions.Add(neighborTile.itemOnTile);
                }
            }
        }

        return champions;
    }

    public List<HexTile> FindShortestPath(GameObject champion1, GameObject champion2)
    {
        List<HexTile> path = new List<HexTile>();

        ChampionBase cBase1 = champion1.GetComponent<ChampionBase>();
        ChampionBase cBase2 = champion2.GetComponent<ChampionBase>();

        HexTile startTile = FindNearestTile(champion1, cBase1.BattleStageIndex);
        HexTile targetTile = FindNearestTile(champion2, cBase2.BattleStageIndex);

        // A* 알고리즘을 위한 우선순위 큐 및 거리 정보 초기화
        PriorityQueue<HexTile> priorityQueue = new PriorityQueue<HexTile>();
        Dictionary<HexTile, HexTile> cameFrom = new Dictionary<HexTile, HexTile>();
        Dictionary<HexTile, float> costSoFar = new Dictionary<HexTile, float>();

        priorityQueue.Enqueue(startTile, 0);
        cameFrom[startTile] = null;
        costSoFar[startTile] = 0;

        while (priorityQueue.Count > 0)
        {
            HexTile currentTile = priorityQueue.Dequeue();

            // 목표 타일에 도달한 경우 경로 추적
            if (currentTile == targetTile)
            {
                while (currentTile != startTile)
                {
                    path.Add(currentTile);
                    currentTile = cameFrom[currentTile];
                }
                path.Reverse();
                return path;
            }

            // 인접 타일 검사
            foreach (HexTile neighbor in GetNeighbors(currentTile, cBase1.BattleStageIndex))
            {
                float newCost = costSoFar[currentTile] + 1; // 기본 이동 비용은 1로 설정

                if ((!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor]) &&
                    (neighbor.championOnTile.Count == 0 || neighbor == targetTile))
                {
                    costSoFar[neighbor] = newCost;
                    float priority = newCost + Vector3.Distance(neighbor.transform.position, targetTile.transform.position);
                    priorityQueue.Enqueue(neighbor, priority);
                    cameFrom[neighbor] = currentTile;
                }
            }
        }

        return path;
    }

    // 이웃 타일
    private List<HexTile> GetNeighbors(HexTile tile, int index)
    {
        List<HexTile> neighbors = new List<HexTile>();
        int q = tile.q;
        int r = tile.r;

        // 짝수, 홀수
        bool isEvenRow = (r % 2) == 0;
        (int dq, int dr)[] directions = isEvenRow
            ? new (int, int)[] { (1, 0), (1, -1), (0, -1), (-1, 0), (0, 1), (1, 1) }
            : new (int, int)[] { (1, 0), (0, -1), (-1, -1), (-1, 0), (-1, 1), (0, 1) };

        foreach (var (dq, dr) in directions)
        {
            int neighborQ = q + dq;
            int neighborR = r + dr;
            if (_mapGenerator.mapInfos[index].HexDictionary.TryGetValue((neighborQ, neighborR), out HexTile neighborTile))
            {
                neighbors.Add(neighborTile);
            }
        }

        return neighbors;
    }

    // 챔피언 가장 가까운 타일
    public HexTile FindNearestTile(GameObject champion, int index)
    {
        Vector3 championPosition = champion.transform.position;
        HexTile nearestTile = null;
        float minDistance = float.MaxValue;

        foreach (HexTile tile in _mapGenerator.mapInfos[index].HexDictionary.Values)
        {
            float distance = Vector3.Distance(championPosition, tile.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTile = tile;
            }
        }

        return nearestTile;
    }


    public void SetNearestTile(GameObject champion)
    {
        Vector3 championPosition = champion.transform.position;
        HexTile nearestTile = null;
        float minDistance = float.MaxValue;
        ChampionBase cBase = champion.GetComponent<ChampionBase>();

        foreach (HexTile tile in _mapGenerator.mapInfos[cBase.BattleStageIndex].HexDictionary.Values)
        {
            float distance = Vector3.Distance(championPosition, tile.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTile = tile;
            }
        }

        if(!nearestTile.championOnTile.Contains(champion))
        {
            nearestTile.championOnTile.Add(champion);
        }
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

    #region
    public void DistributeExp()
    {
        foreach (GameObject playerObj in AllPlayers)
        {
            Player playerComponent = playerObj.GetComponent<Player>();
            UserData userData = playerComponent.UserData;
            Manager.Level.AddExperience(userData, 2);
        }
    }
    #endregion

    #region 골드 분배 로직
    public void DistributeGoldToPlayers()
    {
        foreach (GameObject playerObj in AllPlayers)
        {
            Player playerComponent = playerObj.GetComponent<Player>();
            UserData userData = playerComponent.UserData;

            int baseGold = GetBaseGold(currentStage, currentRound);
            int interestGold = GetInterestGold(userData.UserGold);
            int streakGold = GetStreakGold(userData);

            int totalGold = baseGold + interestGold + streakGold;

            // 플레이어의 골드 업데이트
            userData.UserGold += totalGold;
            

            // 필요하다면 골드 지급 내용을 로그로 출력
            Debug.Log($"{userData.UserName} 님에게 총 {totalGold} 골드가 지급되었습니다. (기본: {baseGold}, 이자: {interestGold}, 연승/연패 보너스: {streakGold})");
        }
    }

    int GetBaseGold(int stage, int round)
    {
        // 2-1 라운드까지는 3골드, 2-2 라운드부터는 5골드 지급
        if (stage == 2 && round >= 2)
            return 5;
        else if (stage > 2)
            return 5;
        else
            return 3;
    }

    int GetInterestGold(int currentGold)
    {
        if (currentGold >= 50)
            return 5;
        else if (currentGold >= 40)
            return 4;
        else if (currentGold >= 30)
            return 3;
        else if (currentGold >= 20)
            return 2;
        else if (currentGold >= 10)
            return 1;
        else
            return 0;
    }

    int GetStreakGold(UserData userData)
    {
        int winStreak = userData.UserSuccessiveWin;
        int loseStreak = userData.UserSuccessiveLose;
        int streakCount = Mathf.Max(winStreak, loseStreak);

        if (streakCount >= 6)
            return 3;
        else if (streakCount == 5)
            return 2;
        else if (streakCount >= 4)
            return 1;
        else
            return 0;
    }
    #endregion
}