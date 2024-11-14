using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static MapGenerator;

public class StageManager
{
    public GameObject[] AllPlayers; // �� 8���� �÷��̾� (�ڱ� �ڽ� ����)
    private List<GameObject> players; // ��� �÷��̾� ���
    private List<(GameObject, GameObject)> matchups; // ��Ī�� �÷��̾� ��� ���
    private int ongoingBattles = 0; // ���� ���� ���� ���� �����ϴ� ���� �߰�

    private MapGenerator _mapGenerator;

    public int currentStage = 1;
    public int currentRound = 1;

    // ���������� �⺻ ���ط��� ������ �� ���ִ� ���ط�
    public int[] baseDamages = new int[] { 0, 2, 5, 8, 10, 12, 17 }; // �ε����� �������� ��ȣ - 1
    public int[] damagePerEnemyUnit = new int[] { 1, 2, 3, 4, 5, 6, 7 }; // �ε����� �������� ��ȣ - 1

    // ���� ���ð� ����
    private int normalWaitTime = 3; //���� �� ���ð�
    private int augmentWaitTime = 5; //���� ���� ���� �ð�
    private int postMatchWaitTime = 3; //��ġ �� ���ð�
    private int roundDuration = 3; //�Ϲ� ���� ����ð�

    private bool isAugmentRound = false;

    private Coroutine roundCoroutine;

    private GameObject CripPrefab;

    private List<AIPlayer> aiPlayers = new List<AIPlayer>(); // AIPlayer �ν��Ͻ� ����Ʈ
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
        // �ڱ� �ڽŰ� ��� �÷��̾� �и�
        // ���⼭�� ù ��° �÷��̾ �ڱ� �ڽ����� ����

        players = new List<GameObject>(AllPlayers);

        // AI �÷��̾� �ʱ�ȭ
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

    #region �������� ����
    private void StartStage(int stageNumber)
    {
        currentRound = 1;

        if (roundCoroutine != null)
            CoroutineHelper.StopCoroutine(roundCoroutine);

        roundCoroutine = CoroutineHelper.StartCoroutine(StartRoundCoroutine());
    }

    IEnumerator StartRoundCoroutine()
    {
        // UI ������Ʈ
        UIManager.Instance.UpdateStageRoundUI(currentStage, currentRound);

        // ���� ���� ���� ���� Ȯ��
        isAugmentRound = IsAugmentRound(currentStage, currentRound);

        // ���� ���� ���� ���� Ȯ��
        bool isCarouselRound = IsCarouselRound(currentStage, currentRound);

        // ũ�� ���� ���� Ȯ��
        bool isCripRound = IsCripRound(currentStage, currentRound);

        // ���� �� ���ð� ����
        int preWaitTime = isAugmentRound ? augmentWaitTime : normalWaitTime;

        // **1. ���� �� ���ð�**

        // ��� �ð� ���� AI �ൿ ����
        PerformAIActions();

        // ���� �� ���ð� Ÿ�̸� ����
        UIManager.Instance.StartTimer(preWaitTime);

        // ���� �� ���ð� ���� ���
        yield return new WaitForSeconds(preWaitTime);

        // **2. ��ġ �� ���ð� �� ���� ����**

        if (isCarouselRound)
        {
            // ���� ���� ���� ó��
            yield return CoroutineHelper.StartCoroutine(StartCarouselRound());
            yield break; // ���� ���� ����� ������ �帧�̹Ƿ� ���⼭ ����
        }
        else if (isCripRound)
        {
            // ũ�� ������ ���

            // ũ�� ����
            SpawnCrips();

            // ��ġ �� ���ð� Ÿ�̸� ����
            UIManager.Instance.StartTimer(postMatchWaitTime);

            // ��ġ �� ���ð� ���� ���
            yield return new WaitForSeconds(postMatchWaitTime);

            // **3. �Ϲ� ���� ����ð�**

            // ũ�� ���� ����
            //StartAllCripBattles();

            // ���� ���� �ð� Ÿ�̸� ����
            UIManager.Instance.StartTimer(roundDuration);

            // ���� ���� �ð� ���� ���
            yield return new WaitForSeconds(roundDuration);

            // ���� ���� ó��
            EndCripRound();
        }
        else if (isAugmentRound)
        {
            // ���� ���� ������ ���

            // ��Ī �� ���� �̵� ����
            GenerateMatchups();

            // ��ġ �� ���ð� Ÿ�̸� ����
            UIManager.Instance.StartTimer(postMatchWaitTime);

            // ��ġ �� ���ð� ���� ���
            yield return new WaitForSeconds(postMatchWaitTime);

            // **3. �Ϲ� ���� ����ð�**

            // ���� ����
            StartAllBattles();

            // ���� ���� �ð� Ÿ�̸� ����
            UIManager.Instance.StartTimer(roundDuration);

            // ���� ���� �ð� ���� ���
            yield return new WaitForSeconds(roundDuration);

            // ���� ����� �� �÷��̾��� ������ ����� ������ `OnBattleEnd`���� ó���˴ϴ�.
        }
        else
        {
            // �Ϲ� ������ ���

            // ��Ī �� ���� �̵� ����
            GenerateMatchups();

            // ��ġ �� ���ð� Ÿ�̸� ����
            UIManager.Instance.StartTimer(postMatchWaitTime);

            // ��ġ �� ���ð� ���� ���
            yield return new WaitForSeconds(postMatchWaitTime);

            // **3. �Ϲ� ���� ����ð�**

            // ���� ����
            StartAllBattles();

            // ���� ���� �ð� Ÿ�̸� ����
            UIManager.Instance.StartTimer(roundDuration);

            // ���� ���� �ð� ���� ���
            yield return new WaitForSeconds(roundDuration);

            // ���� ����� �� �÷��̾��� ������ ����� ������ `OnBattleEnd`���� ó���˴ϴ�.
        }
    }

