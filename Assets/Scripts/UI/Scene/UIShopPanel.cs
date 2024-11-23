using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopPanel : UIBase
{
    [SerializeField] private UISceneMain uiMain;

    [SerializeField] private GameDataBlueprint gameDataBlueprint;
    [SerializeField] private List<ItemBlueprint> itemBlueprint;

    [Header("����")]
    [SerializeField] private List<GameObject> championSlotList;
    [SerializeField] private List<string> shopChampionList;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private List<Transform> championPos;
    [SerializeField] private RectTile rtile;
    [SerializeField] private Button btn_Reroll_Lock;
    [SerializeField] private GameObject image_Lock;
    [SerializeField] private GameObject image_UnLock;

    [Header("����")]
    [SerializeField] private TextMeshProUGUI txt_User_Level;
    [SerializeField] private Slider slider_Xp;
    [SerializeField] private TextMeshProUGUI txt_User_Xp;


    [Header("����")]
    [SerializeField] private List<TextMeshProUGUI> txt_Champion_Percent;
    [SerializeField] private TextMeshProUGUI txt_Gold;
    [SerializeField] private GameObject continous_Box;
    [SerializeField] private Image contious_Image;
    [SerializeField] private TextMeshProUGUI continous_Count;

    [Header("�Ǹ�")]
    public GameObject SellPanel;
    [SerializeField] private TextMeshProUGUI txt_SellAmount;

    // ����
    [SerializeField] private bool reRoll_Lock;
   

    #region Init

    public void InitShopBtn(GameDataBlueprint gameData)
    {
        gameDataBlueprint = gameData;

        SetUI<Button>();

        // ���� ����ġ ��ư
        SetButtonEvent("Btn_Exp", UIEventType.Click, UpdateExpBtn);
        SetButtonEvent("Btn_Reroll", UIEventType.Click, UpdateChampionSlot);
        SetButtonEvent("Btn_Reroll_Lock", UIEventType .Click, Btn_ReRoll_Lock);

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

        InitChampionPos();

        UpdateChampionPercent(Manager.User.GetHumanUserData());

        continous_Box.SetActive(false);

        reRoll_Lock = true;
        Btn_ReRoll_Lock(null);
    }

    #endregion

    #region è�Ǿ� ����
    private void InstantiateChampion(ChampionBlueprint cBlueprint, GameObject obj)
    {
        HexTile hextile = null;
        Transform tileTransform = null;

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
        if (reRoll_Lock)
            return;

        UserData user = Manager.User.GetHumanUserData();

        if (user.UserGold >= 2)
        {
            Manager.Augmenter.ApplyRerollAugmenter(user);

            user.UserGold -= 2;
            shopChampionList = Manager.Champion.GetRandomChampions(user.UserLevel);
            
            
            
            // �׽�Ʈ �� �����ߵ�
            shopChampionList.Clear();
            shopChampionList.Add("ChampionBlueprint_Ashe");
            shopChampionList.Add("ChampionBlueprint_Ashe");
            shopChampionList.Add("ChampionBlueprint_Ashe");
            shopChampionList.Add("ChampionBlueprint_Ashe");
            shopChampionList.Add("ChampionBlueprint_Ashe");




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

            //Debug.Log($"{user.UserName}���� 2 ��带 ����Ͽ� è�Ǿ� ������ ������Ʈ�߽��ϴ�.");
        }
        else
        {
            //Debug.Log($"{user.UserName}�Կ��Դ� 2 ��尡 �����մϴ�.");
        }

      
    }
    public void InitChampionPos()
    {
        UserData user = Manager.User.GetHumanUserData();
        MapGenerator.MapInfo currentMap = user.MapInfo;
        RectTile rectTile = currentMap.mapTransform.GetComponent<RectTile>();
        championPos = rectTile.GetRectTileList();
    }

    public void SetChampionPos(List<Transform> setChamPos)
    {
        championPos = setChamPos;
    }
    public void Btn_ReRoll_Lock(PointerEventData eventData)
    {
        if (reRoll_Lock)
        {
            reRoll_Lock = false;
            image_Lock.SetActive(false);
            image_UnLock.SetActive(true);
        }
        else
        {
            reRoll_Lock = true;
            image_Lock.SetActive(true);
            image_UnLock.SetActive(false);
        }

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
        }
        else
        {
            Debug.Log($"{user.UserName}�Կ��Դ� ����� ��尡 �����ϴ�.");
        }

    }

    public void UpdatePlayerXP(UserData user)
    {
        int level = user.UserLevel;
        txt_User_Level.text = level.ToString() + "����";


        int curXp = user.UserExp;
        int maxXp = Manager.Level.ExperienceTable[level];
        txt_User_Xp.text = $"{curXp} / {maxXp}";

        SetSliderRange(curXp, maxXp);
     
    }

    public void UpdatePlayerGold(UserData user)
    {
        txt_Gold.text = user.UserGold.ToString();
    }


    /// <summary>
    /// ���� ������ ȣ��
    /// </summary>
    public void UpdateContinousBox(UserData user)
    {
        if(user.UserSuccessiveWin == 0 && user.UserSuccessiveLose != 0)
        {
            continous_Box.SetActive(true);
            contious_Image.color = Color.blue;
            continous_Count.text = user.UserSuccessiveLose.ToString();
        }
        else if(user.UserSuccessiveLose == 0 && user.UserSuccessiveWin != 0)
        {
            continous_Box.SetActive(true);
            contious_Image.color = Color.red;
            continous_Count.text = user.UserSuccessiveWin.ToString();
        }
        else if(user.UserSuccessiveWin == 0 && user.UserSuccessiveLose == 0)
        {
            continous_Box.SetActive(false);
        }
    }
    #endregion

    #region è�Ǿ� Ȯ��

    public void UpdateChampionPercent(UserData user)
    {
        int level = user.UserLevel;
       
        for(int i =0;i < uiMain.GameDataBlueprint.ChampionRandomDataList[level - 1].Probability.Length; i++)
        {
            txt_Champion_Percent[i].text = (uiMain.GameDataBlueprint.ChampionRandomDataList[level].Probability[i] * 100).ToString() + "%";
        }
    }

    #endregion

    #region è�Ǿ� �Ǹ�
    
    public void InitSellPanel(GameObject champion)
    {
        ChampionBase cBase = champion.GetComponent<ChampionBase>();

        txt_SellAmount.text = $"�Ǹ� ���� : {cBase.ChampionSellCost(Utilities.SetSlotCost(cBase.ChampionCost), cBase.ChampionLevel)} ��";
    }

    public void SellChampion(GameObject champion)
    {
        Manager.Champion.RemoveChampion(Manager.User.GetHumanUserData(), champion);

        //UpdatePlayerGold(Manager.User.GetHumanUserData());

        HexTile hex = Manager.Stage.GetParentTileInHex(champion);
        hex.championOnTile.Clear();


        ChampionFrame cFrame = champion.GetComponentInChildren<ChampionFrame>();
        cFrame.ReleaseObject();
        cFrame.ObjectOff();


        Destroy(champion);
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
