using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;


public class UIPopupSkillDetail : UIPopup
{
    #region Field & Property
    private SkillBlueprint skillBlueprint;
    private ChampionBase championBase;


    [Header("Top")]
    [SerializeField] private Image image_skill_Icon;
    [SerializeField] private TextMeshProUGUI txt_Skill_Name;

    [Header("Middle")]
    [SerializeField] private TextMeshProUGUI txt_Skill_Desc;


    [Header("Botton")]
    [SerializeField] private List<GameObject> obj_Desc;
    [SerializeField] private List<TextMeshProUGUI> txt_Desc;

    [SerializeField] private List<GameObject> obj_Level_Percent;
    [SerializeField] private List<TextMeshProUGUI> txt_Level_Percent;

    #endregion


    #region Init

    public void InitSkillDetailPopup(SkillBlueprint sBlueprint, ChampionBase cBase)
    {
        skillBlueprint = sBlueprint;
        championBase = cBase;

        SetSkillDetailPopup();
    }

    #endregion

    private void SetSkillDetailPopup()
    {
        image_skill_Icon.sprite = skillBlueprint.SkillSprite;
        txt_Skill_Name.text = skillBlueprint.SkillName;

        txt_Skill_Desc.text = skillBlueprint.Description;

        int count = skillBlueprint.SkillLevelData.Count;

        for (int i = 0; i < obj_Level_Percent.Count; i++) 
        {
            if(i < count)
            {
                obj_Desc[i].SetActive(true);
                obj_Level_Percent[i].SetActive(true);

                txt_Desc[i].text = skillBlueprint.SkillLevelData[i].Description;
                
                StringBuilder valueString = new StringBuilder();

                valueString.Append("<color=#FFFFFF>[ </color>");

                for (int j=0;j < skillBlueprint.SkillLevelData[i].Value.Count; j++)
                {
                    string valueText = string.Empty;

                    if(skillBlueprint.SkillLevelData[i].Value[j] <= 5)
                    {
                        valueText = (skillBlueprint.SkillLevelData[i].Value[j] * 100).ToString() + "%";
                    }
                    else
                    {
                        valueText = (skillBlueprint.SkillLevelData[i].Value[j]).ToString() + "%";
                    }
                   

                    if (j < championBase.ChampionLevel)
                    {
                        valueText = $"<color=#FFFFFF>{valueText}</color>";
                    }
                    else
                    {
                        valueText = $"<color=#808080>{valueText}</color>";
                    }

                    valueString.Append(valueText);

                    if (j < skillBlueprint.SkillLevelData[i].Value.Count - 1)
                    {
                        valueString.Append("/ ");
                    }
                }

                valueString.Append("<color=#FFFFFF> ]</color>");

                txt_Level_Percent[i].text += " " + valueString.ToString();
            }
            else
            {
                obj_Desc[i].SetActive(false);
                obj_Level_Percent[i].SetActive(false);
            }
        }

    }

}
