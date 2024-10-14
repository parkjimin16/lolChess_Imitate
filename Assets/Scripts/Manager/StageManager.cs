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

    // 라운드 대기시간 설정
    private int normalWaitTime = 30;
    private int augmentWaitTime = 50;
    private int postMatchWaitTime = 3;
    private int roundDuration = 30;

    private bool isAugmentRound = false;

    private Coroutine roundCoroutine;

    void Start()
    {
        InitializePlayers();
        StartStage(currentStage);
    }

    void InitializePlayers()
    {
        // 자기 자신과 상대 플레이어 분리
        // 여기서는 첫 번째 플레이어를 자기 자신으로 가정
        selfPlayer = allPlayers[0];

        opponents = new List<PlayerData>(allPlayers);
        opponents.Remove(selfPlayer);
    }

    void StartStage(int stageNumber)
    {
        currentRound = 1;
        ShuffleOpponents(); // 상대 리스트를 섞음

        if (roundCoroutine != null)
            StopCoroutine(roundCoroutine);
        roundCoroutine = StartCoroutine(StartRoundCoroutine());
    }

    IEnumerator StartRoundCoroutine()
    {
        // UI 업데이트
        UIManager.Instance.UpdateStageRoundUI(currentStage, currentRound);

        // 증강 선택 라운드 여부 확인
        isAugmentRound = IsAugmentRound(currentStage, currentRound);

        // 대기시간 설정
        int waitTime = isAugmentRound ? augmentWaitTime : normalWaitTime;

        Debug.Log($"라운드 시작 전 대기시간: {waitTime}초");

        // 대기시간 타이머 시작
        UIManager.Instance.StartTimer(waitTime);

        yield return new WaitForSeconds(waitTime);

        // 상대 매칭
        int opponentIndex = (currentRound - 1) % opponents.Count;
        currentOpponent = opponents[opponentIndex];

        Debug.Log($"{currentOpponent.playerName}와 매칭되었습니다.");

        // 매칭 후 대기시간
        Debug.Log($"매칭 후 대기시간: {postMatchWaitTime}초");

        // 매칭 후 대기시간 타이머 시작
        UIManager.Instance.StartTimer(postMatchWaitTime);

        yield return new WaitForSeconds(postMatchWaitTime);

        Debug.Log("라운드가 시작됩니다!");

        // 전투 시작
        BattleManager battleManager = FindObjectOfType<BattleManager>();
        battleManager.StartBattle(selfPlayer, currentOpponent, roundDuration);

        // 라운드 진행 시간 타이머 시작
        UIManager.Instance.StartTimer(roundDuration);
    }

    public void OnRoundEnd(bool playerWon, int survivingEnemyUnits)
    {
        if (!playerWon)
        {
            ApplyDamage(survivingEnemyUnits);
        }

        // 라운드 증가
        currentRound++;

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
        }
        else
        {
            // 다음 라운드 시작
            if (roundCoroutine != null)
                StopCoroutine(roundCoroutine);
            roundCoroutine = StartCoroutine(StartRoundCoroutine());
        }
    }

    void ApplyDamage(int survivingEnemyUnits)
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
        // 증강 선택 라운드: 2-1, 3-2, 4-2
        if ((stage == 2 && round == 1) || (stage == 3 && round == 2) || (stage == 4 && round == 2))
        {
            return true;
        }
        return false;
    }
}
