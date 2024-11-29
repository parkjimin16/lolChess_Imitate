using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionHpMpController : MonoBehaviour
{
    private ChampionBase cBase;
    private int totalDamage;

    private bool isDieCoroutineRunning;

    public bool IsDie()
    {
        return cBase.Champion_CurHp <= 0;
    }


    private void Update()
    {
        if (IsDie() && !isDieCoroutineRunning)
        {
            isDieCoroutineRunning = true;
            cBase.ChampionStateController.ChangeState(ChampionState.Die, cBase);
            HexTile hex = Manager.Stage.GetParentTileInHex(gameObject);
            hex.championOnTile.Remove(gameObject);
            //hex.championOnTile.Clear();
            StartCoroutine(WaitForAnimationToEnd());
        }
    }

    #region Init

    public void Init(ChampionBase championBase)
    {
        cBase = championBase;
        cBase.ChampionFrame.SetHPSlider(cBase.Champion_CurHp, cBase.Champion_MaxHp);
        cBase.ChampionFrame.SetManaSlider(cBase.Champion_CurHp, cBase.Champion_MaxHp);
        isDieCoroutineRunning = false;
    }
    
    public void InitBattleEnd()
    {
        StopAllCoroutines();
        isDieCoroutineRunning = false;
    }
    #endregion

    public void AddHealth(int hp, float value)
    {
        int hpValue = (int)(hp * value);
        if(cBase.Champion_CurHp + hpValue >= cBase.Champion_MaxHp)
        {
            cBase.Champion_CurHp = cBase.Champion_MaxHp;
        }
        else
        {
            cBase.Champion_CurHp += hpValue;
        }

        cBase.UpdateChampmionStat();
    }

    public void TakeDamage(float damage)
    {
        float reducedDamage = damage * (100f / (100f + cBase.Champion_AD_Def));

        if (cBase.Champion_Total_Def < 1f) 
            reducedDamage /= (1f - cBase.Champion_Total_Def);
        else
            reducedDamage = float.MaxValue; 

        // 정수로 변환하여 실제 데미지 반영
        int totalDamage = Mathf.RoundToInt(reducedDamage);
        cBase.Champion_CurHp = Mathf.Max(0, cBase.Champion_CurHp - totalDamage); 

        DamageMana();

        if (cBase.EquipItem != null && cBase.EquipItem.Count > 0)
        {
            foreach (ItemBlueprint it in cBase.EquipItem)
            {
                it.BaseItem.CheckHp(cBase.Champion_CurHp, cBase.Champion_MaxHp);
            }
        }

        if (cBase.Champion_CurHp > cBase.Champion_MaxHp)
        {
            cBase.Champion_CurHp = cBase.Champion_MaxHp;
        }

        cBase.UpdateChampmionStat();
    }




    public bool IsManaFull()
    {
        return cBase.Champion_CurMana >= cBase.Champion_MaxMana;
    }

    public void UseSkillMana()
    {
        cBase.Champion_CurMana = 0;
        cBase.UpdateChampmionStat();
    }

    public void NormalAttackMana()
    {
        cBase.Champion_CurMana += 5;
        cBase.UpdateChampmionStat();
    }

    public void ManaPlus(int mana)
    {
        if (cBase.Champion_MaxMana <= cBase.Champion_CurMana + mana)
        {
            cBase.Champion_CurMana = cBase.Champion_MaxMana;
        }
        else
        {
            cBase.Champion_CurMana += mana;
        }
    }

    public void DamageMana()
    {
        cBase.Champion_CurMana += 5;
    }



    private IEnumerator WaitForAnimationToEnd()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
