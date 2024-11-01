using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISceneMain : UIBase
{
    [Header("�ó���")]
    [SerializeField] private UISynergyPanel uiSynergyPanel;
    private SymbolDataBlueprint symbol;


    [Header("����")]
    [SerializeField] private UIShopPanel uiShopPanel;

    [Header("��������")]
    [SerializeField] private PlayerData[] player;
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private GameObject go;

    [Header("���� ü��")]
    [SerializeField] private GameObject healthBarContainer;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private List<HealthUI> hpBarList = new List<HealthUI>();

    protected override void Init()
    {
        base.Init();
        Manager.Stage.InitStage(player, mapGenerator, go);
        Manager.UserHp.InitUserHp(healthBarContainer, healthBarPrefab, hpBarList);
    }


    public void InitPanel(GameDataBlueprint gameData, SymbolDataBlueprint symbolData)
    {
        symbol = symbolData;

        uiShopPanel.InitShopBtn(gameData);
        uiSynergyPanel.InitSynergyBtn(symbolData);
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.U)) 
        {
            Debug.Log("Press U");
            uiSynergyPanel.UpdateSynergy();
            //Manager.User.User1_Data.PrintSortedChampionSynergiesWithCount();
        }
    }

}
