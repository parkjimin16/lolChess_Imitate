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

            // ChampionLineData�� ChampionJobData ��������
            ChampionLineData line = symbolData.GetChampionLineDataToString(synergyName);
            ChampionJobData job = symbolData.GetChampionJobDataToString(synergyName);

            // Ȱ��ȭ�� ���� ���� ���
            int activeLevelsCount = 0;

            // Ȱ��ȭ�� ���� �� ���
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

            // ���� ����Ʈ�� �߰�
            finalSortedList.Add((synergyName, synergyCount, activeLevelsCount));
        }

        // ���� ���� (Ȱ��ȭ�� ���� ���� ���� �����ϰ�, �� ���� Value �������� ����)
        finalSortedList = finalSortedList
            .OrderByDescending(item => item.ActiveLevelsCount) // Ȱ��ȭ�� ���� �� ���� ��������
            .ThenByDescending(item => item.SynergyCount) // Value ���� ��������
            .ToList();


        for (int i = 0; i < finalSortedList.Count; i++)
        {
            var (synergyName, synergyCount, _) = finalSortedList[i];

            // ChampionLineData�� ChampionJobData ��������
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