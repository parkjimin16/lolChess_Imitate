using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HextechBayonet : BaseItem
{
    private ChampionBase targetChampion;
    

    public override void InitItemSkill()
    {
        
    }

    public override void InitTargetObject(GameObject target)
    {
        if (target == null)
        {
            Debug.Log("Target");
            return;
        }



        SelectChampion();

        if (targetChampion == null)
        {
            Debug.Log("TargetChampion");
            return;
        }

        Debug.Log($"Target Champion Name : {targetChampion.name} +  || Cur Health : {targetChampion.CurHP}");


        targetChampion.ChampionHpMpController.AddHealth((int)(targetChampion.Display_TotalDamage * 0.2f));
    }

    private void SelectChampion()
    {
        int max = 100000000;

        foreach(ChampionBase champion in Manager.Game.BattleChampionBase)
        {
            if (champion == null)
                return;

            if(champion.Display_CurHp < max)
            {
                max = champion.Display_CurHp;
                targetChampion = champion;
            }
        }
    }
}
