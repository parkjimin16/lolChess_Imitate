using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionHpMpController : MonoBehaviour
{
    private ChampionBase cBase;
    private int totalDamage;

    public bool IsDie()
    {
        return cBase.CurHP <= 0;
    }

    public void Init(ChampionBase championBase)
    {
        cBase = championBase;
        cBase.ChampionFrame.SetHPSlider(cBase.Display_CurHp, cBase.Display_MaxHp);
        cBase.ChampionFrame.SetManaSlider(cBase.DIsplay_CurMana, cBase.Display_CurHp);
    }

    public void AddHealth(int hp, float value)
    {
        int hpValue = (int)(hp * value);

        if(cBase.Display_CurHp + hpValue >= cBase.Display_MaxHp)
        {
            Debug.Log("�ִ� ü���Դϴ�.");
            Debug.Log($"CurHP : {cBase.CurHP} = MaxHP : {cBase.MaxHP} ");

            cBase.CurHP = cBase.MaxHP;
        }
        else
        {
            Debug.Log($"���� ü�� : {cBase.CurHP} , �߰��� ü�� : {hpValue}");
            cBase.CurHP += hpValue;
        }

        cBase.UpdateChampmionStat();
    }

    public void TakeDamage(float damage)
    {
        totalDamage = (int)(100 * (100 / (100 + cBase.Display_AD_Def)));
        totalDamage = (int)(totalDamage / (1 - cBase.Display_Total_Def));
        cBase.Display_CurHp -= totalDamage;
        DamageMana();

        if (cBase.EquipItem == null)
            return;

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
