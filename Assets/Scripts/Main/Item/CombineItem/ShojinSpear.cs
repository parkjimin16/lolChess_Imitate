using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShojinSpear : BaseItem
{
    public override void InitTargetObject(GameObject targetChampion)
    {
        if (EquipChampionBase == null)
            return;

        EquipChampionBase.ChampionHpMpController.ManaPlus(5);
    }
}
