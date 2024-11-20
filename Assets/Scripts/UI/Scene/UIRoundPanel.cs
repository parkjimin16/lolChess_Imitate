using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIRoundPanel : UIBase
{
    #region 변수 및 프로퍼티

    [Header("이미지")]
    [SerializeField] private Sprite combat;     // 전투 라운드
    [SerializeField] private Sprite crip;       // 크립 라운드
    [SerializeField] private Sprite select;     // 공동 선택 라운드
    [SerializeField] private Sprite augmenter;  // 증강 라운드

    [Header("첫 라운드")]
    [SerializeField] private GameObject FirstStage;
    [SerializeField] private List<Image> image_FirstStage;
    [SerializeField] private TextMeshProUGUI txt_FirstStage;
    [SerializeField] private TextMeshProUGUI txt_FirstTimer;
    [SerializeField] private Slider slider_Timer_First;

    [Header("이후 라운드")]
    [SerializeField] private GameObject AfterSecondStage;
    [SerializeField] private List<Image> image_AfterStage;
    [SerializeField] private TextMeshProUGUI txt_AfterStage;
    [SerializeField] private TextMeshProUGUI txt_AfterTimer;
    [SerializeField] private Slider slider_Timer_After;

    private bool isFirst;
    private Coroutine timerCoroutine;
    private List<Sprite> sprites;
    #endregion

    #region 초기화

    public void InitRoundPanel()
    {
        sprites = new List<Sprite>();
    }

    public void UpdateStageRoundPanel(int stage, int round)
    {
        sprites.Clear();

        if (stage == 1)
        {
            sprites = new List<Sprite> { crip, crip, crip  };
            FirstStage.SetActive(true);
            AfterSecondStage.SetActive(false);
            txt_FirstStage.text = $"{stage} - {round}";

            for(int i=0;i < sprites.Count; i++)
            {
                image_FirstStage[i].sprite = sprites[i];
            }
        }
        else
        {
            if(stage == 2) 
            {
                sprites = new List<Sprite> { augmenter, combat, combat, select, combat, combat, crip };
            }
            else
            {
                sprites = new List<Sprite> { combat, augmenter, combat, select, combat, combat, crip };
            }

            FirstStage.SetActive(false);
            AfterSecondStage.SetActive(true);
            txt_AfterStage.text = $"{stage} - {round}";

            for (int i = 0; i < sprites.Count; i++)
            {
                image_AfterStage[i].sprite = sprites[i];
            }
        }
    }

    #endregion

    #region 타이머 로직

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
