using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISceneMain : UIBase
{
    [SerializeField] private GameDataBlueprint gameDataBlueprint;
    [SerializeField] private SymbolDataBlueprint symbolDataBlueprint;

    [Header("시너지")]
    [SerializeField] private UISynergyPanel uiSynergyPanel;


    [Header("상점")]
    [SerializeField] private UIShopPanel uiShopPanel;

    [Header("스테이지")]
    [SerializeField] private GameObject[] player;
    [SerializeField] private MapGenerator mapGenerator;

    [Header("유저 체력")]
    [SerializeField] private GameObject healthBarContainer;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private List<HealthUI> hpBarList = new List<HealthUI>();

    [Header("챔피언 정보")]
    [SerializeField] private UIChampionExplainPanel uiChampionExplainPanel;

    public GameDataBlueprint GameDataBlueprint => gameDataBlueprint;
    public SymbolDataBlueprint SymbolDataBlueprint => symbolDataBlueprint;

    public UIChampionExplainPanel UIChampionExplainPanel
    {
        get { return uiChampionExplainPanel; }
        set { uiChampionExplainPanel = value; }
    }

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
        Manager.UserHp.InitUserHp(hpBarList);
    }


    public void InitPanel(GameDataBlueprint gameData, SymbolDataBlueprint symbolData)
    {
        symbolDataBlueprint = symbolData;
        gameDataBlueprint = gameData;

        uiSynergyPanel.InitSynergyBtn(symbolData);
        uiShopPanel.InitShopBtn(gameData);
        uiChampionExplainPanel.InitChampionExplainPanel(symbolData);

    }
}
