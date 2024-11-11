using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BattleManager
{
    private Dictionary<GameObject, Coroutine> battleCoroutines = new Dictionary<GameObject, Coroutine>();

    public void StartBattle(GameObject player1, GameObject player2, int duration)
    {
        // 이미 전투 중인 플레이어가 있으면 건너뜁니다.
        if (battleCoroutines.ContainsKey(player1))
            return;

        Coroutine battleCoroutine = CoroutineHelper.StartCoroutine(BattleCoroutine(duration, player1, player2));
        battleCoroutines.Add(player1, battleCoroutine);
        battleCoroutines.Add(player2, battleCoroutine); // player2도 추가하여 추적
    }

    IEnumerator BattleCoroutine(int duration, GameObject player1, GameObject player2)
    {
        Debug.Log($"전투가 시작되었습니다. {player1.GetComponent<Player>().UserData.UserName} vs {player2.GetComponent<Player>().UserData.UserName}, 전투 시간: {duration}초");

        // 전투 진행 (전투 시간만큼 대기)
        yield return new WaitForSeconds(duration);

        // 예시로 랜덤하게 승패를 결정
        bool player1Won = Random.value > 0.5f;
        int survivingEnemyUnits = player1Won ? 0 : Random.Range(1, 5); // 패배 시 1~4개의 적 유닛이 생존

        //Debug.Log(player1Won ? $"{player1.name} 승리" : $"{player1.name} 패배");

        // 전투 종료 처리
        EndBattle(player1, player2, player1Won, survivingEnemyUnits);
    }

    void EndBattle(GameObject player1, GameObject player2, bool player1Won, int survivingEnemyUnits)
    {
        // 전투 코루틴 제거
        if (battleCoroutines.ContainsKey(player1))
        {
            battleCoroutines.Remove(player1);
        }
        if (battleCoroutines.ContainsKey(player2))
        {
            battleCoroutines.Remove(player2);
        }

        // StageManager에 전투 종료를 알림 (양쪽 플레이어와 승패 정보 전달)
        Debug.Log(player1Won ? $"{player1.GetComponent<Player>().UserData.UserName} 승리" : $"{player2.GetComponent<Player>().UserData.UserName} 승리");
        Manager.Stage.OnBattleEnd(player1, player2, player1Won, survivingEnemyUnits);
    }
}
