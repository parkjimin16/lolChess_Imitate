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

        // ���� ����ġ ��ư
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

        // championPos ����Ʈ���� �� Ÿ���� ã���ϴ�.
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

        // ��尡 2 �̻����� Ȯ��
        if (user.UserGold >= 2)
        {
            // ��� 2 �Ҹ�
            user.UserGold -= 2;

            // UI ������Ʈ (��� ǥ��)
            //UIManager.Instance.UpdateUserGoldUI(user);

            // è�Ǿ� ���� ������Ʈ
            shopChampionList = Manager.Champion.GetRandomChampions(user.UserLevel);

            int idx = 0;

            foreach (var championName in shopChampionList)
            {
                // ���� �ε����� ������ �ʰ����� �ʵ��� Ȯ��
                if (idx >= championSlotList.Count)
                    break;

                ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint(championName) as ChampionBlueprint;
                ChampionSlot championSlotScript = championSlotList[idx].GetComponent<ChampionSlot>();
                Button btn = championSlotList[idx].GetComponent<Button>();

                // ���� �ʱ�ȭ
                btn.interactable = true;
                championSlotScript.ChampionSlotInit(championBlueprint, Utilities.SetSlotColor(championBlueprint.ChampionCost));
                idx++;
            }

            Debug.Log($"{user.UserName}���� 2 ��带 ����Ͽ� è�Ǿ� ������ ������Ʈ�߽��ϴ�.");
        }
        else
        {
            // ��� ���� �� �޽��� ���
            Debug.Log($"{user.UserName}�Կ��Դ� 2 ��尡 �����մϴ�.");
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
        /*Debug.Log("����ġ ����");
        Level++;
        if(Level >= 10)
        {
            Level = 10;
        }*/
        // ���� ������ �������� (��: ù ��° ����)
        UserData user = Manager.User.GetHumanUserData();

        // ��尡 4 �̻����� Ȯ��
        if (user.UserGold >= 4)
        {
            // ��� 4 �Ҹ�
            user.UserGold -= 4;

            // ����ġ 4 �߰�
            Manager.Level.AddExperience(user, 4);
            //LevelManager.Instance.AddExperience(user, 4);

            // UI ������Ʈ (��� �� ����ġ)
            //UIManager.Instance.UpdateUserGoldUI(user);
            //UIManager.Instance.UpdateUserExpUI(user);

            Debug.Log($"{user.UserName}���� 4 ��带 ����Ͽ� 4 EXP�� ������ϴ�.");
        }
        else
        {
            Debug.Log($"{user.UserName}�Կ��Դ� ����� ��尡 �����ϴ�.");
            // �ʿ� ��, UI�� �����ϴٴ� �޽����� ǥ���� �� �ֽ��ϴ�.
        }
    }
    
    public void SetChampionPos(RectTile tile)
    {
        rtile = tile;
        championPos = rtile.GetRectTileList();
    }
}
