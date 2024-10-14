using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // UI ��� ����
    public TextMeshProUGUI stageRoundText;
    public TextMeshProUGUI timerText;

    // Ÿ�̸� �ڷ�ƾ�� �����ϱ� ���� ����
    private Coroutine timerCoroutine;

    // �̱��� ������ ����Ͽ� �ٸ� ��ũ��Ʈ���� ���� ������ �� �ֵ��� �մϴ�.
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // ���������� ���� UI ������Ʈ
    public void UpdateStageRoundUI(int stage, int round)
    {
        stageRoundText.text = $"Stage {stage} - {round}";
    }

    // Ÿ�̸� ����
    public void StartTimer(int duration)
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(TimerCoroutine(duration));
    }

    // Ÿ�̸� �ڷ�ƾ
    IEnumerator TimerCoroutine(int duration)
    {
        int remainingTime = duration;
        while (remainingTime > 0)
        {
            UpdateTimerUI(remainingTime);
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }
        UpdateTimerUI(0); // Ÿ�̸Ӱ� ������ �� 0���� ǥ��
    }

    // Ÿ�̸� UI ������Ʈ
    void UpdateTimerUI(int time)
    {
        timerText.text = $"{time}s";

        // ����: ���� �ð��� 5�� ������ �� �ؽ�Ʈ ���� ����
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