    private void ProceedToNextRound()
    {
        currentRound++;

        int maxRounds = currentStage == 1 ? 3 : 7;

        if (currentRound > maxRounds)
        {
            // ���� ���������� �̵�
            currentStage++;
            if (currentStage > 8)
            {
                // ���� ����
                // Debug.Log("���� Ŭ����!");
                return;
            }
            StartStage(currentStage);
        }
        else
        {
            // ���� ���� ����
            if (roundCoroutine != null)
                CoroutineHelper.StopCoroutine(roundCoroutine);
            roundCoroutine = CoroutineHelper.StartCoroutine(StartRoundCoroutine());
        }
    }

    bool IsAugmentRound(int stage, int round)
    {
        // ���� ���� ����: 2-1, 3-2, 4-2
        if ((stage == 2 && round == 1) || (stage == 3 && round == 2) || (stage == 4 && round == 2))
        {
            return true;
        }
        return false;
    }
    #endregion

    #region ��Ī ����
    private void GenerateMatchups()
    {
        // �÷��̾� ����Ʈ�� �����ϴ�.
        ShufflePlayers();

        matchups = new List<(GameObject, GameObject)>();

        // �÷��̾� ���� ���� ��Ī�� �����մϴ�.
        int playerCount = players.Count;

        // �÷��̾� ���� Ȧ���� ��� ���� �÷��̾� �Ǵ� ����(bye)�� ó���ؾ� �մϴ�.
        for (int i = 0; i < playerCount - 1; i += 2)
        {
            GameObject player1 = players[i];
            GameObject player2 = players[i + 1];
            matchups.Add((player1, player2));
            Manager.Battle.MovePlayer(player1, player2);
        }

        // �÷��̾� ���� Ȧ���� ��� ������ �÷��̾� ó��
        if (playerCount % 2 != 0)
        {
            GameObject lastPlayer = players[playerCount - 1];
            // ���� �÷��̾�� ��Ī�ϰų�, �������� �ٸ� �÷��̾�� ��Ī
            // ���⼭�� ������ ���� �÷��̾�� �ٽ� ��Ī
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

    #region ���� �������� ����
    private void StartAllBattles()
    {
        ongoingBattles = 0; // ���� ���� ���� �ʱ�ȭ

        foreach (var matchup in matchups)
        {
            GameObject player1 = matchup.Item1;
            GameObject player2 = matchup.Item2;

            // �� �÷��̾��� ������ ����
            Manager.Battle.StartBattle(player1, player2, roundDuration);

            // ���� ���� ���� �� ����
            ongoingBattles++;
            //Debug.Log($"���� ����: {player1.GetComponent<Player>().UserData.UserName} vs {player2.GetComponent<Player>().UserData.UserName}, ���� ���� ���� ��: {ongoingBattles}");
            Debug.Log($"���� ����: {player1.name} vs {player2.name} , ���� ���� ���� ��: {ongoingBattles}");

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
        // �й��� �÷��̾� ����
        GameObject losingPlayer = player1Won ? player2 : player1;
        GameObject winningPlayer = player1Won ? player1 : player2;

        // �й��� �÷��̾�� ������ ����
        ApplyDamage(losingPlayer, survivingEnemyUnits);
        // �÷��̾��� ü���� 0 �������� Ȯ���ϰ� Ż�� ó��
        CheckPlayerElimination(losingPlayer);

        // ���� ���� ���� �� ���� (������ �� ���� ����)
        ongoingBattles--;
        //Debug.Log($"���� ����: {player1.GetComponent<Player>().UserData.UserName} vs {player2.GetComponent<Player>().UserData.UserName}, ���� ���� ���� ��: {ongoingBattles}");

        // ��� ������ ����Ǿ����� Ȯ��
        if (AllBattlesFinished())
        {
            DistributeGoldToPlayers();
            // ���� ���� �� ���� ���� ����
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

    #region ���� ����
    private void CheckPlayerElimination(GameObject player)
    {
        int playerHealth = player.GetComponent<Player>().UserData.UserHealth;
        if (playerHealth <= 0)
        {
            // �÷��̾� Ż�� ó��
            // Ż���� �÷��̾ ����Ʈ���� ����
            players.Remove(player);
        }
    }

    void ApplyDamage(GameObject player, int survivingEnemyUnits)
    {
        int index = currentStage - 1;
        int totalDamage = baseDamages[index] + (damagePerEnemyUnit[index] * survivingEnemyUnits);

        // �÷��̾�� ������ ����
        int hp = player.GetComponent<Player>().UserData.UserHealth;
        hp -= totalDamage;
        player.GetComponent<Player>().UserData.UserHealth = hp;

        // ü�¹� ������Ʈ
        Manager.UserHp.UpdateHealthBars();

        // ���� ���� üũ
        if (hp <= 0)
        {
            // Debug.Log($"{player.GetComponent<Player>().PlayerName} Ż��!");
            // �÷��̾� Ż�� ó��
        }
    }



    void DisplayCurrentStageAndRound()
    {
       // Debug.Log($"���� ��������: {currentStage}, ���� ����: {currentRound}");
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

    #region �������� ����
    IEnumerator StartCarouselRound()
    {
        //Debug.Log("���� ���� ���尡 ���۵˴ϴ�!");

        // ī�޶� ���� ���� ������ �̵�
        

        // ��� �÷��̾��� �������� �Ͻ������� ��Ȱ��ȭ
        DisableAllPlayerMovement();

        foreach (GameObject playerObj in AllPlayers)
        {
            PlayerMove playerMove = playerObj.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                playerMove.StartCarouselRound(_mapGenerator.sharedSelectionMapTransform, _mapGenerator.sharedMapInfo);
            }
        }

        // �÷��̾���� ü�� ������� ����
        List<GameObject> sortedPlayers = GetPlayersSortedByHealth();

        int playersPerInterval = 2; // �� ���� ������ �� �ִ� �÷��̾� ��
        int intervalWaitTime = 6;   // ���� �׷��� �����̱������ ��� �ð�

        // �÷��̾���� ���� ���� �� ��ġ�� �̵�
        

        for (int i = 0; i < sortedPlayers.Count; i += playersPerInterval)
        {
            // ���� �׷��� �÷��̾��
            List<GameObject> currentGroup = sortedPlayers.GetRange(i, Mathf.Min(playersPerInterval, sortedPlayers.Count - i));

            // �ش� �÷��̾���� ������ Ȱ��ȭ
            EnablePlayerMovement(currentGroup);

            // ��� �ð�
            if (i + playersPerInterval < sortedPlayers.Count)
            {
                yield return new WaitForSeconds(intervalWaitTime);
            }
        }

        // ��� �÷��̾ è�Ǿ��� ������ ������ ��� (�����ϰ� ���� �ð� ����ϵ��� ����)
        float totalCarouselDuration = 30f; // ��ü ���� ���� ���� �ð�
        yield return new WaitForSeconds(totalCarouselDuration);

        // ���� ���� ���� ���� ó��
        EndCarouselRound();
    }

    bool IsCarouselRound(int stage, int round)
    {
        // 2������������ �� 4���帶�� ���� ���� ����
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

                // AI �÷��̾��� ��� �ڵ����� è�Ǿ��� �����ϵ��� ó��
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

            // ü���� ���� ������� ����
            return healthA.CompareTo(healthB);
        });

        return sortedPlayers;
    }
    void EndCarouselRound()
    {
       // Debug.Log("���� ���� ���尡 ����Ǿ����ϴ�.");

        // ��� �÷��̾�鿡�� ���� ���� ���� ���Ḧ �˸��� ���� ��ġ�� ����
        foreach (GameObject playerObj in AllPlayers)
        {
            PlayerMove playerMove = playerObj.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                // �θ� �ٽ� ���� (�ֻ��� �Ǵ� ���� �θ��)
                //playerObj.transform.SetParent(null);

                // ���� ��ġ�� ����
                playerMove.ReturnToOriginalPosition();

                // �ʿ��� ��� �߰����� �ʱ�ȭ ����
                playerMove.EndCarouselRound();
                
            }
        }
        CameraManager.Instance.MoveCameraToPlayer(AllPlayers[0].GetComponent<Player>());
        // ���� ���� ���� ����
        // ���� ����� �̵�
        currentRound++;

        int maxRounds = currentStage == 1 ? 3 : 7;

        if (currentRound > maxRounds)
        {
            // ���� ���������� �̵�
            currentStage++;
            if (currentStage > 8)
            {
                // ���� ����
               // Debug.Log("���� Ŭ����!");
                return;
            }
            StartStage(currentStage);
        }
        else
        {
            // ���� ���� ����
            if (roundCoroutine != null)
                CoroutineHelper.StopCoroutine(roundCoroutine);
            roundCoroutine = CoroutineHelper.StartCoroutine(StartRoundCoroutine());
        }
    }
    #endregion

    #region �÷��̾� �� ���� �޾ƿ���
    private MapInfo GetPlayerMapInfo(Player playerComponent)
    {
        // MapGenerator���� �÷��̾��� MapInfo�� ã���ϴ�.
        foreach (var mapInfo in _mapGenerator.mapInfos)
        {
            if (mapInfo.playerData == playerComponent)
            {
                return mapInfo;
            }
        }

        // ã�� ���� ��� null ��ȯ
        return null;
    }
    #endregion

    #region ũ������
    bool IsCripRound(int stage, int round)
    {
        // �������� 1�� 1, 2, 3 ����� 2������������ �� 7���帶�� ũ�� ����
        if (stage == 1 && (round == 1 || round == 2 || round == 3))
            return true;
        else if (stage >= 2 && round == 7)
            return true;
        else
            return false;
    }
    IEnumerator StartCripRound()
    {
        // ���� ���� �ð� Ÿ�̸� ����
        UIManager.Instance.StartTimer(roundDuration);

        // ���� ���� (ũ������ ���� ������ �����ؾ� �մϴ�)
        //Manager.Battle.StartCripBattle(selfPlayer, roundDuration);

        // ���� ���� �ð���ŭ ���
        yield return new WaitForSeconds(roundDuration);

        // ���� ���� ó��
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
                Debug.LogWarning($"�÷��̾� {playerComponent.UserData.UserName}�� MapInfo�� ã�� �� �����ϴ�.");
                continue;
            }

            // ����� �� �ִ� Ÿ�� ����� �����մϴ�.
            List<HexTile> availableTiles = new List<HexTile>();

            foreach (var tileEntry in playerMapInfo.HexDictionary)
            {
                HexTile tile = tileEntry.Value;
                // Ÿ���� "EnemyTile" �±׸� ������ �ְ�, ����ִ� ��쿡�� �߰�
                if (tile.CompareTag("EnemyTile") && !tile.isOccupied)
                {
                    availableTiles.Add(tile);
                }
            }

            if (availableTiles.Count == 0)
            {
                Debug.LogWarning($"�÷��̾� {playerComponent.UserData.UserName}�� �ʿ� ũ���� ������ �� �ִ� Ÿ���� �����ϴ�.");
                continue;
            }

            // ������ ũ���� ���� �����մϴ�.
            int numCripsToSpawn = GetNumberOfCripsToSpawn(currentStage, currentRound);

            // ������ �� �ִ� �ִ� ũ�� ���� ����մϴ�.
            int cripsToSpawn = Mathf.Min(numCripsToSpawn, availableTiles.Count);

            // Ÿ�� ����� �����ϴ�.
            for (int i = 0; i < availableTiles.Count; i++)
            {
                HexTile temp = availableTiles[i];
                int randomIndex = Random.Range(i, availableTiles.Count);
                availableTiles[i] = availableTiles[randomIndex];
                availableTiles[randomIndex] = temp;
            }

            // �ʿ��� ����ŭ�� Ÿ�Ͽ� ũ���� �����մϴ�.
            for (int i = 0; i < cripsToSpawn; i++)
            {
                HexTile tile = availableTiles[i];

                CripPrefab = Manager.Asset.InstantiatePrefab("Crip", tile.transform);
                CripPrefab.transform.position = tile.transform.position + new Vector3(0, 0.5f, 0);


                // Ÿ�Ͽ� ũ���� �����մϴ�.
                tile.championOnTile.Add(CripPrefab);

                // ũ���� ���� Ÿ�� ������ �����մϴ�.
                Crip cripComponent = CripPrefab.GetComponent<Crip>();
                if (cripComponent != null)
                {
                    cripComponent.currentTile = tile;
                    cripComponent.playerMapInfo = playerMapInfo;
                }

                // ũ���� �θ� �����Ͽ� �� ������ ���Եǵ��� �մϴ�.
                CripPrefab.transform.SetParent(tile.transform);
            }
        }
    }

