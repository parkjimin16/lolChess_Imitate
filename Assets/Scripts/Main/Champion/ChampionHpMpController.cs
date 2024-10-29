using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionHpMpController : MonoBehaviour
{
    private ChampionBase cBase;
    private int totalDamage;

    public bool IsDie()
    {
        return cBase.Champion_CurHp <= 0;
    }

    public void Init(ChampionBase championBase)
    {
        cBase = championBase;
        cBase.ChampionFrame.SetHPSlider(cBase.Champion_CurHp, cBase.Champion_MaxHp);
        cBase.ChampionFrame.SetManaSlider(cBase.Champion_CurHp, cBase.Champion_MaxHp);
    }

    public void AddHealth(int hp, float value)
    {
        int hpValue = (int)(hp * value);
        Debug.Log("HP value" + hpValue);
        if(cBase.Champion_CurHp + hpValue >= cBase.Champion_MaxHp)
        {
            Debug.Log($"최대 체력입니다 => CurHP : {cBase.Champion_CurHp} = MaxHP : {cBase.Champion_MaxHp} ");
            cBase.Champion_CurHp = cBase.Champion_MaxHp;
        }
        else
        {
            Debug.Log($"현재 체력 : {cBase.Champion_CurHp} , 추가될 체력 : {hpValue}");
            cBase.Champion_CurHp += hpValue;
            Debug.Log($"추가된 체력 : {cBase.Champion_CurHp}");
        }

        cBase.UpdateChampmionStat();
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"들어온 데미지 : {damage}");

        totalDamage = (int)(100 * (100 / (100 + cBase.Champion_AD_Def)));
        totalDamage = (int)(totalDamage / (1 - cBase.Champion_Total_Def));

        Debug.Log($"가할 데미지 : {totalDamage}");

        cBase.Champion_CurHp -= totalDamage;
        DamageMana();

        if (cBase.EquipItem == null)
            return;

        foreach(ItemBlueprint it in cBase.EquipItem)
        {
            it.BaseItem.CheckHp(cBase.Champion_CurHp, cBase.Champion_MaxHp);
        }
    }

    

    public bool IsManaFull()
    {
        return cBase.Champion_CurMana >= cBase.Champion_MaxMana;
    }

    public void UseSkillMana()
    {
        cBase.Champion_CurMana = 0;
    }

    public void NormalAttackMana()
    {
        cBase.Champion_CurMana += 5;
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
}
