using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPopupSynergyDetail : UIPopup
{
    #region Field & Property
    [Header("UI Object")]
    [SerializeField] private GameObject uiObject;
    [SerializeField] private RectTransform synergyDetailPopup;

    [Header("UI")]
    // Top
    [SerializeField] private Image image_SynergyIcon;
    [SerializeField] private TextMeshProUGUI txt_SynergyName;

    // Middle
    [SerializeField] private TextMeshProUGUI txt_SynergyDesc;
    [SerializeField] private TextMeshProUGUI txt_synergyDetail;

    // Bottom
    [SerializeField] private GameObject[] image_ChampionImage;

    private ChampionLineData lineData;
    private ChampionJobData jobData;

    #endregion


    #region Init
    protected override void Init()
    {
        base.Init();

        SetImage();
        SetTextMeshProUGUI();
        InitSymbolData();
    }


    private void SetImage()
    {
        SetUI<Image>();


        image_SynergyIcon = GetUI<Image>("image_SynergyIcon");
    }

    private void SetTextMeshProUGUI()
    {
        SetUI<TextMeshProUGUI>();

        txt_SynergyName = GetUI<TextMeshProUGUI>("Txt_SynergyName");
        txt_SynergyDesc = GetUI<TextMeshProUGUI>("Txt_SynergyDesc");
        txt_synergyDetail = GetUI<TextMeshProUGUI>("Txt_SynergyDetail");
    }

    private void InitSymbolData()
    {
        lineData = new ChampionLineData();
        jobData = new ChampionJobData();

        lineData = null;
        jobData = null;
    }


    #endregion

    #region UI Update Logic
    public void SetSymbolData_Line(ChampionLineData symbolData)
    {
        lineData = symbolData;
        jobData = null;

        SetUISynergyDetail_Line();
        UpdateUIPanelSize();
    }

    public void SetSymbolData_Job(ChampionJobData symbolData)
    {
        jobData = symbolData;
        lineData = null;

        SetUISynergyDetail_Job();
        UpdateUIPanelSize();
    }

    private void SetUISynergyDetail_Line()
    {
        image_SynergyIcon.sprite = lineData.ChampionLineSprite;
        txt_SynergyName.text = lineData.ChampionLineName;

        txt_SynergyDesc.text = lineData.ChampionLineDesc;
        txt_synergyDetail.text = UpdateSynergyDetailText_Line(lineData);

        int idx = 0;

        foreach (var champion in lineData.ChampionBlueprint)
        {
            if (idx >= image_ChampionImage.Length) break;

            var imageObj = image_ChampionImage[idx];
            imageObj.SetActive(true);

            var slot = imageObj.GetComponent<SynergyChampionImageSlot>();

            if (slot != null)
            {
                UserData user = Manager.User.GetHumanUserData();
                Color outlineColor = Utilities.SetSlotColor(champion.ChampionCost);
                List<string> name = GetChampionDetails(champion);
                Color championColor = Color.black;


                if (!Manager.User.CheckChamipon(user, champion.ChampionName))
                {
                    championColor = Color.gray;
                }
                else
                {
                    championColor = Color.white;
                }

                slot.InitSlot(champion.ChampionImage, championColor, outlineColor, name, champion.ChampionName);
            }

            idx++;
        }

        for (int i = idx; i < image_ChampionImage.Length; i++)
        {
            image_ChampionImage[i].SetActive(false);
        }
    }

    private void SetUISynergyDetail_Job()
    {
        image_SynergyIcon.sprite = jobData.ChampionJobSprite;
        txt_SynergyName.text = jobData.ChampionJobName;

        txt_SynergyDesc.text = jobData.ChampionJobDesc;
        txt_synergyDetail.text = UpdateSynergyDetailText_Job(jobData);

        int idx = 0;

        foreach (var champion in jobData.ChampionBlueprint)
        {
            if (idx >= image_ChampionImage.Length) break;

            var imageObj = image_ChampionImage[idx];
            imageObj.SetActive(true);

            var slot = imageObj.GetComponent<SynergyChampionImageSlot>();

            if (slot != null)
            {
                UserData user = Manager.User.GetHumanUserData();
                Color outlineColor = Utilities.SetSlotColor(champion.ChampionCost);
                List<string> name = GetChampionDetails(champion);
                Color championColor = Color.black;


                if (!Manager.User.CheckChamipon(user, champion.ChampionName))
                {
                    championColor = Color.gray;
                }
                else
                {
                    championColor = Color.white;
                }

                slot.InitSlot(champion.ChampionImage, championColor, outlineColor, name, champion.ChampionName);
            }



            idx++;
        }

        for (int i = idx; i < image_ChampionImage.Length; i++)
        {
            image_ChampionImage[i].SetActive(false);
        }
    }

    private string UpdateSynergyDetailText_Line(ChampionLineData lineData)
    {
        int count = Manager.Synergy.GetSynergyCount(Manager.User.GetHumanUserData(), lineData.ChampionLineName);

        string synergyDetailText = "";

        foreach (var symbolData in lineData.SymbolData)
        {
            string color;

            if (count >= symbolData.Level)
            {
                color = "white"; 
            }
            else
            {
                color = "#7F7F7F"; 
            }

            synergyDetailText += $"<color={color}>( {symbolData.Level} ) {symbolData.Desc}</color>\n";
        }

        return synergyDetailText;
    }

    private string UpdateSynergyDetailText_Job(ChampionJobData jobData)
    {
        int count = Manager.Synergy.GetSynergyCount(Manager.User.GetHumanUserData(), jobData.ChampionJobName);

        string synergyDetailText = "";

        foreach (var symbolData in jobData.SymbolData)
        {
            string color;

            if (count >= symbolData.Level)
            {
                color = "white"; 
            }
            else
            {
                color = "#7F7F7F";
            }

            synergyDetailText += $"<color={color}>( {symbolData.Level} ) {symbolData.Desc}</color>\n";
        }

        return synergyDetailText;
    }

    private List<string> GetChampionDetails(ChampionBlueprint champion)
    {
        var details = new List<string>();

        if (champion.ChampionLine_First != ChampionLine.None) 
        {
            details.Add(Utilities.GetLineName(champion.ChampionLine_First));
        }

        if (champion.ChampionLine_Second != ChampionLine.None)
        {
            details.Add(Utilities.GetLineName(champion.ChampionLine_Second));
        }

        if (champion.ChampionJob_First != ChampionJob.None)
        {
            details.Add(Utilities.GetJobName(champion.ChampionJob_First));
        }

        if (champion.ChampionJob_Second != ChampionJob.None)
        {
            details.Add(Utilities.GetJobName(champion.ChampionJob_Second));
        }

        return details;
    }
    #endregion 

    #region UI Object Logic

    private void UpdateUIPanelSize()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(synergyDetailPopup);
    }

    public void SetPosition(Vector2 position)
    {
        uiObject.transform.position = position;
    }

    #endregion

   
}
