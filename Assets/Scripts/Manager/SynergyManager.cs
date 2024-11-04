using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SynergyManager
{
    #region ���� �� ������Ƽ

    private SymbolDataBlueprint symbolDataBlueprint;
    private List<KeyValuePair<string, int>> activeSynergy = new List<KeyValuePair<string, int>>();
    private List<SynergyBase> synergyBaseList = new List<SynergyBase>();

    // ������Ƽ
    public SymbolDataBlueprint SymbolDataBlueprint => symbolDataBlueprint;
    public List<SynergyBase> SynergyBaseList => synergyBaseList;
    #endregion

    #region �ʱ�ȭ ����

    public void Init(SymbolDataBlueprint symbolData)
    {
        symbolDataBlueprint = symbolData;
    }

    #endregion

    #region �ó��� �߰� �� ���� ����
    public void AddSynergyLine(UserData userData, string championName, string synergyName)
    {
        if (ReferenceEquals(synergyName, "None"))
        {
            return;
        }

        if (!userData.ChampionSynergies.ContainsKey(championName))
        {
            userData.ChampionSynergies[championName] = new SynergyData();
        }

        var synergyData = userData.ChampionSynergies[championName];
        if (!synergyData.Lines.Contains(synergyName))
        {
            synergyData.Lines.Add(synergyName);

            if (userData.Synergies_Line.ContainsKey(synergyName))
            {
                userData.Synergies_Line[synergyName]++;
            }
            else
            {
                userData.Synergies_Line[synergyName] = 1;
            }
        }
    }

    public void AddSynergyJob(UserData userData, string championName, string synergyName)
    {
        if (ReferenceEquals(synergyName, "None"))
        {
            return;
        }

        if (!userData.ChampionSynergies.ContainsKey(championName))
        {
            userData.ChampionSynergies[championName] = new SynergyData();
        }

        var synergyData = userData.ChampionSynergies[championName];
        if (!synergyData.Jobs.Contains(synergyName))
        {
            synergyData.Jobs.Add(synergyName);
        }

        if (userData.Synergies_Job.ContainsKey(synergyName))
        {
            userData.Synergies_Job[synergyName]++;
        }
        else
        {
            userData.Synergies_Job[synergyName] = 1;
        }
    }

    public void DeleteSynergy(UserData userData, string synergyName)
    {
        if (userData.Synergies_Line.ContainsKey(synergyName))
        {
            userData.Synergies_Line[synergyName]--;
        }
        else
        {
            userData.Synergies_Line[synergyName] = 0;
        }
    }

    
    // �ó��� ��ȯ �� ����� ��
    public void PrintSortedChampionSynergiesWithCount(UserData userData)
    {
        var sortedSynergies = GetSortedChampionSynergiesWithCount(userData);

        foreach (var synergy in sortedSynergies)
        {
            Debug.Log($"Synergy: {synergy.Key}, Count: {synergy.Value}");
        }
    }

    public List<KeyValuePair<string, int>> GetSortedChampionSynergiesWithCount(UserData userData)
    {
        var synergyCounts = new Dictionary<string, int>();

        foreach (var champion in userData.ChampionSynergies)
        {
            var synergyData = champion.Value;

            foreach (var line in synergyData.Lines)
            {
                if (synergyCounts.ContainsKey(line))
                {
                    synergyCounts[line]++;
                }
                else
                {
                    synergyCounts[line] = 1;
                }
            }

            foreach (var job in synergyData.Jobs)
            {
                if (synergyCounts.ContainsKey(job))
                {
                    synergyCounts[job]++;
                }
                else
                {
                    synergyCounts[job] = 1;
                }
            }
        }
        return synergyCounts.OrderByDescending(s => s.Value).ToList();
    }

    public int GetSynergyCount(UserData userData, string synergyName)
    {
        foreach (var line in userData.Synergies_Line)
        {
            if (line.Key.Contains(synergyName))
            {
                return line.Value;
            }
        }

        foreach (var job in userData.Synergies_Job)
        {
            if (job.Key.Contains(synergyName))
            {
                return job.Value;
            }
        }

        return 0;
    }

    #endregion


    #region �ó��� ���� ����
    public void UpdateSynergies(UserData userData)
    {
        // ���� ��ġ�� è�Ǿ��� �ó����� Ȯ��
        activeSynergy = GetSortedChampionSynergiesWithCount(userData);
        synergyBaseList.Clear();

        // ** ���⿡ ���� ���� �ִ� �ó����� Ȯ���ϴ� ������ �߰��ϼ���. **
        foreach (var synergy in activeSynergy)
        {
            string synergyName = synergy.Key;
            int synergyCount = synergy.Value;


            // ���޼��� �ó��� ó��
            if (synergyName == "���޼���")
            {
                ChampionLineData cLine = symbolDataBlueprint.GetChampionLineData(ChampionLine.Sugarcraft);
                ChampionJobData cJob = symbolDataBlueprint.GetChampionJobData(ChampionJob.None);
                int level = CalculateSynergyLevel(synergyCount, cLine, cJob);
                GameObject obj = symbolDataBlueprint.GetLineSynergyBase(ChampionLine.Sugarcraft);

                SynergyBase sBase = obj.GetComponent<SynergyBase>();

                if (sBase == null)
                    return;

                synergyBaseList.Add(sBase);
                sBase.UpdateLevel(userData, level);

            }
            // ���� �ó��� ó��
            else if (synergyName == "����")
            {
                ChampionLineData cLine = symbolDataBlueprint.GetChampionLineData(ChampionLine.Witchcraft);
                ChampionJobData cJob = symbolDataBlueprint.GetChampionJobData(ChampionJob.None);
                int level = CalculateSynergyLevel(synergyCount, cLine, cJob);
                GameObject obj = symbolDataBlueprint.GetLineSynergyBase(ChampionLine.Witchcraft);

                SynergyBase sBase = obj.GetComponent<SynergyBase>();

                if (sBase == null)
                    return;

                synergyBaseList.Add(sBase); 
                sBase.UpdateLevel(userData, level);
            }
            else
            {
                Debug.Log("���� ���� ����");
            }
        }
    }

    private int CalculateSynergyLevel(int count, ChampionLineData lineData, ChampionJobData jobData)
    {
        int synergyLevel = 0;

        if(lineData != null)
        {
            foreach (var symbolData in lineData.SymbolData)
            {
                if (count >= symbolData.Level)
                {
                    synergyLevel = symbolData.Level; // count�� �ش� level �̻��� ��� ������Ʈ
                }
            }
        }
        else if(jobData != null)
        {
            foreach (var symbolData in jobData.SymbolData)
            {
                if (count >= symbolData.Level)
                {
                    synergyLevel = symbolData.Level; // count�� �ش� level �̻��� ��� ������Ʈ
                }
            }
        }
        return synergyLevel;
    }

    public void ApplySynergy(UserData user)
    {
        if (synergyBaseList.Count > 0)
            StartGame(user, SynergyBaseList);
    }

    public void UnApplySynergy(UserData user)
    {
        if (synergyBaseList.Count > 0)
            OnChampionRemoved(user, SynergyBaseList);
    }
    #endregion



    #region ���� ���߿� �ű���� �ؾߵ�, �ó��� ���۰� ����

    private void StartGame(UserData user, List<SynergyBase> sBaseList)
    {
        // ���� ���� �� �ó��� Ȱ��ȭ
        foreach (var synergy in sBaseList)
        {
            synergy.Activate(user);
        }
    }

    private void OnChampionRemoved(UserData user, List<SynergyBase> sBaseList)
    {
        // è�Ǿ��� ���ŵ� �� �ó��� ��Ȱ��ȭ
        foreach (var synergy in sBaseList)
        {
            synergy.Deactivate(user);
        }
    }
    #endregion
}