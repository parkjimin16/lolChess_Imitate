using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ItemAttribute;
using static SugarCraftCake;

public class UserManager
{
    private List<UserData> userDatas; 

    public List<UserData> UserDatas => userDatas;

    #region ÃÊ±âÈ­
    public void Init()
    {
        userDatas = new List<UserData>();

        GameObject obj = GameObject.Find("User");

        if (obj == null)
            return;

        
        for(int i = 1; i<= 8; i++)
        {
            GameObject newPlayer = Manager.Asset.InstantiatePrefab($"Player", obj.transform);
            newPlayer.name = $"Player{i}";
            PlayerData pData = Manager.Asset.GetBlueprint($"PlayerData_{i}") as PlayerData;
            Player player = newPlayer.GetComponent<Player>();
            player.PlayerData = pData;

            Manager.Game.PlayerListObject[i - 1] = newPlayer;
        }

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            UserData user = new UserData();
            user.InitUserData(3, "¹ÚÅÂ¿µ", i);
            Transform child = obj.transform.GetChild(i);
            Player player = child.GetComponent<Player>();
            user.Player = player;
            player.InitPlayer(user);
            userDatas.Add(user);
        }
    }
    #endregion

    #region À¯Àú Ä¸½¶ È¹µæ

    public void UserCrushWithCapsule(UserData user, int gold, List<string> itemList, List<string> championList)
    {
        // °ñµå
        user.UserGold += gold;

        if (itemList.Count > 0)
        {
            // ¾ÆÀÌÅÛ
            foreach (var itemId in itemList)
            {
                user.MapInfo.ItemTile[0].GenerateItem(itemId);
            }
        }
       

        if(championList.Count > 0)
        {
            // Ã¨ÇÇ¾ð
            foreach (var championId in championList)
            {
                HexTile tile = Manager.Champion.FindChampionPos(user);

                if (tile == null)
                    return;

                Transform transform = tile.transform;
                ChampionBlueprint cBlueprint = Manager.Asset.GetBlueprint(championId) as ChampionBlueprint;
                Manager.Champion.InstantiateChampion(user, cBlueprint, tile, transform);
            }
        }
       
     

    }
    #endregion

    #region À¯Àú ·ÎÁ÷
    public void UpdateMaxChampion(UserData user)
    {
        int count = 0;

        for (int i =0;i < user.UserAugmenter.Count; i++)
        {
            if (user.UserAugmenter[i].AugmenterName == "½Åº´")
            {
                count++;
            }
        }

        foreach (var item in user.TotalItemBlueprint)
        {
            if (item.ItemId == "C019" || item.ItemId == "C018" || item.ItemId == "C017")
            {
                count++;
            }
        }
        
        /*
        foreach (var champion in user.TotalChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            for (int i = 0; i < cBase.EquipItem.Count; i++)
            {
                if (cBase.EquipItem[i].ItemId == "C019" || cBase.EquipItem[i].ItemId == "C018" || cBase.EquipItem[i].ItemId == "C017")
                {
                    count++;
                }
            }
        }
        */

        user.MaxPlaceChampion = user.UserLevel + count;
    }


    public bool CheckChamipon(UserData user, string championName)
    {
        return user.BattleChampionObject.Any(championObject =>
        {
            if (championObject == null)
                return false;

            ChampionBase championBase = championObject.GetComponent<ChampionBase>();
            return championBase != null && championBase.ChampionName == championName;
        });
    }

    public UserData GetHumanUserData()
    {
        return userDatas[0];
    }

    public void ClearSynergy(UserData user)
    {
        user.ChampionSynergies_Line.Clear();
        user.ChampionSynergies_Job.Clear();
        user.ChampionSynergies.Clear();
    }

    public void InitMap(MapGenerator mapGenerator)
    {
        foreach(var user in userDatas)
        {
            user.MapInfo = mapGenerator.mapInfos.FirstOrDefault(mapInfo => mapInfo.mapId == user.UserId);
            user.SugarCraftPosition = user.MapInfo.SugarcraftPosition;
            user.PortalPosition = user.MapInfo.PortalPosition;

            GameObject augObj = Manager.Asset.InstantiatePrefab("Augmenter_Cube");
            user.MapInfo.PlayerAugBox = augObj;
            augObj.transform.position = user.MapInfo.PlayerAugmenterPosition.position;

            // GoldDisplay ÃÊ±âÈ­ ¹× ¼³Á¤
            user.GoldDisplay = new GoldDisplay(user);
            user.MapInfo.goldDisplay = user.GoldDisplay;
        }

        UserData hUser = GetHumanUserData();
        hUser.MapInfo = mapGenerator.mapInfos.FirstOrDefault(mapInfo => mapInfo.mapId == hUser.UserId);
        hUser.SugarCraftPosition = hUser.MapInfo.SugarcraftPosition;
        hUser.PortalPosition = hUser.MapInfo.PortalPosition;

    }

    public void SetShopUI(UserData user, UISceneMain uiMain)
    {
        user.UIMain = uiMain;

        user.UIMain.UIShopPanel.UpdatePlayerGold(user);
        user.UIMain.UIShopPanel.UpdatePlayerXP(user);
    }

    #endregion
}


