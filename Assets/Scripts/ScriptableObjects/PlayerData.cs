using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Blueprints/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string playerName;
    public int health = 100;
}
