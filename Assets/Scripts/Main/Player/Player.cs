using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private UserData userData;


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

    public void InitPlayer(UserData user)
    {
        userData = user;

        userData.UserName = playerData.PlayerName;
        userData.UserHealth = playerData.Health;
        userData.PlayerType = playerData.PlayerType;
    }
}
