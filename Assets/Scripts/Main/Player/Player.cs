using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private string playerName;
    [SerializeField] private int currentHealth;
    [SerializeField] private PlayerType playerType;

    private void Start()
    {
        // PlayerData를 사용하여 플레이어 초기화
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        playerName = playerData.PlayerName;
        currentHealth = playerData.Health;
        playerType = playerData.PlayerType;
    }
    public PlayerType PlayerType
    {
        get { return playerType; }
    }
    public string PlayerName
    {
        get { return playerName; }
    }
    public int CurrentHealth
    {
        get { return currentHealth; }
    }
    public int setCurrentHealth
    { set { currentHealth = value; } }
}
