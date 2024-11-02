using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Blueprints/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField]string playerName;
    [SerializeField]private int health = 100;
    [SerializeField]private PlayerType playerType;
    public string PlayerName => playerName;
    public int Health => health;
    public PlayerType PlayerType => playerType;
}
