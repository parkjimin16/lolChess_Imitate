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

    public override void ResetItem()
    {
        isShieldDestroy = false;
        hasShieldActivated = false;
        shieldAmount = 0;
    }

    /// <summary>
    /// 공격 받을때마다 호출
    /// </summary>
    /// <param name="curHp"></param>
    /// <param name="maxHp"></param>
    public override void CheckHp(int curHp, int maxHp)
    {
        ActivateShield(curHp, maxHp);
    }

    private void ActivateShield(int currentHealth, int maxHealth)
    {
        if (currentHealth / maxHealth > 0.4f || hasShieldActivated)
        {
            return;
        }
 
        shieldAmount = Mathf.RoundToInt(maxHealth * 0.25f);
        hasShieldActivated = true;

        CoroutineHelper.StartCoroutine(ShieldDurationCoroutine());
    }

    private IEnumerator ShieldDurationCoroutine()
    {
        EquipChampionBase.SetShield(shieldAmount);

        Debug.Log("Shield Create");
        yield return new WaitForSeconds(shieldDuration);

        Debug.Log("Shield Destroy");

        shieldAmount = 0;
        EquipChampionBase.SetShield(shieldAmount);
        isShieldDestroy = true;
    }

    public float GetShieldAmount()
    {
        return shieldAmount;
    }
}
