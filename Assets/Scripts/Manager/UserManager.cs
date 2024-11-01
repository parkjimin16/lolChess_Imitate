using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserManager
{
    public UserData User1_Data;


    public void Init()
    {
        User1_Data = new UserData();


        User1_Data.InitUserData(10, "박태영");
    }

    public void AddChampion(GameObject chamipon, ChampionBlueprint cBlueprint)
    {
        User1_Data.AddBattleChampion(chamipon, cBlueprint);
    }
}


[System.Serializable]
public class UserData
{
    [SerializeField] private int gold;
    [SerializeField] private string userName;
    [SerializeField] private List<GameObject> battleChampionObject;
    [SerializeField] private List<GameObject> nonBattleChamiponObject;
    [SerializeField] private List<ChampionBlueprint> battleChampion;
    [SerializeField] private List<ChampionBlueprint> nonBattleChampion;
    [SerializeField] private List<ItemBlueprint> totalItemBlueprint;
    
    private Dictionary<string, int> synergies_Line;
    private Dictionary<string, int> synergies_Job;
    private Dictionary<string, HashSet<string>> championSynergies_Line;
    private Dictionary<string, HashSet<string>> championSynergies_Job;


    private Dictionary<string, SynergyData> championSynergies;

    #region Property

    public void SetGold(int gold)
    {
        this.gold = gold;
    }

    public int GetGold() { return gold; }

    public string GetUserName() { return userName; }

    public List<GameObject> BattleChampionObject
    {
        get { return battleChampionObject; }
        set { battleChampionObject = value; }
    }

    public List<GameObject> NonBattleChampionObject
    {
        get { return nonBattleChamiponObject; }
        set {  nonBattleChamiponObject = value; }
    }


    public List<ChampionBlueprint> BattleChampion
    {
        get { return battleChampion; }
        set { battleChampion = value; }
    }

    public List<ChampionBlueprint> NonBattleChampion
    {
        get { return nonBattleChampion; }
        set { nonBattleChampion = value; }
    }

    public List<ItemBlueprint> TotalItemBlueprint 
    {
        get { return totalItemBlueprint; }
        set { totalItemBlueprint = value; } 
    }

    public Dictionary<string, int> TotalSynergies
    {
        get { return synergies_Line; }
        set { synergies_Line = value; }
    }
    #endregion

    #region Init
    public void InitUserData(int _gold, string _userName)
    {
        battleChampionObject = new List<GameObject>();
        nonBattleChamiponObject = new List<GameObject>();
        battleChampion = new List<ChampionBlueprint>();
        nonBattleChampion = new List<ChampionBlueprint>();
        totalItemBlueprint = new List<ItemBlueprint>();
        synergies_Line = new Dictionary<string, int>();
        synergies_Job = new Dictionary<string, int>();
        championSynergies_Line = new Dictionary<string, HashSet<string>>();
        championSynergies_Job = new Dictionary<string, HashSet<string>>();
        championSynergies = new Dictionary<string, SynergyData>();


        gold = _gold;
        userName = _userName;
        battleChampion.Clear();
        nonBattleChampion.Clear();
        totalItemBlueprint.Clear();
        synergies_Line.Clear();
    }

    #endregion

    #region Champion
    public void AddBattleChampion(GameObject champion, ChampionBlueprint cBlueprint)
    {
        // 임시
        battleChampionObject.Add(champion);

        AddSynergyLine(cBlueprint.ChampionName, Utilities.GetLineName(cBlueprint.ChampionLine_First));
        AddSynergyLine(cBlueprint.ChampionName, Utilities.GetLineName(cBlueprint.ChampionLine_Second));
        AddSynergyJob(cBlueprint.ChampionName, Utilities.GetJobName(cBlueprint.ChampionJob_First));
        AddSynergyJob(cBlueprint.ChampionName, Utilities.GetJobName(cBlueprint.ChampionJob_Second));
    }

    public void RemoveBattleChampion(GameObject champion)
    {

    }
    #endregion

    #region Item

    #endregion

    #region Synergy
    private void AddSynergyLine(string championName, string synergyName)
    {
        if (ReferenceEquals(synergyName, "None"))
        {
            return;
        }

        if (!championSynergies.ContainsKey(championName))
        {
            championSynergies[championName] = new SynergyData();
        }

        var synergyData = championSynergies[championName];
        if (!synergyData.Lines.Contains(synergyName))
        {
            synergyData.Lines.Add(synergyName);

            if (synergies_Line.ContainsKey(synergyName))
            {
                synergies_Line[synergyName]++;
            }
            else
            {
                synergies_Line[synergyName] = 1; 
            }
        }
    }

    private void AddSynergyJob(string championName, string synergyName)
    {
        if (ReferenceEquals(synergyName, "None"))
        {
            return;
        }

        if (!championSynergies.ContainsKey(championName))
        {
            championSynergies[championName] = new SynergyData();
        }

        var synergyData = championSynergies[championName];
        if (!synergyData.Jobs.Contains(synergyName))
        {
            synergyData.Jobs.Add(synergyName);
        }

        if (synergies_Job.ContainsKey(synergyName))
        {
            synergies_Job[synergyName]++;
        }
        else
        {
            synergies_Job[synergyName] = 1;
        }
    }

    public void DeleteSynergy(string synergyName)
    {
        if (synergies_Line.ContainsKey(synergyName))
        {
            synergies_Line[synergyName]--;
        }
        else
        {
            synergies_Line[synergyName] = 0;
        }
    }

    public void PrintSortedChampionSynergiesWithCount()
    {
        var sortedSynergies = GetSortedChampionSynergiesWithCount();

        foreach (var synergy in sortedSynergies)
        {
            Debug.Log($"Synergy: {synergy.Key}, Count: {synergy.Value}");
        }
    }

    public List<KeyValuePair<string, int>> GetSortedChampionSynergiesWithCount()
    {
        var synergyCounts = new Dictionary<string, int>();

        foreach (var champion in championSynergies)
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
    #endregion
}

[System.Serializable]
public class SynergyData
{
    public List<string> Lines { get; private set; }
    public List<string> Jobs { get; private set; }

    public SynergyData()
    {
        Lines = new List<string>();
        Jobs = new List<string>();
    }
}