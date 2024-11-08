using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISceneMain : UIBase
{
    [Header("시너지")]
    [SerializeField] private UISynergyPanel uiSynergyPanel;
    private SymbolDataBlueprint symbol;


    [Header("상점")]
    [SerializeField] private UIShopPanel uiShopPanel;

    [Header("스테이지")]
    [SerializeField] private GameObject[] player;
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private GameObject Crip;

    [Header("유저 체력")]
    [SerializeField] private GameObject healthBarContainer;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private List<HealthUI> hpBarList = new List<HealthUI>();


    public UISynergyPanel UISynergyPanel
    {
        get { return uiSynergyPanel; }
        set { uiSynergyPanel = value;}
    }

    public UIShopPanel UIShopPanel
    {
        get { return uiShopPanel; }
        set { uiShopPanel = value; }
    }

    protected override void Init()
    {
        base.Init();
        Manager.Stage.InitStage(player, mapGenerator, Crip);
        Manager.UserHp.InitUserHp(healthBarContainer, healthBarPrefab, hpBarList);
    }


    public void InitPanel(GameDataBlueprint gameData, SymbolDataBlueprint symbolData)
    {
        symbol = symbolData;

        uiSynergyPanel.InitSynergyBtn(symbolData);
        uiShopPanel.InitShopBtn(gameData);

    }
}
