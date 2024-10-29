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
            return;
        }

        SelectChampion();

        if (targetChampion == null)
        {
            return;
        }

        targetChampion.ChampionHpMpController.AddHealth((int)(targetChampion.Champion_TotalDamage * 0.2f), targetChampion.HealHpValue);
    }

    private void SelectChampion()
    {
        int max = 100000000;

        foreach(ChampionBase champion in Manager.Game.BattleChampionBase)
        {
            if (champion == null)
                return;

            if(champion.Champion_CurHp < max)
            {
                max = champion.Champion_CurHp;
                targetChampion = champion;
            }
        }
    }
}
