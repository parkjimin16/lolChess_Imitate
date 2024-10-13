using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public PlayerData[] allPlayers; // �� 8���� �÷��̾� (�ڱ� �ڽ� ����)
    private List<PlayerData> opponents; // ��� �÷��̾� ��� (�ڱ� �ڽ� ����)
    private PlayerData selfPlayer; // �ڱ� �ڽ�
    private PlayerData currentOpponent; // ���� ���

    public int currentStage = 1;
    public int currentRound = 1;

    // ���������� �⺻ ���ط��� ������ �� ���ִ� ���ط�
    public int[] baseDamages; // �ε����� �������� ��ȣ - 1
    public int[] damagePerEnemyUnit; // �ε����� �������� ��ȣ - 1

    void Start()
    {
        InitializePlayers();
        StartStage(currentStage);
    }

    void InitializePlayers() //�ڱ� �ڽŰ� �÷��̾� �и�
    {
        // �ڱ� �ڽŰ� ��� �÷��̾� �и�
        // ���⼭�� ù ��° �÷��̾ �ڱ� �ڽ����� ����
        selfPlayer = allPlayers[0];

        opponents = new List<PlayerData>(allPlayers);
        opponents.Remove(selfPlayer);
    }

    void StartStage(int stageNumber) //���� ���������� �����ϰ� ��� ����Ʈ�� �����ϴ�.
    {
        currentRound = 1;
        ShuffleOpponents(); // ��� ����Ʈ�� ����
        DisplayCurrentStageAndRound();
        StartRound();
    }

    void StartRound() // ���� ���带 �����ϰ� ��븦 �����Ͽ� ������ �����մϴ�.
    {
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
            return;
        }

        // ������� ��� ����
        int opponentIndex = (currentRound - 1) % opponents.Count;
        currentOpponent = opponents[opponentIndex];

        Debug.Log($"�������� {currentStage}, ���� {currentRound}: {currentOpponent.playerName}�� ������ �����մϴ�.");

        /*// ���� ���� ����
        BattleManager battleManager = FindObjectOfType<BattleManager>();
        battleManager.StartBattle(selfPlayer, currentOpponent);*/
    }

    public void OnRoundEnd(bool playerWon, int survivingEnemyUnits) //���� ���� �� ����� ó���ϰ� ���� ���带 �����մϴ�.
    {
        if (!playerWon)
        {
            ApplyDamage(survivingEnemyUnits);
        }

        // ���� ����
        currentRound++;

        // ���� ���� ����
        StartRound();
    }

    void ApplyDamage(int survivingEnemyUnits) //�й� �� �������� ����Ͽ� �÷��̾��� ü�¿� �ݿ��մϴ�.
    {
        int index = currentStage - 1;
        int totalDamage = baseDamages[index] + (damagePerEnemyUnit[index] * survivingEnemyUnits);

        // �÷��̾�� ������ ����
        selfPlayer.health -= totalDamage;
        Debug.Log($"�÷��̾ {totalDamage}�� ���ظ� �Ծ����ϴ�. ���� ü��: {selfPlayer.health}");

        // ü�� üũ
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

    void ShuffleOpponents() //��� ����Ʈ�� �������� ���� ������ ���� �������� ��Ī���� �ʵ��� �մϴ�.
    {
        for (int i = 0; i < opponents.Count; i++)
        {
            PlayerData temp = opponents[i];
            int randomIndex = Random.Range(i, opponents.Count);
            opponents[i] = opponents[randomIndex];
            opponents[randomIndex] = temp;
        }
    }
}
