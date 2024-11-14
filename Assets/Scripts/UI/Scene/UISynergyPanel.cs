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
        synergyData = Manager.Synerge.GetSortedChampionSynergiesWithCount(Manager.User.GetHumanUserData());
        int totalSynergies = synergyData.Count;

        synergySlots.ForEach(slot => slot.SetActive(true));

        // 정렬된 시너지 데이터를 가져옴
        var sortedSynergyData = synergyData
            .OrderByDescending(s => s.Value)
            .Select(entry => new
            {
                SynergyName = entry.Key,
                SynergyCount = entry.Value,
                ActiveLevelsCount = CalculateActiveLevels(entry.Key, entry.Value)
            })
            .OrderByDescending(entry => entry.ActiveLevelsCount)
            .ThenByDescending(entry => entry.SynergyCount)
            .ToList();

        // 슬롯 초기화
        for (int i = 0; i < sortedSynergyData.Count && i < synergySlotsScript.Count; i++)
        {
            var synergyEntry = sortedSynergyData[i];
            ChampionLineData line = symbolData.GetChampionLineDataToString(synergyEntry.SynergyName);
            ChampionJobData job = symbolData.GetChampionJobDataToString(synergyEntry.SynergyName);

            if (line != null)
            {
                synergySlotsScript[i].InitSlotLine(line, synergyEntry.SynergyCount);
            }
            else if (job != null)
            {
                synergySlotsScript[i].InitSlotJob(job, synergyEntry.SynergyCount);
            }
        }

        for (int i = sortedSynergyData.Count; i < synergySlots.Count; i++)
        {
            synergySlots[i].SetActive(false);
        }

        Manager.Synerge.UpdateSynergies(Manager.User.GetHumanUserData());
    }

    private int CalculateActiveLevels(string synergyName, int synergyCount)
    {
        int activeLevelsCount = 0;

        ChampionLineData line = symbolData.GetChampionLineDataToString(synergyName);
        ChampionJobData job = symbolData.GetChampionJobDataToString(synergyName);

        if (line != null)
        {
            activeLevelsCount += line.SymbolData.Count(symbol => synergyCount >= symbol.Level);
        }

        if (job != null)
        {
            activeLevelsCount += job.SymbolData.Count(symbol => synergyCount >= symbol.Level);
        }

        return activeLevelsCount;
    }
}