using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    [SerializeField] private GameObject[] playerListObject;
    [SerializeField] private List<Player> playerList;

    public GameObject[] PlayerListObject
    {
        get { return playerListObject; }
        set { playerListObject = value; }
    }
    public List<Player> PlayerList
    {
        get { return playerList; }
        set { playerList = value; }
    }

    public void InitGameManager()
    {
        playerListObject = new GameObject[8];
        playerList = new List<Player>();
    }

    #region À¯Àú




    #endregion
}


