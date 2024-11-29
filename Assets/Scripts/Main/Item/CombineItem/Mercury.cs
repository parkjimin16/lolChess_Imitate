 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mercury : BaseItem
{
    private float adSpd;
    private bool isUse;

    private ItemAttribute adSpeedItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        isUse = false;
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.AD_Speed)
            {
                adSpeedItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }
    }

    public override void ResetItem()
    {
        isUse = false;
        adSpeedItemAttribute.InitItemAttributeValue();
    }
    public override void InitTargetObject(GameObject targetChampion)
    {
        if (EquipChampionBase == null || isUse)
            return;

        CoroutineHelper.StartCoroutine(RemoveAdSpdAfterDelay(14f)); 
    }

    private IEnumerator RemoveAdSpdAfterDelay(float delay)
    {
        isUse = true;

        adSpd = 0.6f;
        adSpeedItemAttribute.SetAttributeValue(adSpd);


        yield return new WaitForSeconds(delay);

        adSpeedItemAttribute.InitItemAttributeValue();
    }
}