[System.Serializable]
public class UserData
{
    [SerializeField] private Player player;
    [SerializeField] private int gold;
    [SerializeField] private string userName;
    [SerializeField] private int userHealthMax;
    [SerializeField] private int userHealth;
    [SerializeField] private int userId;
    [SerializeField] private int userLevel;
    [SerializeField] private int userExp;
    [SerializeField] private int maxPlaceChampion;
    [SerializeField] private int currentPlaceChampion;
    [SerializeField] private int successiveWin;
    [SerializeField] private int successiveLose;
    [SerializeField] private List<GameObject> totalChampionObject;
    [SerializeField] private List<GameObject> battleChampionObject;
    [SerializeField] private List<GameObject> nonBattleChamiponObject;
    [SerializeField] private Queue<GameObject> mergeChampionQueue;
    [SerializeField] private List<ItemBlueprint> totalItemBlueprint;
    [SerializeField] private List<UserSynergyData> userSynergyData;
    [SerializeField] private MapGenerator.MapInfo mapInfo;
    [SerializeField] private List<Transform> sugarCraftPosition;
    [SerializeField] private List<Transform> portalPosition;
    [SerializeField] private PlayerType playerType;
    [SerializeField] private List<AugmenterData> userAugmenter;
    [SerializeField] private List<GameObject> userItemObject;
    [SerializeField] private GoldDisplay goldDisplay;
    [SerializeField] private List<GameObject> CripObject;
    [SerializeField] private UISceneMain uiMain;
    [SerializeField] private List<GameObject> clonedEnemyChampions;
    private Dictionary<string, int> synergies_Line;
    private Dictionary<string, int> synergies_Job;
    private Dictionary<string, int> totalSynergies;
    private Dictionary<string, HashSet<string>> championSynergies_Line;
    private Dictionary<string, HashSet<string>> championSynergies_Job;

    private Dictionary<string, SynergyData> championSynergies;

    private Dictionary<GameObject, ChampionOriginalState> championOriginalStates;
    private Dictionary<GameObject, ItemOriginalState> itemOriginalStates;

    private SugarCakeOriginalState sugarCakeOriginState;

    #region Property

    public Player Player
    {
        get { return player; }
        set { player = value; }
    }

    public int UserGold
    {
        get { return gold; }
        set 
        { 
            gold = value;
            if (goldDisplay != null)
            {
                goldDisplay.SetPlayerGold(gold);
            }

            if(uiMain != null)
            {
                uiMain.UIShopPanel.UpdatePlayerGold(this);
            }
        }
    }

    public int UserHealthMax
    {
        get { return userHealthMax; }
        set { userHealthMax = value; }
    }

    public int UserHealth
    {
        get { return userHealth; }
        set { userHealth = value; }
    }

    public int UserId
    {
        get { return userId; }
        set { userId = value; } 
    }

    public string UserName
    {
        get { return userName; }
        set { userName = value; }
    }

    public int UserLevel
    {
        get { return userLevel; }
        set 
        { 
            userLevel = value;

            if (uiMain != null)
            {
                uiMain.UIShopPanel.UpdatePlayerXP(this);
                Manager.Augmenter.ApplyLevelUpAugmenter(this);
            }
        }
    }

    public int UserExp
    {
        get { return userExp; }
        set 
        {
            userExp = value;

            if(uiMain != null)
            {
                uiMain.UIShopPanel.UpdatePlayerXP(this);
            }
        }
    }

    public int MaxPlaceChampion
    {
        get { return maxPlaceChampion; }
        set { maxPlaceChampion = value; }
    }

