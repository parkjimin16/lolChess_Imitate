using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
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


    [SerializeField] private TextMeshProUGUI txt_User_Level;
    [SerializeField] private Slider slider_Xp;
    [SerializeField] private TextMeshProUGUI txt_User_Xp;

    [SerializeField] private List<TextMeshProUGUI> txt_Champion_Percent;
    [SerializeField] private TextMeshProUGUI txt_Gold;
    [SerializeField] private GameObject continous_Box;
    [SerializeField] private Image contious_Image;
    [SerializeField] private TextMeshProUGUI continous_Count;


    private int currentChampionIndex = 0;

    private bool isLoadComplete = false;
    public int Level = 1;

    [SerializeField] private RectTile rtile;

    #region Init

    public void InitShopBtn(GameDataBlueprint gameData)
    {
        gameDataBlueprint = gameData;

        SetUI<Button>();

        // ���� ����ġ ��ư
        SetButtonEvent("Btn_Exp", UIEventType.Click, UpdateExpBtn);
        SetButtonEvent("Btn_Reroll", UIEventType.Click, UpdateChampionSlot);

        int i = 0;
        foreach (GameObject slot in championSlotList)
        {
            Button button = slot.GetComponent<Button>();
            ChampionSlot cSlot = slot.GetComponent<ChampionSlot>();
            button.interactable = true;
            slot.SetActive(true);

            if (button != null)
            {
                button.onClick.AddListener(() => InstantiateChampion(cSlot.ChampionBlueprint, slot));
            }
            i++;
        }

        UpdatePlayerXP();
        UpdateChampionPercent();
    }

    #endregion

    #region è�Ǿ�
    private void InstantiateChampion(ChampionBlueprint cBlueprint, GameObject obj)
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


        HideSlot(obj);
        Manager.Champion.InstantiateChampion(Manager.User.GetHumanUserData(), cBlueprint, hextile, tileTransform);
    }

    public void UpdateChampionSlot(PointerEventData enterEvent)
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
                ShowSlot(championSlotList[idx]);
                championSlotScript.ChampionSlotInit(uiMain.SymbolDataBlueprint, championBlueprint, Utilities.SetSlotColor(championBlueprint.ChampionCost));
                idx++;
            }

            Debug.Log($"{user.UserName}���� 2 ��带 ����Ͽ� è�Ǿ� ������ ������Ʈ�߽��ϴ�.");
        }
        else
        {
            Debug.Log($"{user.UserName}�Կ��Դ� 2 ��尡 �����մϴ�.");
        }

      
    }
    public void SetChampionPos(RectTile tile)
    {
        rtile = tile;
        championPos = rtile.GetRectTileList();
    }

    #endregion

    #region ����ġ & ��� & ���� ����
    private void UpdateExpBtn(PointerEventData enterEvent)
    {
        UserData user = Manager.User.GetHumanUserData();

        if (user.UserGold >= 4)
        {
            user.UserGold -= 4;
            Manager.Level.AddExperience(user, 4);
            UpdatePlayerXP();
            UpdateChampionPercent();
            Debug.Log($"{user.UserName}���� 4 ��带 ����Ͽ� 4 EXP�� ������ϴ�.");
        }
        else
        {
            Debug.Log($"{user.UserName}�Կ��Դ� ����� ��尡 �����ϴ�.");
        }
    }


    private void UpdatePlayerXP()
    {
        int level = Manager.User.GetHumanUserData().UserLevel;
        txt_User_Level.text = level.ToString() + "����";


        int curXp = Manager.User.GetHumanUserData().UserExp;
        int maxXp = Manager.Level.ExperienceTable[level+1];
        txt_User_Xp.text = $"{curXp} / {maxXp}";

        SetSliderRange(curXp, maxXp);
    }

    #endregion

    #region è�Ǿ� Ȯ��

    private void UpdateChampionPercent()
    {
        int level = Manager.User.GetHumanUserData().UserLevel;
       

        for(int i =0;i < uiMain.GameDataBlueprint.ChampionRandomDataList[level].Probability.Length; i++)
        {
            txt_Champion_Percent[i].text = (uiMain.GameDataBlueprint.ChampionRandomDataList[level].Probability[i] * 100).ToString() + "%";
        }
    }

    #endregion
    #region ����
    public void HideSlot(GameObject slot)
    {
        CanvasGroup canvasGroup = slot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = slot.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowSlot(GameObject slot)
    {
        CanvasGroup canvasGroup = slot.GetComponent<CanvasGroup>();
        if (canvasGroup == null) return;

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void SetSliderRange(float cur, float max)
    {
        if (slider_Xp != null)
        {
            slider_Xp.value = cur;
            slider_Xp.maxValue = max;
        }
        else
        {
            Debug.LogWarning("����ġ �����̴� ����");
        }
    }
    #endregion
}
