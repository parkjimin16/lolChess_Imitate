using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private List<GameObject> battleChampion;
    private List<GameObject> nonBattleChampion;

    private List<ChampionBase> battleChampionBase;
    private List<ChampionBase> nonBattleChampionBase;

    public List<GameObject> BattleChampion => battleChampion;

    public List<GameObject> NonBattleChampion => nonBattleChampion;
    
    public List<ChampionBase> BattleChampionBase => battleChampionBase;

    public List<ChampionBase> NonBattleChampionBase => nonBattleChampionBase;

    public void InitGameManager()
    {
        battleChampion = new List<GameObject>();
        nonBattleChampion = new List<GameObject>();
        battleChampionBase = new List<ChampionBase>();
        nonBattleChampionBase = new List<ChampionBase>();
    }

    public void AddBattleChampion(GameObject champion)
    {
        ChampionBase cBase = champion.GetComponent<ChampionBase>();

        battleChampion.Add(champion);
        battleChampionBase.Add(cBase);
    }

    public void RemoveBattleChampion(GameObject champion)
    {
        if (battleChampion.Contains(champion))
        {
            battleChampion.Remove(champion);
        }

        ChampionBase cBase = champion.GetComponent<ChampionBase>();
        if (cBase != null && battleChampionBase.Contains(cBase))
        {
            battleChampionBase.Remove(cBase);
        }
    }

    #region À¯Àú




    #endregion
}


