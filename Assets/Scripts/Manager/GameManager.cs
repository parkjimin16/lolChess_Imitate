using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    #region 변수 & 프로퍼티
    [SerializeField] private GameObject[] playerListObject;
    [SerializeField] private List<Player> playerList;

    private bool isBattle;



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

    public bool IsBattle
    {
        get { return isBattle; }
        set { isBattle = value; }
    }


    #endregion

    public void InitGameManager()
    {
        playerListObject = new GameObject[8];
        playerList = new List<Player>();

        isBattle = false;
    }

    #region 유저




    #endregion
}


