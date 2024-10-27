using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StageManager
{
    public PlayerData[] AllPlayers; // �� 8���� �÷��̾� (�ڱ� �ڽ� ����)
    private List<PlayerData> opponents; // ��� �÷��̾� ��� (�ڱ� �ڽ� ����)
    private PlayerData selfPlayer; // �ڱ� �ڽ�
    private PlayerData currentOpponent; // ���� ���

    public int currentStage = 1;
    public int currentRound = 1;

    // ���������� �⺻ ���ط��� ������ �� ���ִ� ���ط�
    public int[] baseDamages = new int[] { 0, 2, 5, 8, 10, 12, 17 }; // �ε����� �������� ��ȣ - 1
    public int[] damagePerEnemyUnit = new int[] { 1, 2, 3, 4, 5, 6, 7 }; // �ε����� �������� ��ȣ - 1

    // ���� ���ð� ����
    private int normalWaitTime = 3;
    private int augmentWaitTime = 50;
    private int postMatchWaitTime = 3;
    private int roundDuration = 3;

    private bool isAugmentRound = false;

    private Coroutine roundCoroutine;


    #region Init

    public void InitStage(PlayerData[] playerData)
    {
        AllPlayers = playerData;
        InitializePlayers();
        StartStage(currentStage);

    }
    #endregion

    private void InitializePlayers()
    {
        
        // �ڱ� �ڽŰ� ��� �÷��̾� �и�
        // ���⼭�� ù ��° �÷��̾ �ڱ� �ڽ����� ����
        
        selfPlayer = AllPlayers[0];
        
        opponents = new List<PlayerData>(AllPlayers);
        opponents.Remove(selfPlayer);
        for (int i = 0; i < AllPlayers.Length; i++)
        {
            AllPlayers[i].health = 100;
        }
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

        // ���ð� ����
        int waitTime = isAugmentRound ? augmentWaitTime : normalWaitTime;

        //Debug.Log($"���� ���� �� ���ð�: {waitTime}��");

        // ���ð� Ÿ�̸� ����
        UIManager.Instance.StartTimer(waitTime);

        yield return new WaitForSeconds(waitTime);

        // ��� ��Ī
        int opponentIndex = (currentRound - 1) % opponents.Count;
        currentOpponent = opponents[opponentIndex];

        //Debug.Log($"{currentOpponent.playerName}�� ��Ī�Ǿ����ϴ�.");

        // ��Ī �� ���ð�
        //Debug.Log($"��Ī �� ���ð�: {postMatchWaitTime}��");

        // ��Ī �� ���ð� Ÿ�̸� ����
        UIManager.Instance.StartTimer(postMatchWaitTime);

        yield return new WaitForSeconds(postMatchWaitTime);

        //Debug.Log("���尡 ���۵˴ϴ�!");

        // ���� ����
        Manager.Battle.StartBattle(selfPlayer, currentOpponent, roundDuration);

        // ���� ���� �ð� Ÿ�̸� ����
        UIManager.Instance.StartTimer(roundDuration);
    }

    public void OnRoundEnd(bool playerWon, int survivingEnemyUnits)
    {
        if (!playerWon)
        {
            ApplyDamage(survivingEnemyUnits);
        }

        // ���� ����
        currentRound++;

        int maxRounds = currentStage == 1 ? 4 : 7;

        if (currentRound > maxRounds)
        {
            // ���� ���������� �̵�
            currentStage++;
            if (currentStage > 8)
            {
                // ���� ����
                Debug.Log("���� Ŭ����!");
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
        selfPlayer.health -= totalDamage;
        //Debug.Log($"�÷��̾ {totalDamage}�� ���ظ� �Ծ����ϴ�. ���� ü��: {selfPlayer.health}");

        // ü�¹� ������Ʈ
        Manager.UserHp.UpdateHealthBars();


        // ���� ���� üũ
        if (selfPlayer.health <= 0)
        {
            Debug.Log("���� ����!");
            // ���� ���� ���� ó��
        }
    }

    void DisplayCurrentStageAndRound()
    {
        Debug.Log($"���� ��������: {currentStage}, ���� ����: {currentRound}");
    }

    void ShuffleOpponents()
    {
        for (int i = 0; i < opponents.Count; i++)
        {
            PlayerData temp = opponents[i];
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
}
