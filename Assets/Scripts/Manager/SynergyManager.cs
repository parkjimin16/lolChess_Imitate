using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SynergyManager
{
    #region 변수 및 프로퍼티

    private SymbolDataBlueprint symbolDataBlueprint;
    private List<KeyValuePair<string, int>> activeSynergy = new List<KeyValuePair<string, int>>();
    private List<SynergyBase> synergyBaseList = new List<SynergyBase>();

    // 프로퍼티
    public SymbolDataBlueprint SymbolDataBlueprint => symbolDataBlueprint;
    public List<SynergyBase> SynergyBaseList => synergyBaseList;
    #endregion

    #region 초기화 로직

    public void Init(SymbolDataBlueprint symbolData)
    {
        symbolDataBlueprint = symbolData;
    }

    #endregion

    #region 시너지 추가 및 삭제 로직
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

    
    // 시너지 반환 및 디버깅 용
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


    #region 시너지 적용 로직
    public void UpdateSynergies(UserData userData)
    {
        // 현재 배치된 챔피언의 시너지를 확인
        activeSynergy = GetSortedChampionSynergiesWithCount(userData);
        synergyBaseList.Clear();

        // ** 여기에 현재 켜져 있는 시너지를 확인하는 로직을 추가하세요. **
        foreach (var synergy in activeSynergy)
        {
            string synergyName = synergy.Key;
            int synergyCount = synergy.Value;


            // 달콤술사 시너지 처리
            if (synergyName == "달콤술사")
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
            // 마녀 시너지 처리
            else if (synergyName == "마녀")
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
                Debug.Log("아직 구현 안함");
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
                    synergyLevel = symbolData.Level; // count가 해당 level 이상일 경우 업데이트
                }
            }
        }
        else if(jobData != null)
        {
            foreach (var symbolData in jobData.SymbolData)
            {
                if (count >= symbolData.Level)
                {
                    synergyLevel = symbolData.Level; // count가 해당 level 이상일 경우 업데이트
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



    #region 예시 나중에 옮기던가 해야됨, 시너지 시작과 종료

    private void StartGame(UserData user, List<SynergyBase> sBaseList)
    {
        // 게임 시작 시 시너지 활성화
        foreach (var synergy in sBaseList)
        {
            synergy.Activate(user);
        }
    }

    private void OnChampionRemoved(UserData user, List<SynergyBase> sBaseList)
    {
        // 챔피언이 제거될 때 시너지 비활성화
        foreach (var synergy in sBaseList)
        {
            synergy.Deactivate(user);
        }
    }
    #endregion
}