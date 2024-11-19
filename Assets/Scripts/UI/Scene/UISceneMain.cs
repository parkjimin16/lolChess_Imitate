using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISceneMain : UIBase
{
    [SerializeField] private GameDataBlueprint gameDataBlueprint;
    [SerializeField] private SymbolDataBlueprint symbolDataBlueprint;

    [Header("�ó���")]
    [SerializeField] private UISynergyPanel uiSynergyPanel;

    [Header("�÷��̾� ü��")]
    [SerializeField] private UIHeatlhBarPanel uiHealthBarPanel;

    [Header("����")]
    [SerializeField] private UIShopPanel uiShopPanel;

    [Header("��������")]
    [SerializeField] private GameObject[] player;
    [SerializeField] private MapGenerator mapGenerator;

    [Header("è�Ǿ� ����")]
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
    }


    public void InitPanel(GameDataBlueprint gameData, SymbolDataBlueprint symbolData)
    {
        symbolDataBlueprint = symbolData;
        gameDataBlueprint = gameData;

        uiSynergyPanel.InitSynergyBtn(symbolData);
        uiShopPanel.InitShopBtn(gameData);
        uiChampionExplainPanel.InitChampionExplainPanel(symbolData);
        uiHealthBarPanel.InitHpBarPanel();
    }
}
