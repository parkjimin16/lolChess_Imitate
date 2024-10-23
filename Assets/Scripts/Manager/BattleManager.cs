using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public StageManager stageManager;
    private bool playerWon;
    private int survivingEnemyUnits;

    public void StartBattle(PlayerData selfPlayer, PlayerData opponent, int duration)
    {
        // ���� ���� ����
        // ���⼭�� ������ ���� �ð��� ����Ͽ� ���и� �����մϴ�.

        StartCoroutine(BattleCoroutine(duration, selfPlayer, opponent));
    }

    IEnumerator BattleCoroutine(int duration, PlayerData selfPlayer, PlayerData opponent)
    {
        Debug.Log($"������ ���۵Ǿ����ϴ�. ���� �ð�: {duration}��");

        // ���� ���� (���� �ð���ŭ ���)
        yield return new WaitForSeconds(duration);

        // ���÷� �����ϰ� ���и� ����
        playerWon = Random.value > 0.9f;
        survivingEnemyUnits = playerWon ? 0 : Random.Range(1, 5); // �й� �� 1~4���� �� ������ ����

        Debug.Log(playerWon ? "�÷��̾ �¸��߽��ϴ�." : "�÷��̾ �й��߽��ϴ�.");

        // ���� ���� ó��
        EndBattle();
    }

    void EndBattle()
    {
        stageManager.OnRoundEnd(playerWon, survivingEnemyUnits);
    }
}
