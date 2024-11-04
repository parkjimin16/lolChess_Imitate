using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StageManager
{
    public GameObject[] AllPlayers; // �� 8���� �÷��̾� (�ڱ� �ڽ� ����)
    private List<GameObject> opponents; // ��� �÷��̾� ��� (�ڱ� �ڽ� ����)
    private GameObject selfPlayer; // �ڱ� �ڽ�
    private GameObject currentOpponent; // ���� ���

    private MapGenerator _mapGenerator;

    public int currentStage = 1;
    public int currentRound = 1;

    // ���������� �⺻ ���ط��� ������ �� ���ִ� ���ط�
    public int[] baseDamages = new int[] { 0, 2, 5, 8, 10, 12, 17 }; // �ε����� �������� ��ȣ - 1
    public int[] damagePerEnemyUnit = new int[] { 1, 2, 3, 4, 5, 6, 7 }; // �ε����� �������� ��ȣ - 1

    // ���� ���ð� ����
    private int normalWaitTime = 3; //���� �� ���ð�
    private int augmentWaitTime = 3; //���� ���� ���� �ð�
    private int postMatchWaitTime = 3; //��ġ �� ���ð�
    private int roundDuration = 3; //�Ϲ� ���� ����ð�

    private bool isAugmentRound = false;

    private Coroutine roundCoroutine;

    public GameObject go;
    #region Init

    public void InitStage(GameObject[] playerData, MapGenerator mapGenerator, GameObject Go)
    {
        AllPlayers = playerData;
        _mapGenerator = mapGenerator;
        go = Go;
        InitializePlayers();
        StartStage(currentStage);

    }
    #endregion

    private void InitializePlayers()
    {
        
        // �ڱ� �ڽŰ� ��� �÷��̾� �и�
        // ���⼭�� ù ��° �÷��̾ �ڱ� �ڽ����� ����
        
        selfPlayer = AllPlayers[0];
        
        opponents = new List<GameObject>(AllPlayers);
        opponents.Remove(selfPlayer);

    }

    void StartStage(int stageNumber)
    {
        currentRound = 1;
        ShuffleOpponents(); // ��� ����Ʈ�� ����

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

        // ���ð� ����
        int waitTime = isAugmentRound ? augmentWaitTime : normalWaitTime;

      //  Debug.Log($"���� ���� �� ���ð�: {waitTime}��");

        // ���ð� Ÿ�̸� ����
        UIManager.Instance.StartTimer(waitTime);

        yield return new WaitForSeconds(waitTime);

        if (isCarouselRound)
        {
            // **���� ���� ���� ó��**
            CoroutineHelper.StartCoroutine(StartCarouselRound());
        }
        else
        {
            // **�Ϲ� ���� ó��**

            // ��� ��Ī
            int opponentIndex = (currentRound - 1) % opponents.Count;
            currentOpponent = opponents[opponentIndex];

          //  Debug.Log($"{currentOpponent.GetComponent<Player>().PlayerName}�� ��Ī�Ǿ����ϴ�.");

            // ��Ī �� ���ð�
           // Debug.Log($"��Ī �� ���ð�: {postMatchWaitTime}��");

            // ��Ī �� ���ð� Ÿ�̸� ����
            UIManager.Instance.StartTimer(postMatchWaitTime);

            yield return new WaitForSeconds(postMatchWaitTime);

            //Debug.Log("���尡 ���۵˴ϴ�!");

            // ���� ����
            Manager.Battle.StartBattle(selfPlayer, currentOpponent, roundDuration);

            // ���� ���� �ð� Ÿ�̸� ����
            UIManager.Instance.StartTimer(roundDuration);
        }
    }

    public void OnRoundEnd(bool playerWon, int survivingEnemyUnits)
    {
        if (!playerWon)
        {
            ApplyDamage(survivingEnemyUnits);
        }

        // ���� ����
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

    void ApplyDamage(int survivingEnemyUnits)
    {
        int index = currentStage - 1;
        int totalDamage = baseDamages[index] + (damagePerEnemyUnit[index] * survivingEnemyUnits);

        // �÷��̾�� ������ ����
        int Hp = selfPlayer.GetComponent<Player>().CurrentHealth;
        Hp -= totalDamage;
        selfPlayer.GetComponent<Player>().setCurrentHealth = Hp;
        //Debug.Log($"�÷��̾ {totalDamage}�� ���ظ� �Ծ����ϴ�. ���� ü��: {selfPlayer.GetComponent<Player>().CurrentHealth}");

        // ü�¹� ������Ʈ
        Manager.UserHp.UpdateHealthBars();


        // ���� ���� üũ
        if (selfPlayer.GetComponent<Player>().CurrentHealth <= 0)
        {
           // Debug.Log("���� ����!");
            // ���� ���� ���� ó��
        }
    }

    void DisplayCurrentStageAndRound()
    {
       // Debug.Log($"���� ��������: {currentStage}, ���� ����: {currentRound}");
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
        // ���� ���� ����: 2-1, 3-2, 4-2
        if ((stage == 2 && round == 1) || (stage == 3 && round == 2) || (stage == 4 && round == 2))
        {
            return true;
        }
        return false;
    }

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
                if (playerComponent.PlayerType != PlayerType.Player1)
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
            int healthA = a.GetComponent<Player>().CurrentHealth;
            int healthB = b.GetComponent<Player>().CurrentHealth;

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
}