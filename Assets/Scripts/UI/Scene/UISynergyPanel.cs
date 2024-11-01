using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class UISynergyPanel : UIBase
{
    [SerializeField] private SymbolDataBlueprint symbolData;
    [SerializeField] private List<GameObject> synergySlots;
    [SerializeField] private List<SynergySlot> synergySlotsScript;

    [SerializeField] private List<KeyValuePair<string, int>> synergyData;

    public void InitSynergyBtn(SymbolDataBlueprint symbol)
    {
        symbolData = symbol;
        synergyData= new List<KeyValuePair<string, int>>();
    }

    public void UpdateSynergy()
    {
        synergyData.Clear();
        synergyData = Manager.User.User1_Data.GetSortedChampionSynergiesWithCount();
        int totalSynergies = synergyData.Count;


        int slotIndex = 0;

        for (int i = 0; i < synergySlotsScript.Count; i++)
        {
            synergySlots[i].SetActive(true);
        }

        for (int i = 0; i < totalSynergies; i++)
        {
            var synergyEntry = synergyData[i];
            string synergyName = synergyEntry.Key;

            ChampionLineData line = symbolData.GetChampionLineDataToString(synergyName);
            ChampionJobData job = symbolData.GetChampionJobDataToString(synergyName);

            int synergyCount = synergyEntry.Value;

            if (i < synergySlotsScript.Count)
            {
                if (line != null)
                {
                    synergySlotsScript[i].InitSlotLine(line, synergyCount);
                }
                else if (job != null)
                {
                    synergySlotsScript[i].InitSlotJob(job, synergyCount);
                }
                slotIndex++;
            }
        }

        for (int i = slotIndex; i < synergySlots.Count; i++)
        {
            synergySlots[i].SetActive(false);
        }

        //SortedSlots();
    }

    private void SortedSlots()
    {
        synergySlotsScript = synergySlotsScript
       .OrderBy(slot => slot.SynergyCount)
       .ToList(); 
    }
}