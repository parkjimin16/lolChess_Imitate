using UnityEngine;
using UnityEngine.EventSystems;

public class BtnSynergyIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SymbolDataBlueprint symbolData;
    private string symbolName;

    [SerializeField] private ChampionLineData lineData;
    [SerializeField] private ChampionJobData jobData;

    #region Unity Flow


    #endregion

    #region Mouse Event

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowSymbolInfoUI();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //HideItemInfoUI();
    }
    #endregion

    public void SetSymbolData_Line(ChampionLineData symbolData)
    {
        lineData = symbolData;
        jobData = null;
    }
    public void SetSymbolData_Job(ChampionJobData symbolData)
    {
        jobData = symbolData;
        lineData = null;
    }




    #region Show & Hide
    private void ShowSymbolInfoUI()
    {
        if (Manager.UI.CheckPopupStack())
        {
            Manager.UI.ClosePopup(); 
        }


        var symbolInfoUIPopup = Manager.UI.ShowPopup<UIPopupSynergyDetail>();

        if(lineData == null)
        {
            symbolInfoUIPopup.SetSymbolData_Job(jobData);
        }
        else
        {
            symbolInfoUIPopup.SetSymbolData_Line(lineData);
        }
    }

    public void HideItemInfoUI()
    {
        Manager.UI.ClosePopup();
    }

    #endregion
}