    int GetNumberOfCripsToSpawn(int stage, int round)
    {
        // ���������� ���忡 ���� ������ ũ���� ���� �����մϴ�.

        if (stage == 1)
            return 3; // ���÷� �������� 1������ 3���� ����
        else if (stage == 2)
            return 4; // �������� 2������ 4���� ����
        else
            return 5; // �� �ܿ��� 5���� ����
    }

    void EndCripRound()
    {
        foreach (GameObject player in AllPlayers)
        {
            Player playerComponent = player.GetComponent<Player>();
            MapInfo playerMapInfo = GetPlayerMapInfo(playerComponent);

            if (playerMapInfo == null)
            {
                Debug.LogWarning($"�÷��̾� {playerComponent.UserData.UserName}�� MapInfo�� ã�� �� �����ϴ�.");
                continue;
            }

            // �ش� �÷��̾��� �ʿ� �����ִ� ũ���� ã���ϴ�.
            List<Crip> remainingCrips = new List<Crip>();

            // �÷��̾��� �ʿ� �ִ� ��� Ÿ���� �˻��մϴ�.
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

            // �����ִ� ũ�� �ı� �� Ÿ�� ���� ������Ʈ
            foreach (Crip crip in remainingCrips)
            {
                if (crip.currentTile != null)
                {
                    HexTile currentTile = crip.currentTile;

                    // Ÿ���� championsOnTile ����Ʈ���� ũ�� ����
                    currentTile.championOnTile.Remove(crip.gameObject);

                    // �ʿ信 ���� Ÿ���� ���� ���� ������Ʈ (IsOccupied ������Ƽ ��� �� ���� ����)
                    // currentTile.isOccupied = currentTile.championsOnTile.Count > 0;
                }
                crip.Death(); // OnDeath() �Լ��� ȣ���Ͽ� ������ ���� �� ũ�� �ı� ó��
            }

            // �÷��̾� �¸� ���� �Ǵ�
            bool playerWon = survivingCrips == 0;

            // �� �÷��̾�� ���� ���� ó��
            //OnRoundEndForPlayer(playerComponent, playerWon, survivingCrips);
        }
        currentRound++;

        int maxRounds = currentStage == 1 ? 3 : 7;

        if (currentRound > maxRounds)
        {
            // ���� ���������� �̵�
            currentStage++;
            if (currentStage > 8)
            {
                // ���� ����
                // Debug.Log("���� Ŭ����!");
                return;
            }
            StartStage(currentStage);
        }
        else
        { 
            // ���� ���� ����
            if (roundCoroutine != null)
                CoroutineHelper.StopCoroutine(roundCoroutine);
            roundCoroutine = CoroutineHelper.StartCoroutine(StartRoundCoroutine());
        }
        DistributeGoldToPlayers();
        DistributeExp();
    }

