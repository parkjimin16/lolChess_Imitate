using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // UI 요소 참조
    public TextMeshProUGUI stageRoundText;
    public TextMeshProUGUI timerText;

    // 타이머 코루틴을 제어하기 위한 변수
    private Coroutine timerCoroutine;

    // 싱글톤 패턴을 사용하여 다른 스크립트에서 쉽게 접근할 수 있도록 합니다.
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // 스테이지와 라운드 UI 업데이트
    public void UpdateStageRoundUI(int stage, int round)
    {
        stageRoundText.text = $"Stage {stage} - {round}";
    }

    // 타이머 시작
    public void StartTimer(int duration)
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(TimerCoroutine(duration));
    }

    // 타이머 코루틴
    IEnumerator TimerCoroutine(int duration)
    {
        int remainingTime = duration;
        while (remainingTime > 0)
        {
            UpdateTimerUI(remainingTime);
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }
        UpdateTimerUI(0); // 타이머가 끝났을 때 0으로 표시
    }

    // 타이머 UI 업데이트
    void UpdateTimerUI(int time)
    {
        timerText.text = $"{time}s";

        // 예시: 남은 시간이 5초 이하일 때 텍스트 색상 변경
        if (time <= 5)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }
    }
}
