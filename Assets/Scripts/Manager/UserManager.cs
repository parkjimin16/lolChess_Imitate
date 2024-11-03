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

        User1_Data.InitUserData(10, "¹ÚÅÂ¿µ");
    }

    public void AddChampion(GameObject chamipon)
    {
        Manager.Champion.AddBattleChampion(User1_Data, chamipon);
    }

    public bool CheckChamipon(UserData user, string championName)
    {
        return user.BattleChampionObject.Any(championObject =>
        {
            ChampionBase championBase = championObject.GetComponent<ChampionBase>();
            return championBase != null && championBase.ChampionName == championName;
        });
    }

    public UserData GetUserData()
    {
        return User1_Data;
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
    [SerializeField] private List<UserSynergyData> userSynergyData;


    private Dictionary<string, int> synergies_Line;
    private Dictionary<string, int> synergies_Job;
    private Dictionary<string, int> totalSynergies;
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
    public List<UserSynergyData> UserSynergyData
    {
        get { return userSynergyData; }
        set { userSynergyData = value; }
    }

    public Dictionary<string, int> Synergies_Line
    {
        get { return synergies_Line; }
        set { synergies_Line = value; }
    }
    public Dictionary<string, int> Synergies_Job
    {
        get { return synergies_Job; }
        set { synergies_Job = value; }
    }

    public Dictionary<string, int> TotalSynergies
    {
        get { return totalSynergies; }
        set { totalSynergies = value; }
    }
    public Dictionary<string, HashSet<string>> ChampionSynergies_Line
    {
        get { return championSynergies_Line; }
        set { championSynergies_Line = value; }
    }

    public Dictionary<string, HashSet<string>> ChampionSynergies_Job
    {
        get { return championSynergies_Job; }
        set { championSynergies_Job = value; }
    }

    public Dictionary<string, SynergyData> ChampionSynergies
    {
        get { return championSynergies; }
        set {  championSynergies = value;}
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

[System.Serializable]
public class UserSynergyData
{
    [SerializeField] string synergyName;
    [SerializeField] int synergyCount;


    public string SynergyName
    {
        get { return synergyName; }
        set { synergyName = value; }
    }

    public int SynergyCount
    {
        get { return synergyCount; }
        set { synergyCount = value; }
    }
}
