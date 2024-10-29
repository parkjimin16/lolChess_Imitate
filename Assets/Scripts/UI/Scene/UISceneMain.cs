using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISceneMain : UIBase
{
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


    public void InitPanel(GameDataBlueprint gameData)
    {
        uiShopPanel.InitShopBtn(gameData);
    }
}
