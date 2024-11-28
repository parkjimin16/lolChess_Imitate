using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ¹ãÀÇ ³¡ÀÚ¶ô
/// </summary>
public class EndOfNight : BaseItem
{
    private bool useSkill;
    private ItemAttribute itemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.AD_Speed)
            {
                itemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        if (targetChampion == null)
            return;

        InitItemSkill();

 
        if(!useSkill)
            CheckChampionHp();
    }

    public override void ResetItem()
    {
        useSkill = false;
        itemAttribute.InitItemAttributeValue();
    }

    

    private void CheckChampionHp()
    {

        if (EquipChampion == null || EquipChampionBase == null)
        {
            return;
        }
            

        if (EquipChampionBase.Champion_CurHp / EquipChampionBase.Champion_MaxHp <= 0.6f)
        {
            itemAttribute.SetAttributeValue(0.15f);
            CoroutineHelper.StartCoroutine(ChangeTagTemporarily(EquipChampionBase.gameObject, "CantSelectChampion", "Champion", 1f));

        }
    }

    private IEnumerator ChangeTagTemporarily(GameObject equipChampion, string temporaryTag, string originalTag, float delay)
    {
        equipChampion.tag = temporaryTag;
        
        yield return new WaitForSeconds(delay);

        equipChampion.tag = originalTag;
        useSkill = true;
    }
}