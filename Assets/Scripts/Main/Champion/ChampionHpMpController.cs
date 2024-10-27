using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionHpMpController : MonoBehaviour
{
    private ChampionBase cBase;

    public bool IsDie()
    {
        return cBase.CurHP <= 0;
    }

    public void Init(ChampionBase championBase)
    {
        cBase = championBase;
        cBase.ChampionFrame.SetHPSlider(cBase.CurHP, cBase.MaxHP);
        cBase.ChampionFrame.SetManaSlider(cBase.CurMana, cBase.MaxMana);
    }

    public void AddHealth(int hp)
    {
        if(cBase.CurHP + hp >= cBase.MaxHP)
        {
            cBase.CurHP = cBase.MaxHP;
        }
        else
        {
            cBase.CurHP += hp;
        }
        
    }

    public void TakeDamage(float damage)
    {
        cBase.CurHP -= (int)damage;
        DamageMana();

        foreach(ItemBlueprint it in cBase.EquipItem)
        {
            it.BaseItem.CheckHp(cBase.Display_CurHp, cBase.Display_MaxHp);
        }
    }

    

    public bool IsManaFull()
    {
        return cBase.CurMana >= cBase.MaxMana;
    }

    public void UseSkillMana()
    {
        cBase.CurMana = 0;
    }

    public void NormalAttackMana()
    {
        cBase.CurMana += 5;
    }

    public void ManaPlus(int mana)
    {
        if (cBase.MaxMana <= cBase.CurMana + mana)
        {
            cBase.CurMana = cBase.MaxMana;
        }
        else
        {
            cBase.CurMana += mana;
        }
    }

    public void DamageMana()
    {
        cBase.CurMana += 5;
    }
}