    #endregion

    #region AI����
    private void PerformAIActions()
    {
        foreach (AIPlayer aiPlayer in aiPlayers)
        {
            aiPlayer.PerformActions(aiPlayer.AiPlayerComponent);
        }
    }
    #endregion

    #region �׸��� ����
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

        // ¦�� ��
        bool isEvenRow = (r % 2) == 0;
        (int dq, int dr)[] directions;

        if (isEvenRow)
        {
            // ¦�� ���� ����
            directions = new (int, int)[]
            {
            (1, 0),    // ����
            (1, -1),   // ������
            (0, -1),   // ������
            (-1, 0),   // ����
            (0, 1),    // �ϼ���
            (1, 1)     // �ϵ���
            };
        }
        else
        {
            // Ȧ�� ���� ����
            directions = new (int, int)[]
            {
            (1, 0),    // ����
            (0, -1),   // ������
            (-1, -1),  // ������
            (-1, 0),   // ����
            (-1, 1),   // �ϼ���
            (0, 1)     // �ϵ���
            };
        }

        foreach (var dir in directions)
        {
            int neighborQ = q + dir.dq;
            int neighborR = r + dir.dr;

            // ���� Ÿ���� �����ϴ��� Ȯ��
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

        // A* �˰����� ���� �켱���� ť �� �Ÿ� ���� �ʱ�ȭ
        PriorityQueue<HexTile> priorityQueue = new PriorityQueue<HexTile>();
        Dictionary<HexTile, HexTile> cameFrom = new Dictionary<HexTile, HexTile>();
        Dictionary<HexTile, float> costSoFar = new Dictionary<HexTile, float>();

        priorityQueue.Enqueue(startTile, 0);
        cameFrom[startTile] = null;
        costSoFar[startTile] = 0;

        while (priorityQueue.Count > 0)
        {
            HexTile currentTile = priorityQueue.Dequeue();

            // ��ǥ Ÿ�Ͽ� ������ ��� ��� ����
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

            // ���� Ÿ�� �˻�
            foreach (HexTile neighbor in GetNeighbors(currentTile, cBase1.BattleStageIndex))
            {
                float newCost = costSoFar[currentTile] + 1; // �⺻ �̵� ����� 1�� ����

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

    // �̿� Ÿ��
    private List<HexTile> GetNeighbors(HexTile tile, int index)
    {
        List<HexTile> neighbors = new List<HexTile>();
        int q = tile.q;
        int r = tile.r;

        // ¦��, Ȧ��
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

    // è�Ǿ� ���� ����� Ÿ��
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
            //   Debug.Log("�ֺ��� è�Ǿ��� �����ϴ�.");
            return;
        }

        // Debug.Log($"�ֺ��� �ִ� è�Ǿ� ��: {champions.Count}");
        foreach (var champion in champions)
        {
            if (champion != null)
            {
                //  Debug.Log($"è�Ǿ� �̸�: {champion.name}, ��ġ: {champion.transform.position}");
            }
            else
            {
                // Debug.Log("è�Ǿ��� null�Դϴ�.");
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

    #region ��� �й� ����
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

            // �÷��̾��� ��� ������Ʈ
            userData.UserGold += totalGold;
            

            // �ʿ��ϴٸ� ��� ���� ������ �α׷� ���
            Debug.Log($"{userData.UserName} �Կ��� �� {totalGold} ��尡 ���޵Ǿ����ϴ�. (�⺻: {baseGold}, ����: {interestGold}, ����/���� ���ʽ�: {streakGold})");
        }
    }

    int GetBaseGold(int stage, int round)
    {
        // 2-1 ��������� 3���, 2-2 ������ʹ� 5��� ����
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