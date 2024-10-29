using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalWhisper : BaseItem
{
    private bool isDecreaseDef;
    private float originalDef;
    private float tempDef;

    public override void InitItemSkill()
    {
        isDecreaseDef = false;
        originalDef = 0f;
        tempDef = 0f;
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        if (targetChampion == null)
            return;

        ChampionBase cBase = targetChampion.GetComponent<ChampionBase>();

        if (cBase == null) 
            return;


        if (!isDecreaseDef)
            CoroutineHelper.StartCoroutine(DefDecrease(cBase));
    }

    private IEnumerator DefDecrease(ChampionBase cBase)
    {
        isDecreaseDef = true;
        originalDef = cBase.Champion_AD_Def;
        tempDef = originalDef * 0.7f;
        cBase.Champion_AD_Def = tempDef;

        yield return new WaitForSeconds(3.0f);

        cBase.Champion_AD_Def = originalDef;
        isDecreaseDef = false;
    }
}
