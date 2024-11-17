using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ChampionSlot : MonoBehaviour
{
    #region Var 
    [Header("Top")]
    [SerializeField] private Image image_ChampionBackground;
    [SerializeField] private Image image_Champion;
    [SerializeField] private Image image_Attribute_1;
    [SerializeField] private Image image_Attribute_2;
    [SerializeField] private Image image_Attribute_3;
    [SerializeField] private TextMeshProUGUI text_Attribute_1;
    [SerializeField] private TextMeshProUGUI text_Attribute_2;
    [SerializeField] private TextMeshProUGUI text_Attribute_3;

    [SerializeField] private List<GameObject> image_Upgrades;
    [SerializeField] private Image image_Upgrade_1;
    [SerializeField] private Image image_Upgrade_2;
    [SerializeField] private Image image_Upgrade_3;


    [Header("Bottom")]
    [SerializeField] private Image image_NameBackground;
    [SerializeField] private TextMeshProUGUI text_ChampionName;
    [SerializeField] private TextMeshProUGUI text_GoldCost;


    [Header("Attribute String")]
    private List<string> attributeString = new List<string>();
    private Image[] image_Attributes;
    private TextMeshProUGUI[] text_Attributes;
    private ChampionBlueprint championBlueprint;



    public ChampionBlueprint ChampionBlueprint => championBlueprint;
    #endregion

    #region Init

    public void ChampionSlotInit(ChampionBlueprint championBlueprint, Color color)
    {
        attributeString.Clear();

        this.championBlueprint = championBlueprint;
        image_ChampionBackground.color = color;
        image_Champion.sprite = championBlueprint.ChampionImage;

        int count = GetSameChampionCount(championBlueprint);

        switch(count)
        {
            case 2:
                image_Upgrades[0].SetActive(true);
                image_Upgrades[1].SetActive(false);
                image_Upgrades[2].SetActive(false);
                break;
            case 5:
                image_Upgrades[0].SetActive(true);
                image_Upgrades[1].SetActive(true);
                image_Upgrades[2].SetActive(false);
                break;
            case 8:
                image_Upgrades[0].SetActive(true);
                image_Upgrades[1].SetActive(true);
                image_Upgrades[2].SetActive(true);
                break;
            default:
                image_Upgrades[0].SetActive(false);
                image_Upgrades[1].SetActive(false);
                image_Upgrades[2].SetActive(false);
                break;
        }




        attributeString.AddRange(new List<string> {
            Utilities.GetLineName(championBlueprint.ChampionLine_First),
             Utilities.GetLineName(championBlueprint.ChampionLine_Second),
             Utilities.GetJobName(championBlueprint.ChampionJob_First),
            Utilities.GetJobName(championBlueprint.ChampionJob_Second)
        }.Where(attribute => attribute != "None"));
        

        int startIndex = 3 - attributeString.Count;

        for (int i = 0; i < image_Attributes.Length; i++)
        {
            image_Attributes[i].gameObject.SetActive(false);
            text_Attributes[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < attributeString.Count; i++)
        {
            int index = startIndex + i;
            image_Attributes[index].gameObject.SetActive(true);
            text_Attributes[index].gameObject.SetActive(true);

            text_Attributes[index].text = attributeString[i];
        }

        text_ChampionName.text = championBlueprint.ChampionName;
        text_GoldCost.text = Utilities.SetSlotCost(championBlueprint.ChampionCost).ToString();
    }

    private void AttributeInit()
    {
        for (int i = 0; i < image_Attributes.Length; i++)
        {
            image_Attributes[i].gameObject.SetActive(false);
            text_Attributes[i].gameObject.SetActive(false);
        }
    }

    private int GetSameChampionCount(ChampionBlueprint championBlueprint)
    {
        int matchingCount = 0;

        foreach (var championObject in Manager.User.GetHumanUserData().TotalChampionObject)
        {
            ChampionBase championBase = championObject.GetComponent<ChampionBase>();
            if (championBase != null && championBase.ChampionBlueprint == championBlueprint)
            {
                matchingCount++;
            }
        }

        return matchingCount;
    }

    #endregion

    #region Unity Flow

    private void Start()
    {
        image_Attributes = new Image[] { image_Attribute_1, image_Attribute_2, image_Attribute_3 };
        text_Attributes = new TextMeshProUGUI[] { text_Attribute_1, text_Attribute_2, text_Attribute_3 };

        AttributeInit();
    }
    #endregion
}