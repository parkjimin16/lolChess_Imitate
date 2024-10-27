using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 피바라기
/// </summary>
public class BloodFlower : BaseItem
{
    private bool isShieldDestroy;
    private bool hasShieldActivated;
    private float shieldDuration = 5f; 
    private int shieldAmount; 

    public bool IsShieldDestroy
    {
        get { return isShieldDestroy; }
        set { isShieldDestroy = value; }
    }

    public override void InitItemSkill()
    {
        isShieldDestroy = false;
        hasShieldActivated = false;
        shieldAmount = 0;
    }

    public override void CheckHp(int curHp, int maxHp)
    {
        ActivateShield(curHp, maxHp);
    }
    public void ActivateShield(int currentHealth, int maxHealth)
    {
        float curHp = currentHealth;
        float maxHp = maxHealth;

        Debug.Log($"체력 % : {curHp / maxHp}");

        if (curHp / maxHp > 0.4f || hasShieldActivated)
        {
            Debug.Log("Return");
            return;
        }
   

        


        shieldAmount = Mathf.RoundToInt(maxHp * 0.25f);
        hasShieldActivated = true;

        CoroutineHelper.StartCoroutine(ShieldDurationCoroutine());
    }

    private IEnumerator ShieldDurationCoroutine()
    {
        EquipChampionBase.Display_Shield = shieldAmount;

        Debug.Log("Shield Create");
        yield return new WaitForSeconds(shieldDuration);

        Debug.Log("Shield Destroy");

        shieldAmount = 0;
        EquipChampionBase.Display_Shield = shieldAmount;
        isShieldDestroy = true;
    }

    public float GetShieldAmount()
    {
        return shieldAmount;
    }
}
