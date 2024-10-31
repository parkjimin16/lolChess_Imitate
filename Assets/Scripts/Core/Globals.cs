using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Globals : MonoBehaviour
{
    public Dictionary<ChampionCost, int> Purchase_Cost;
    public Dictionary<ChampionCost, int> Sell_Cost;

    public Dictionary<ChampionLine, Sprite> ChampionLine_Sprite;
    public Dictionary<ChampionJob, Sprite> ChapmionJob_Sprite;
}