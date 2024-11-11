using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BattleManager
{
    private Dictionary<GameObject, Coroutine> battleCoroutines = new Dictionary<GameObject, Coroutine>();

    public void StartBattle(GameObject player1, GameObject player2, int duration)
    {
        // �̹� ���� ���� �÷��̾ ������ �ǳʶݴϴ�.
        if (battleCoroutines.ContainsKey(player1))
            return;

        Coroutine battleCoroutine = CoroutineHelper.StartCoroutine(BattleCoroutine(duration, player1, player2));
        battleCoroutines.Add(player1, battleCoroutine);
        battleCoroutines.Add(player2, battleCoroutine); // player2�� �߰��Ͽ� ����
    }

    IEnumerator BattleCoroutine(int duration, GameObject player1, GameObject player2)
    {
        Debug.Log($"������ ���۵Ǿ����ϴ�. {player1.GetComponent<Player>().UserData.UserName} vs {player2.GetComponent<Player>().UserData.UserName}, ���� �ð�: {duration}��");

        // ���� ���� (���� �ð���ŭ ���)
        yield return new WaitForSeconds(duration);

        // ���÷� �����ϰ� ���и� ����
        bool player1Won = Random.value > 0.5f;
        int survivingEnemyUnits = player1Won ? 0 : Random.Range(1, 5); // �й� �� 1~4���� �� ������ ����

        //Debug.Log(player1Won ? $"{player1.name} �¸�" : $"{player1.name} �й�");

        // ���� ���� ó��
        EndBattle(player1, player2, player1Won, survivingEnemyUnits);
    }

    void EndBattle(GameObject player1, GameObject player2, bool player1Won, int survivingEnemyUnits)
    {
        // ���� �ڷ�ƾ ����
        if (battleCoroutines.ContainsKey(player1))
        {
            battleCoroutines.Remove(player1);
        }
        if (battleCoroutines.ContainsKey(player2))
        {
            battleCoroutines.Remove(player2);
        }

        // StageManager�� ���� ���Ḧ �˸� (���� �÷��̾�� ���� ���� ����)
        Debug.Log(player1Won ? $"{player1.GetComponent<Player>().UserData.UserName} �¸�" : $"{player2.GetComponent<Player>().UserData.UserName} �¸�");
        Manager.Stage.OnBattleEnd(player1, player2, player1Won, survivingEnemyUnits);
    }
}
