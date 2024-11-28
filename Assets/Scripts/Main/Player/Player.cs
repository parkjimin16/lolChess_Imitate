using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private UserData userData;
    [SerializeField] private TextMeshPro text_ChampionName;
    [SerializeField] private TextMeshPro text_ChampionLevel;
    [SerializeField] private Player enemyUser;


    [SerializeField] private int battleStageIndex;


    public PlayerData PlayerData
    {
        get { return playerData; }
        set { playerData = value; }
    }

    public UserData UserData
    {
        get { return userData; }
        set { userData = value; }
    }
    
    public Player EnemyUser
    {
        get { return enemyUser; }
        set { enemyUser = value; }
    }

    public int BattleStageIndex
    {
        get { return battleStageIndex; }
        set { battleStageIndex = value; }
    }
   

    public void InitPlayer(UserData user)
    {
        userData = user;

        userData.UserName = playerData.PlayerName;
        userData.UserHealthMax = playerData.Health;
        userData.UserHealth = playerData.Health;
        userData.PlayerType = playerData.PlayerType;

        text_ChampionName.text = userData.UserName;
        SetPlayerLevelText(userData.UserLevel);
    }

    public void SetPlayerLevelText(int level)
    {
        text_ChampionLevel.text = level.ToString();
    }

    public void SetBattleUser()
    {
        enemyUser = Manager.Stage.GetOpponentData(this.gameObject);
    }

    public void SetBattleStageIndex(int index)
    {
        battleStageIndex = index;

        foreach(var champion in userData.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            cBase.BattleStageIndex = index;
        }
    }
}
