using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRightSidePanel : UIBase
{
    [SerializeField] private GameObject ChampionExplainPanel;
    [SerializeField] private Button button;

    protected override void Init()
    {
        base.Init();

        button.onClick.AddListener(ClickBtn);
    }

    private void ClickBtn()
    {
        if (ChampionExplainPanel.activeSelf)
        {
            ChampionExplainPanel.SetActive(false);
        }
    }
}
