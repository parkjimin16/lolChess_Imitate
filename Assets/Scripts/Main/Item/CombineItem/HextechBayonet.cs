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

        UserData user = Manager.User.User1_Data;

        foreach(GameObject champion in user.BattleChampionObject)
        {
            if (champion == null)
                return;

            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if(cBase.Champion_CurHp < max)
            {
                max = cBase.Champion_CurHp;
                targetChampion = cBase;
            }
        }
    }
}
