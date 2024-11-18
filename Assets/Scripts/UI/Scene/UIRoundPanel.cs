using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private GameObject FirstRound;
    [SerializeField] private List<Image> image_FirstRound;
    [SerializeField] private TextMeshProUGUI txt_FirstStage;
    [SerializeField] private TextMeshProUGUI txt_FirstTimer;
    [SerializeField] private Slider slier_Timer_First;

    [Header("이후 라운드")]
    [SerializeField] private GameObject AfterSecondRound;
    [SerializeField] private List<Image> image_AfterRound;
    [SerializeField] private TextMeshProUGUI txt_AfterStage;
    [SerializeField] private TextMeshProUGUI txt_AfterTimer;
    [SerializeField] private Slider slier_Timer_After;

    #endregion

    #region 초기화

    public void InitRound(int round, int subRound)
    {
        if(round == 1)
        {
            FirstRound.SetActive(true);
            AfterSecondRound.SetActive(false);
        }
        else
        {
            FirstRound.SetActive(false);
            AfterSecondRound.SetActive(true);
        }
    }

    #endregion
}
