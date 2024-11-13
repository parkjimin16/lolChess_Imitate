using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopPanel : UIBase
{
    [SerializeField] private UISceneMain uiMain;

    [SerializeField] private List<GameObject> championSlotList;
    [SerializeField] private GameDataBlueprint gameDataBlueprint;
    [SerializeField] private List<ItemBlueprint> itemBlueprint;
    [SerializeField] private List<string> shopChampionList;
    [SerializeField] private GameObject targetObject;

    [SerializeField] private List<Transform> championPos;
    private int currentChampionIndex = 0;

    private bool isLoadComplete = false;
    public int Level = 1;

    [SerializeField] private RectTile rtile;

    public void InitShopBtn(GameDataBlueprint gameData)
    {
        gameDataBlueprint = gameData;

        SetUI<Button>();

        // 유저 경험치 버튼
        SetButtonEvent("Btn_Exp", UIEventType.Click, UpdateExpBtn);
        SetButtonEvent("Btn_Reroll", UIEventType.Click, UpdateChampionSlot);

        foreach (GameObject slot in championSlotList)
        {
            Button button = slot.GetComponent<Button>();
            ChampionSlot cSlot = slot.GetComponent<ChampionSlot>();
            button.interactable = true;

            if (button != null)
            {
                button.onClick.AddListener(() => InstantiateChampion(cSlot.ChampionBlueprint, button));
            }
        }
        
    }

    private void InstantiateChampion(ChampionBlueprint cBlueprint, Button button)
    {
        HexTile hextile = null;
        Transform tileTransform = null;

        // championPos 리스트에서 빈 타일을 찾습니다.
        foreach (Transform pos in championPos)
        {
            HexTile tile = pos.GetComponent<HexTile>();
            if (!tile.isOccupied)
            {
                hextile = tile;
                tileTransform = pos;
                break;
            }
        }

        if (hextile == null)
            return;

        button.interactable = false;
        Manager.Champion.InstantiateChampion(Manager.User.User1_Data, cBlueprint, hextile, tileTransform);
    }

    private void UpdateChampionSlot(PointerEventData enterEvent)
    {
        shopChampionList = Manager.Champion.GetRandomChampions(Level);

        int idx = 0;

        foreach (var championName in shopChampionList)
        {
            ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint(championName) as ChampionBlueprint;
            ChampionSlot championSlotSciprt = championSlotList[idx].GetComponent<ChampionSlot>();
            Button btn = championSlotList[idx].GetComponent<Button>();

            btn.interactable = true;
            championSlotSciprt.ChampionSlotInit(championBlueprint, Utilities.SetSlotColor(championBlueprint.ChampionCost));
            idx++;
        }
    }

    private void UpdateExpBtn(PointerEventData enterEvent)
    {
        Debug.Log("경험치 증가");
        Level++;
        if(Level >= 10)
        {
            Level = 10;
        }
    }
    
    public void SetChampionPos(RectTile tile)
    {
        rtile = tile;
        championPos = rtile.GetRectTileList();
    }
}
