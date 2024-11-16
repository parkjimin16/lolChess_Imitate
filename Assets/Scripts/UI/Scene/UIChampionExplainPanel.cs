using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UIChampionExplainPanel : UIBase
{
    [SerializeField] private ChampionBase championBase;
    [SerializeField] private SymbolDataBlueprint symbolData;
    [SerializeField] private SkillBlueprint skillBlueprint;

    [Header("Champion Info")]
    [SerializeField] private List<GameObject> level;
    [SerializeField] private Image image_Champion;
    [SerializeField] private List<GameObject> iconBox;
    [SerializeField] private List<Image> image_Icon;
    [SerializeField] private List<TextMeshProUGUI> txt_Icon;

    [SerializeField] private TextMeshProUGUI txt_ChampionName;
    [SerializeField] private Image image_NameTag;
    [SerializeField] private TextMeshProUGUI txt_ChampionPositon;
    [SerializeField] private TextMeshProUGUI txt_ChampionCost;

    [SerializeField] private Slider slider_Hp;
    [SerializeField] private Slider slider_Mana;
    [SerializeField] private TextMeshProUGUI txt_Hp;
    [SerializeField] private TextMeshProUGUI txt_Mana;

    [Header("Champion Skill")]
    [SerializeField] private Image image_Skill_Icon;
    [SerializeField] private TextMeshProUGUI txt_ChampionPos;
    [SerializeField] private TextMeshProUGUI txt_Champion_Attack_Range;


    [Header("Champion Item")]
    [SerializeField] private List<Image> image_Item;

    [Header("Champion Stats")]
    [SerializeField] private TextMeshProUGUI txt_AD_Power;
    [SerializeField] private TextMeshProUGUI txt_AP_Power;
    [SerializeField] private TextMeshProUGUI txt_AD_Def;
    [SerializeField] private TextMeshProUGUI txt_AP_Def;
    [SerializeField] private TextMeshProUGUI txt_Atk_Spd;
    [SerializeField] private TextMeshProUGUI txt_Critical_Percent;
    [SerializeField] private TextMeshProUGUI txt_Critical_Damage;
    [SerializeField] private TextMeshProUGUI txt_Blood_Suck;
    [SerializeField] private TextMeshProUGUI txt_Total_Power;
    [SerializeField] private TextMeshProUGUI txt_Total_Def;
    [SerializeField] private TextMeshProUGUI txt_Sell_Cost;

    public void InitChampionExplainPanel(SymbolDataBlueprint symbol)
    {
        symbolData = symbol;
    }

    public void UpdateChampionExplainPanel(ChampionBase cBase)
    {
        if(!this.gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        championBase = cBase;
        skillBlueprint = cBase.SkillBlueprint;

        InitChampionExplainPanelDetail();
    }

    #region Init
    private void InitChampionExplainPanelDetail()
    {
        InitChampionInfo();
        InitChampionSKill();
        InitChampionItem();
        InitChampionStats();
    }

    private void InitChampionInfo() 
    {
        for(int i=0;i < level.Count; i++)
        {
            if(i < championBase.ChampionLevel)
                level[i].SetActive(true);
            else
                level[i].SetActive(false);
        }

        image_Champion.sprite = championBase.ChampionBlueprint.ChampionImage;


        List<string> names = new List<string>();
        List<Sprite> sprites = new List<Sprite>();

        if(championBase.ChampionLine_First != ChampionLine.None)
        {
            sprites.Add(symbolData.GetChampionLineData(championBase.ChampionLine_First).ChampionLineSprite);
            names.Add(Utilities.GetLineName(championBase.ChampionLine_First));
        }

        if (championBase.ChampionLine_Second != ChampionLine.None)
        {
            sprites.Add(symbolData.GetChampionLineData(championBase.ChampionLine_Second).ChampionLineSprite);
            names.Add(Utilities.GetLineName(championBase.ChampionLine_Second));
        }

        if (championBase.ChampionJob_First != ChampionJob.None)
        {
            sprites.Add(symbolData.GetChampionJobData(championBase.ChampionJob_First).ChampionJobSprite);
            names.Add(Utilities.GetJobName(championBase.ChampionJob_First));
        }
        if (championBase.ChampionJob_Second != ChampionJob.None)
        {
            sprites.Add(symbolData.GetChampionJobData(championBase.ChampionJob_Second).ChampionJobSprite);
            names.Add(Utilities.GetJobName(championBase.ChampionJob_Second));
        }

        for (int i = 0; i < txt_Icon.Count; i++)
        {
            if (i < names.Count)
            {
                iconBox[i].SetActive(true);
                txt_Icon[i].text = names[i];
                image_Icon[i].sprite = sprites[i];
            }
            else
                iconBox[i].SetActive(false);
        }


        txt_ChampionName.text = championBase.ChampionName;
        txt_ChampionCost.text = Utilities.SetSlotCost(championBase.ChampionCost).ToString();

        SetHpSlider(championBase.Champion_MaxHp, championBase.Champion_CurHp);
        SetManaSlider(championBase.Champion_MaxMana, championBase.Champion_CurMana);
    }


    private void InitChampionSKill() 
    {
        image_Skill_Icon.sprite = skillBlueprint.SkillSprite;

        image_NameTag.color = Utilities.SetSlotColor(championBase.ChampionCost);
        
        if(championBase.Attack_Range <= 2)
        {
            txt_ChampionPositon.text = "전방 챔피언";
        }
        else
        {
            txt_ChampionPositon.text = "후방 챔피언";
        }

        txt_Champion_Attack_Range.text = $"사거리 : {championBase.Attack_Range}";

    }
    private void InitChampionItem() 
    {
        for(int i=0;i < image_Item.Count; i++)
        {
            if(i < championBase.EquipItem.Count)
            {
                image_Item[i].sprite = championBase.EquipItem[i].Icon;
            }
            else
            {
                image_Item[i].sprite = null;
            }
        }
    }
    private void InitChampionStats()
    {
        txt_AD_Power.text = championBase.Champion_AD_Power.ToString();
        txt_AP_Power.text = championBase.Champion_AP_Power.ToString();
        txt_AD_Def.text = championBase.Champion_AD_Def.ToString();
        txt_AP_Def.text = championBase.Champion_AP_Def.ToString();
        txt_Atk_Spd.text = championBase.Champion_Atk_Spd.ToString();
        txt_Critical_Percent.text = $"{championBase.Champion_Critical_Percent * 100}%";
        txt_Critical_Damage.text = $"{championBase.Champion_Critical_Power * 100}%";
        txt_Blood_Suck.text = $"{championBase.Champion_Blood_Suck * 100}%";
        txt_Total_Power.text = $"{championBase.Champion_Power_Upgrade * 100}%";
        txt_Total_Def.text = $"{championBase.Champion_Total_Def * 100}%";


        txt_Sell_Cost.text = Utilities.SetSlotCost(championBase.ChampionCost).ToString();
    }

    #endregion


    private void SetHpSlider(float maxHp, float curHp)
    {
        if (slider_Hp != null)
        {
            slider_Hp.maxValue = maxHp;
            slider_Hp.value = Mathf.Clamp(curHp, 0, maxHp);
            txt_Hp.text = $"{curHp} / {maxHp}";
        }
        else
        {
            Debug.LogError("slider_Hp가 연결되지 않았습니다.");
        }
    }

    private void SetManaSlider(float maxMana, float curMana)
    {
        if (slider_Mana != null)
        {
            slider_Mana.maxValue = maxMana;
            slider_Mana.value = Mathf.Clamp(curMana, 0, maxMana);
            txt_Mana.text = $"{curMana} / {maxMana}";
        }
        else
        {
            Debug.LogError("slider_Mana가 연결되지 않았습니다.");
        }
    }


}
