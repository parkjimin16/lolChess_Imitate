using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDataBlueprint", menuName = "Blueprints/GameDataBlueprint")]
public class GameDataBlueprint : ScriptableObject
{ 
    [Header("Champion Data")]
    [SerializeField] private List<ChampionData> championDataList;
    [SerializeField] private List<ChampionRandomData> championRandomDataList;
    [SerializeField] private List<ChampionMaxCount> championMaxCount;



    public List<ChampionData> ChampionDataList => championDataList;
    public List<ChampionRandomData> ChampionRandomDataList => championRandomDataList;
    public List<ChampionMaxCount> ChampionMaxCount => championMaxCount;
}

