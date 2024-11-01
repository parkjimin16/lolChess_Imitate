using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Text;

public class SynergySlot : MonoBehaviour
{
    [SerializeField] private Image sprite_SynergyIcon;
    [SerializeField] private Image sprite_BackgroundSynergyIcon;

    [SerializeField] private TextMeshProUGUI txt_SynergyCount;
    [SerializeField] private TextMeshProUGUI txt_SynergyName;
    [SerializeField] private TextMeshProUGUI txt_SynergyLevelDesc;

    public int SynergyCount;

    public void InitSlotLine(ChampionLineData cData, int levelCount)
    {
        sprite_SynergyIcon.sprite = cData.ChampionLineSprite;

        SynergyCount = levelCount;
        txt_SynergyCount.text = levelCount.ToString();
        txt_SynergyName.text = cData.ChampionLineName;
        txt_SynergyLevelDesc.text = GetSymbolDesc(cData.SymbolData, levelCount);
    }

    public void InitSlotJob(ChampionJobData cData, int levelCount)
    {
        sprite_SynergyIcon.sprite = cData.ChampionJobSprite;


        txt_SynergyCount.text = levelCount.ToString();
        txt_SynergyName.text = cData.ChampionJobName;
        txt_SynergyLevelDesc.text = GetSymbolDesc(cData.SymbolData, levelCount);
    }

    public string GetSymbolDesc(List<SymbolLevelData> symbol, int levelCount)
    {
        if (symbol == null || symbol.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder descBuilder = new StringBuilder();

        int highlightLevel = 0;
        for (int i = 0; i < symbol.Count; i++)
        {
            if (levelCount >= symbol[i].Level)
            {
                highlightLevel = symbol[i].Level; 
            }
        }

        for (int i = 0; i < symbol.Count; i++)
        {
            string levelText = symbol[i].Level.ToString();

            if (symbol[i].Level == highlightLevel)
            {
                levelText = "<color=yellow>" + levelText + "</color>"; 
            }

            descBuilder.Append(levelText);

            if (i < symbol.Count - 1)
            {
                descBuilder.Append(" / ");
            }
        }

        return descBuilder.ToString();
    }
}
