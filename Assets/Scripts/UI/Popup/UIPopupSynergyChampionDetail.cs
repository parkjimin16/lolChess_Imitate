using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;

public class UIPopupSynergyChampionDetail : UIPopup
{
    [SerializeField] private TextMeshProUGUI championName;
    [SerializeField] private List<GameObject> symbolText;


    #region Init
    protected override void Init()
    {
        base.Init();
    }

    #endregion

    #region UI Update Logic
    public void SetChampionSymbolPopupData(string name, List<string> symbol)
    {
        championName.text = name;

        for (int i = 0; i < symbolText.Count; i++)
        {
            if (i < symbol.Count) 
            {
                symbolText[i].SetActive(true); 
                TextMeshProUGUI symbolTextComponent = symbolText[i].GetComponent<TextMeshProUGUI>();
                if (symbolTextComponent != null)
                {
                    symbolTextComponent.text = symbol[i]; 
                }
            }
            else
            {
                symbolText[i].SetActive(false);
            }
        }
    }
    #endregion
}
