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

        var sortedSynergyData = synergyData
       .OrderByDescending(s => s.Value)
       .ToList();

        var finalSortedList = new List<(string SynergyName, int SynergyCount, int ActiveLevelsCount)>();


        for (int i = 0; i < sortedSynergyData.Count; i++)
        {
            var synergyEntry = sortedSynergyData[i];
            string synergyName = synergyEntry.Key;
            int synergyCount = synergyEntry.Value;

            // ChampionLineData와 ChampionJobData 가져오기
            ChampionLineData line = symbolData.GetChampionLineDataToString(synergyName);
            ChampionJobData job = symbolData.GetChampionJobDataToString(synergyName);

            // 활성화된 레벨 수를 계산
            int activeLevelsCount = 0;

            // 활성화된 레벨 수 계산
            if (line != null)
            {
                foreach (var symbol in line.SymbolData)
                {
                    if (synergyCount >= symbol.Level)
                    {
                        activeLevelsCount++;
                    }
                }
            }

            if (job != null)
            {
                foreach (var symbol in job.SymbolData)
                {
                    if (synergyCount >= symbol.Level)
                    {
                        activeLevelsCount++;
                    }
                }
            }

            // 최종 리스트에 추가
            finalSortedList.Add((synergyName, synergyCount, activeLevelsCount));
        }

        // 최종 정렬 (활성화된 레벨 수로 먼저 정렬하고, 그 다음 Value 기준으로 정렬)
        finalSortedList = finalSortedList
            .OrderByDescending(item => item.ActiveLevelsCount) // 활성화된 레벨 수 기준 내림차순
            .ThenByDescending(item => item.SynergyCount) // Value 기준 내림차순
            .ToList();


        for (int i = 0; i < finalSortedList.Count; i++)
        {
            var (synergyName, synergyCount, _) = finalSortedList[i];

            // ChampionLineData와 ChampionJobData 가져오기
            ChampionLineData line = symbolData.GetChampionLineDataToString(synergyName);
            ChampionJobData job = symbolData.GetChampionJobDataToString(synergyName);

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

        /*
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
        */
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