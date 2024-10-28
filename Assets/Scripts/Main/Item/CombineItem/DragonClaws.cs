using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonClaws : BaseItem
{
    private Coroutine coroutineInstance;

    private int originalMaxHp;
    private int tempMaxHp;
    private float healPercent;
    private int healAmount;

    private bool isStart;

    private ItemAttribute hpItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        healPercent = 0.025f;
        isStart = false;
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.HP)
            {
                hpItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }

        if (EquipChampionBase == null)
            return;


        tempMaxHp = (int)(EquipChampionBase.Display_MaxHp * 0.09f);
        hpItemAttribute.SetAttributeValue(tempMaxHp);
    }

    public override void InitTargetObject(GameObject targetChampion)
    {
        isStart = true;

        if (EquipChampionBase == null)
            return;


        if (coroutineInstance == null)
            coroutineInstance = CoroutineHelper.StartCoroutine(HealCoroutine());
    }

    private IEnumerator HealCoroutine()
    {
        while (true)
        {
            Debug.Log("Heal");
            healAmount = (int)(EquipChampionBase.Display_MaxHp * healPercent);
            EquipChampionBase.ChampionHpMpController.AddHealth(healAmount, 1.0f);

            yield return new WaitForSeconds(2);

            Debug.Log("Wait");
            if (!isStart)
            {
                Debug.Log("코루틴 끝");
                coroutineInstance = null; 
                yield break;
            }

        }
    }

    public override void ResetItem()
    {
        healPercent = 0.025f;
        isStart = false;
        coroutineInstance = null;

        hpItemAttribute.InitItemAttributeValue();
    }
}
