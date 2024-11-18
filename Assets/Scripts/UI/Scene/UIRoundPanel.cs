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
    [SerializeField] private GameObject FirstRound;
    [SerializeField] private List<Image> image_FirstRound;
    [SerializeField] private TextMeshProUGUI txt_FirstStage;
    [SerializeField] private TextMeshProUGUI txt_FirstTimer;
    [SerializeField] private Slider slier_Timer_First;

    [Header("���� ����")]
    [SerializeField] private GameObject AfterSecondRound;
    [SerializeField] private List<Image> image_AfterRound;
    [SerializeField] private TextMeshProUGUI txt_AfterStage;
    [SerializeField] private TextMeshProUGUI txt_AfterTimer;
    [SerializeField] private Slider slier_Timer_After;

    #endregion

    #region �ʱ�ȭ

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
