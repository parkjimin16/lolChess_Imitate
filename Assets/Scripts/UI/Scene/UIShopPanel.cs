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
        Manager.Champion.InstantiateChampion(Manager.User.GetHumanUserData(), cBlueprint, hextile, tileTransform);
    }

    private void UpdateChampionSlot(PointerEventData enterEvent)
    {
        UserData user = Manager.User.GetHumanUserData();

        // 골드가 2 이상인지 확인
        if (user.UserGold >= 2)
        {
            // 골드 2 소모
            user.UserGold -= 2;

            // UI 업데이트 (골드 표시)
            //UIManager.Instance.UpdateUserGoldUI(user);

            // 챔피언 슬롯 업데이트
            shopChampionList = Manager.Champion.GetRandomChampions(user.UserLevel);

            int idx = 0;

            foreach (var championName in shopChampionList)
            {
                // 슬롯 인덱스가 범위를 초과하지 않도록 확인
                if (idx >= championSlotList.Count)
                    break;

                ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint(championName) as ChampionBlueprint;
                ChampionSlot championSlotScript = championSlotList[idx].GetComponent<ChampionSlot>();
                Button btn = championSlotList[idx].GetComponent<Button>();

                // 슬롯 초기화
                btn.interactable = true;
                championSlotScript.ChampionSlotInit(championBlueprint, Utilities.SetSlotColor(championBlueprint.ChampionCost));
                idx++;
            }

            Debug.Log($"{user.UserName}님이 2 골드를 사용하여 챔피언 슬롯을 업데이트했습니다.");
        }
        else
        {
            // 골드 부족 시 메시지 출력
            Debug.Log($"{user.UserName}님에게는 2 골드가 부족합니다.");
        }

        /*shopChampionList = Manager.Champion.GetRandomChampions(Level);

        int idx = 0;

        foreach (var championName in shopChampionList)
        {
            ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint(championName) as ChampionBlueprint;
            ChampionSlot championSlotSciprt = championSlotList[idx].GetComponent<ChampionSlot>();
            Button btn = championSlotList[idx].GetComponent<Button>();

            btn.interactable = true;
            championSlotSciprt.ChampionSlotInit(championBlueprint, Utilities.SetSlotColor(championBlueprint.ChampionCost));
            idx++;
        }*/
    }

    private void UpdateExpBtn(PointerEventData enterEvent)
    {
        /*Debug.Log("경험치 증가");
        Level++;
        if(Level >= 10)
        {
            Level = 10;
        }*/
        // 유저 데이터 가져오기 (예: 첫 번째 유저)
        UserData user = Manager.User.GetHumanUserData();

        // 골드가 4 이상인지 확인
        if (user.UserGold >= 4)
        {
            // 골드 4 소모
            user.UserGold -= 4;

            // 경험치 4 추가
            Manager.Level.AddExperience(user, 4);
            //LevelManager.Instance.AddExperience(user, 4);

            // UI 업데이트 (골드 및 경험치)
            //UIManager.Instance.UpdateUserGoldUI(user);
            //UIManager.Instance.UpdateUserExpUI(user);

            Debug.Log($"{user.UserName}님이 4 골드를 사용하여 4 EXP를 얻었습니다.");
        }
        else
        {
            Debug.Log($"{user.UserName}님에게는 충분한 골드가 없습니다.");
            // 필요 시, UI에 부족하다는 메시지를 표시할 수 있습니다.
        }
    }
    
    public void SetChampionPos(RectTile tile)
    {
        rtile = tile;
        championPos = rtile.GetRectTileList();
    }
}
