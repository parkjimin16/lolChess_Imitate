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
    [SerializeField] private PlayerData[] player;
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private GameObject go;

    [Header("유저 체력")]
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
