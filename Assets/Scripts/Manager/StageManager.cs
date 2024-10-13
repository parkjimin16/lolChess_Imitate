using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public PlayerData[] allPlayers; // 총 8명의 플레이어 (자기 자신 포함)
    private List<PlayerData> opponents; // 상대 플레이어 목록 (자기 자신 제외)
    private PlayerData selfPlayer; // 자기 자신
    private PlayerData currentOpponent; // 현재 상대

    public int currentStage = 1;
    public int currentRound = 1;

    // 스테이지별 기본 피해량과 생존한 적 유닛당 피해량
    public int[] baseDamages; // 인덱스는 스테이지 번호 - 1
    public int[] damagePerEnemyUnit; // 인덱스는 스테이지 번호 - 1

    void Start()
    {
        InitializePlayers();
        StartStage(currentStage);
    }

    void InitializePlayers() //자기 자신과 플레이어 분리
    {
        // 자기 자신과 상대 플레이어 분리
        // 여기서는 첫 번째 플레이어를 자기 자신으로 가정
        selfPlayer = allPlayers[0];

        opponents = new List<PlayerData>(allPlayers);
        opponents.Remove(selfPlayer);
    }

    void StartStage(int stageNumber) //현재 스테이지를 시작하고 상대 리스트를 섞습니다.
    {
        currentRound = 1;
        ShuffleOpponents(); // 상대 리스트를 섞음
        DisplayCurrentStageAndRound();
        StartRound();
    }

    void StartRound() // 현재 라운드를 시작하고 상대를 선택하여 전투를 시작합니다.
    {
        int maxRounds = currentStage == 1 ? 4 : 7;

        if (currentRound > maxRounds)
        {
            // 다음 스테이지로 이동
            currentStage++;
            if (currentStage > 8)
            {
                // 게임 종료
                Debug.Log("게임 클리어!");
                return;
            }
            StartStage(currentStage);
            return;
        }

        // 순서대로 상대 선택
        int opponentIndex = (currentRound - 1) % opponents.Count;
        currentOpponent = opponents[opponentIndex];

        Debug.Log($"스테이지 {currentStage}, 라운드 {currentRound}: {currentOpponent.playerName}와 전투를 시작합니다.");

        /*// 전투 로직 시작
        BattleManager battleManager = FindObjectOfType<BattleManager>();
        battleManager.StartBattle(selfPlayer, currentOpponent);*/
    }

    public void OnRoundEnd(bool playerWon, int survivingEnemyUnits) //전투 종료 후 결과를 처리하고 다음 라운드를 시작합니다.
    {
        if (!playerWon)
        {
            ApplyDamage(survivingEnemyUnits);
        }

        // 라운드 증가
        currentRound++;

        // 다음 라운드 시작
        StartRound();
    }

    void ApplyDamage(int survivingEnemyUnits) //패배 시 데미지를 계산하여 플레이어의 체력에 반영합니다.
    {
        int index = currentStage - 1;
        int totalDamage = baseDamages[index] + (damagePerEnemyUnit[index] * survivingEnemyUnits);

        // 플레이어에게 데미지 적용
        selfPlayer.health -= totalDamage;
        Debug.Log($"플레이어가 {totalDamage}의 피해를 입었습니다. 남은 체력: {selfPlayer.health}");

        // 체력 체크
        if (selfPlayer.health <= 0)
        {
            Debug.Log("게임 오버!");
            // 게임 오버 로직 처리
        }
    }

    void DisplayCurrentStageAndRound()
    {
        Debug.Log($"현재 스테이지: {currentStage}, 현재 라운드: {currentRound}");
    }

    void ShuffleOpponents() //상대 리스트를 랜덤으로 섞어 동일한 상대와 연속으로 매칭되지 않도록 합니다.
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
