using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRoundPanel : UIBase
{
    #region ���� �� ������Ƽ

    [Header("�̹���")]
    [SerializeField] private Sprite combat;     // ���� ����
    [SerializeField] private Sprite crip;       // ũ�� ����
    [SerializeField] private Sprite select;     // ���� ���� ����
    [SerializeField] private Sprite augmenter;  // ���� ����

    [Header("ù ����")]
    [SerializeField] private GameObject FirstStage;
    [SerializeField] private List<Image> image_FirstStage;
    [SerializeField] private TextMeshProUGUI txt_FirstStage;
    [SerializeField] private TextMeshProUGUI txt_FirstTimer;
    [SerializeField] private Slider slider_Timer_First;

    [Header("���� ����")]
    [SerializeField] private GameObject AfterSecondStage;
    [SerializeField] private List<Image> image_AfterStage;
    [SerializeField] private TextMeshProUGUI txt_AfterStage;
    [SerializeField] private TextMeshProUGUI txt_AfterTimer;
    [SerializeField] private Slider slider_Timer_After;

    private bool isFirst;
    private Coroutine timerCoroutine;
    #endregion

    #region �ʱ�ȭ

    public void UpdateStageRoundPanel(int stage, int round)
    {
        if(stage == 1)
        {
            FirstStage.SetActive(true);
            AfterSecondStage.SetActive(false);
            txt_FirstStage.text = $"{stage} - {round}";
        }
        else
        {
            FirstStage.SetActive(false);
            AfterSecondStage.SetActive(true);
            txt_AfterStage.text = $"{stage} - {round}";
        }


    }

    #endregion

    #region Ÿ�̸� ����

    public void StartTimer(int stage, int duration)
    {
        TextMeshProUGUI txt_Timer;
        Slider slider_Timer;

        if(stage == 1)
        {
            txt_Timer = txt_FirstTimer;
            slider_Timer = slider_Timer_First;
        }
        else
        {
            txt_Timer = txt_AfterTimer;
            slider_Timer = slider_Timer_After;
        }

        slider_Timer.maxValue = duration;
        slider_Timer.value = duration;

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(TimerCoroutine(txt_Timer, slider_Timer, duration));
    }

    IEnumerator TimerCoroutine(TextMeshProUGUI txt_Timer, Slider slider_Timer, int duration)
    {
        float remainingTime = duration;

        while (remainingTime > 0)
        {
            float targetTime = Mathf.Floor(remainingTime);
            UpdateTimerUI(txt_Timer, targetTime);

            float startTime = remainingTime;
            float endTime = remainingTime - 1;

            float elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime;
                remainingTime = Mathf.Lerp(startTime, endTime, elapsedTime); 
                slider_Timer.value = remainingTime;
                yield return null; 
            }

            remainingTime = endTime;
        }

        UpdateTimerUI(txt_Timer, 0);
        slider_Timer.value = 0;
    }

    private void UpdateTimerUI(TextMeshProUGUI txt_Timer, float time)
    {
        txt_Timer.text = $"{Mathf.CeilToInt(time)}s";

        if (time <= 5)
        {
            txt_Timer.color = Color.red;
        }
        else
        {
            txt_Timer.color = Color.black;
        }
    }
    #endregion
}
