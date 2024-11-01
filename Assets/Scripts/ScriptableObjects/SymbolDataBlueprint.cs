using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SymbolDataBlueprint", menuName = "Blueprints/SymbolDataBlueprint")]
public class SymbolDataBlueprint : ScriptableObject
{
    [SerializeField] private List<ChampionLineData> championLineData;
    [SerializeField] private List<ChampionJobData> championJobData;

    public List<ChampionLineData> ChampionLIneData => championLineData;
    public List<ChampionJobData> ChampionJobData => championJobData;

    public ChampionLineData GetChampionLineData(ChampionLine targetLine)
    {
        return championLineData.FirstOrDefault(data => data.ChampionLine == targetLine);
    }
    public ChampionLineData GetChampionLineDataToString(string lineName)
    {
        if (string.IsNullOrWhiteSpace(lineName))
        {
            throw new ArgumentException("공백이 들어옴", nameof(lineName));
        }

        if (lineName == "오류")
            return null;

        var lineData = championLineData.FirstOrDefault(data => data.ChampionLineName == lineName);   

        return lineData;
    }

    public ChampionJobData GetChampionJobData(ChampionJob targetJob)
    {
        return championJobData.FirstOrDefault(data => data.ChampionJob == targetJob);
    }

    public ChampionJobData GetChampionJobDataToString(string jobName)
    {
        if (string.IsNullOrWhiteSpace(jobName))
        {
            throw new ArgumentException("공백이 들어옴.", nameof(jobName));
        }

        if (jobName == "오류")
            return null;

        var jobData = championJobData.FirstOrDefault(data => data.ChampionJobName == jobName);

        return jobData;
    }
}


[System.Serializable]
public class ChampionLineData
{
    [SerializeField] private ChampionLine championLine;
    [SerializeField] private Sprite championLineSprite;
    [SerializeField] private string championLineName;
    [SerializeField, TextArea] string championLineDesc;
    [SerializeField] private List<SymbolLevelData> symbolData;
    [SerializeField] private List<ChampionBlueprint> championBlueprint;

    public ChampionLine ChampionLine => championLine;
    public Sprite ChampionLineSprite => championLineSprite;
    public string ChampionLineName => championLineName;
    public string ChampionLineDesc => championLineDesc;
    public List<SymbolLevelData> SymbolData => symbolData;
    public List<ChampionBlueprint> ChampionBlueprint => championBlueprint;
}

[System.Serializable]
public class ChampionJobData
{
    [SerializeField] private ChampionJob championJob;
    [SerializeField] private Sprite championJobSprite;
    [SerializeField] private string championJobName;
    [SerializeField, TextArea] string championJobDesc;
    [SerializeField] private List<SymbolLevelData> symbolData;
    [SerializeField] private List<ChampionBlueprint> championBlueprint;

    public ChampionJob ChampionJob => championJob;
    public Sprite ChampionJobSprite => championJobSprite;
    public string ChampionJobName => championJobName;
    public string ChampionJobDesc => championJobDesc;
    public List<SymbolLevelData> SymbolData => symbolData;
    public List<ChampionBlueprint> ChampionBlueprint => championBlueprint;

}


[System.Serializable]
public class SymbolLevelData
{
    [SerializeField] private int level;
    [SerializeField, TextArea] string desc;
    [SerializeField] private SymbolColor color;

    public int Level => level;
    public string Desc => desc;
    public SymbolColor Color => color;
}

