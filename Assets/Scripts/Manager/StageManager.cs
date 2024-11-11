using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static MapGenerator;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    private GameDataBlueprint gameDataBlueprint; // ���� ������ �������Ʈ
    #region Init

    public void InitStage(GameObject[] playerData, MapGenerator mapGenerator, GameDataBlueprint gameData)
    {
        AllPlayers = playerData;
        _mapGenerator = mapGenerator;
        gameDataBlueprint = gameData;

        InitializePlayers();
        StartStage(currentStage);
        
    }
    #endregion

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

    void StartStage(int stageNumber)
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

        // **���� ���� ���� ���� Ȯ��**
        bool isCarouselRound = IsCarouselRound(currentStage, currentRound);

        // ũ�� ���� ���� Ȯ��
        bool isCripRound = IsCripRound(currentStage, currentRound);

        // ���ð� ����
        int waitTime = isAugmentRound ? augmentWaitTime : normalWaitTime;

        //  Debug.Log($"���� ���� �� ���ð�: {waitTime}��");

        // **��� �ð� ���� AI �ൿ ����**
        PerformAIActions();

        // ���ð� Ÿ�̸� ����
        UIManager.Instance.StartTimer(waitTime);

        yield return new WaitForSeconds(waitTime);

        if (isCarouselRound)
        {
            // **���� ���� ���� ó��**
            CoroutineHelper.StartCoroutine(StartCarouselRound());
        }
        else if (isCripRound)
        {
            // **��ġ �� ���ð� ���� ũ�� ���� �� �غ�**

            // ��ġ �� ���ð� Ÿ�̸� ����
            UIManager.Instance.StartTimer(postMatchWaitTime);

            // ũ�� ����
            SpawnCrips();

            yield return new WaitForSeconds(postMatchWaitTime);

            // ũ�� ���� ����
            CoroutineHelper.StartCoroutine(StartCripRound());
        }
        else
        {
            // **�Ϲ� ���� ó��**

            // ��� �÷��̾���� ��Ī
            GenerateMatchups();

            //Debug.Log($"{currentOpponent.GetComponent<Player>().PlayerName}�� ��Ī�Ǿ����ϴ�.");

            //��Ī �� ���ð�
            //Debug.Log($"��Ī �� ���ð�: {postMatchWaitTime}��");

            // ��Ī �� ���ð� Ÿ�̸� ����
            UIManager.Instance.StartTimer(postMatchWaitTime);

            yield return new WaitForSeconds(postMatchWaitTime);

            //Debug.Log("���尡 ���۵˴ϴ�!");

            // ���� ����
            StartAllBattles();

            // ���� ���� �ð� Ÿ�̸� ����
            UIManager.Instance.StartTimer(roundDuration);
        }
    }

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
            Debug.Log($"���� ����: {player1.GetComponent<Player>().UserData.UserName} vs {player2.GetComponent<Player>().UserData.UserName}, ���� ���� ���� ��: {ongoingBattles}");
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
            // ���� ���� �� ���� ���� ����
            ProceedToNextRound();
        }
    }

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

    private bool AllBattlesFinished()
    {
        // ���� ���� ���� ���� 0�̸� ��� ������ ����� ��
        return ongoingBattles <= 0;
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


    bool IsAugmentRound(int stage, int round)
    {
        // ���� ���� ����: 2-1, 3-2, 4-2
        if ((stage == 2 && round == 1) || (stage == 3 && round == 2) || (stage == 4 && round == 2))
        {
            return true;
        }
        return false;
    }

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
                playerObj.transform.SetParent(null);

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

    #region ��ĭ�̳� è�Ǿ� ã��
    public List<GameObject> GetChampionsWithinOneTile(GameObject champion)
    {
        List<GameObject> champions = new List<GameObject>();

        // è�Ǿ��� ��ġ ��������
        Vector3 championPosition = champion.transform.position;

        // ���� ����� Ÿ�� ã��
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
            //Debug.LogWarning("��ó�� Ÿ���� �����ϴ�.");
            return champions;
        }

        int q = nearestTile.q;
        int r = nearestTile.r;

        // ¦�� ������ ���� �Ǵ�
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

    }

    #endregion

    #region AI����
    private void PerformAIActions()
    {
        foreach (AIPlayer aiPlayer in aiPlayers)
        {
            aiPlayer.PerformActions();
        }
    }
    #endregion
}