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
        // 전투 로직 구현
        // 여기서는 간단히 전투 시간을 고려하여 승패를 결정합니다.

        StartCoroutine(BattleCoroutine(duration, selfPlayer, opponent));
    }

    IEnumerator BattleCoroutine(int duration, PlayerData selfPlayer, PlayerData opponent)
    {
        Debug.Log($"전투가 시작되었습니다. 전투 시간: {duration}초");

        // 전투 진행 (전투 시간만큼 대기)
        yield return new WaitForSeconds(duration);

        // 예시로 랜덤하게 승패를 결정
        playerWon = Random.value > 0.9f;
        survivingEnemyUnits = playerWon ? 0 : Random.Range(1, 5); // 패배 시 1~4개의 적 유닛이 생존

        Debug.Log(playerWon ? "플레이어가 승리했습니다." : "플레이어가 패배했습니다.");

        // 전투 종료 처리
        EndBattle();
    }

    void EndBattle()
    {
        stageManager.OnRoundEnd(playerWon, survivingEnemyUnits);
    }
}