    public int CurrentPlaceChampion
    {
        get { return BattleChampionObject != null ? BattleChampionObject.Count : 0; }
        set {  currentPlaceChampion = value; }
    }

    public int UserSuccessiveWin
    {
        get { return successiveWin; }
        set { successiveWin = value; }
    }

    public int UserSuccessiveLose
    {
        get { return successiveLose; }
        set { successiveLose = value; }
    }

    public List<GameObject> TotalChampionObject
    {
        get { return totalChampionObject; }
        set { totalChampionObject = value; }
    }
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

    public Queue<GameObject> MergeChampionQueue
    {
        get { return mergeChampionQueue; }
        set {  mergeChampionQueue = value; }
    }

    public List<UserSynergyData> UserSynergyData
    {
        get { return userSynergyData; }
        set { userSynergyData = value; }
    }

    public List<ItemBlueprint> TotalItemBlueprint
    {
        get { return totalItemBlueprint;}
        set { totalItemBlueprint = value; }
    }

    public PlayerType PlayerType
    {
        get { return playerType; }
        set { playerType = value; }
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
    public MapGenerator.MapInfo MapInfo
    { 
        get { return mapInfo; } 
        set {  mapInfo = value; }
    }

    public List<Transform> SugarCraftPosition
    {
        get { return sugarCraftPosition; }
        set {  sugarCraftPosition = value; }
    }

    public List<Transform> PortalPosition 
    { 
        get { return portalPosition; }
        set { portalPosition = value; }
    }


    public List<AugmenterData> UserAugmenter
    {
        get { return userAugmenter; }
        set { userAugmenter = value; }
    }

    public Dictionary<GameObject, ChampionOriginalState> ChampionOriginState
    {
        get { return championOriginalStates; }
        set { championOriginalStates = value; }
    }

    public List<GameObject> UserItemObject
    {
        get { return userItemObject; }
        set { userItemObject = value; }
    }

    public GoldDisplay GoldDisplay
    {
        get { return goldDisplay; }
        set { goldDisplay = value; }
    }

    public Dictionary<GameObject, ItemOriginalState> ItemOriginState
    {
        get { return itemOriginalStates; }
        set { itemOriginalStates = value; }
    }

    public List<GameObject> CripObjectList
    {
        get { return CripObject; } 
        set { CripObject = value; }
    }
    public UISceneMain UIMain
    {
        get { return uiMain; } 
        set
        {
            uiMain = value;
        }
    }
    public List<GameObject> CloneEnemyChampions
    {
        get { return clonedEnemyChampions; }
        set { clonedEnemyChampions = value;}
    }

    public SugarCakeOriginalState SugarCakeOriginalState
    {
        get { return sugarCakeOriginState; }
        set {  sugarCakeOriginState = value; }
    }
    #endregion

    #region Init
    public void InitUserData(int _gold, string _userName, int _userId)
    {
        totalChampionObject = new List<GameObject>();
        battleChampionObject = new List<GameObject>();
        nonBattleChamiponObject = new List<GameObject>();
        totalItemBlueprint = new List<ItemBlueprint>();
        synergies_Line = new Dictionary<string, int>();
        synergies_Job = new Dictionary<string, int>();
        championSynergies_Line = new Dictionary<string, HashSet<string>>();
        championSynergies_Job = new Dictionary<string, HashSet<string>>();
        championSynergies = new Dictionary<string, SynergyData>();
        sugarCraftPosition = new List<Transform>();
        portalPosition = new List<Transform>();
        userAugmenter = new List<AugmenterData>();
        championOriginalStates = new Dictionary<GameObject, ChampionOriginalState> { };
        userItemObject = new List<GameObject>();
        itemOriginalStates = new Dictionary<GameObject, ItemOriginalState>();
        CripObject = new List<GameObject>();
        mergeChampionQueue = new Queue<GameObject>();

        gold = _gold;
        userName = _userName;
        userId = _userId;
        userLevel = 1;
        userExp = 0;
        maxPlaceChampion = 1;
        currentPlaceChampion = 0;
        totalItemBlueprint.Clear();
        synergies_Line.Clear();  
    }


    #endregion

    #region User Logic

    public bool IsUserDie()
    {
        return userHealth <= 0 ? true : false;
    }

    public void SetUserHealth(int hp)
    {
        userHealth += hp;

        if(userHealth >= userHealthMax)
        {
            userHealth = userHealthMax;
        }
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
