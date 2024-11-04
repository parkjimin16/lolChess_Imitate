using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchcraftSynergy : SynergyBase
{
    public WitchcraftSynergy()
    : base("����", ChampionLine.Witchcraft, ChampionJob.None, 0) { }


    protected override void ApplyEffects(UserData user, int level)
    {
        if(level >= 0 && level < 2)
        {
            level = 0;
            Deactivate(user);
        }
        else if(level >= 2 && level < 4)
        {
            level = 2;
        }
        else if (level >= 4 && level < 6)
        {
            level = 4;
        }
        else if (level >= 6 && level < 8)
        {
            level = 6;
        }
        else if (level >= 8)
        {
            level = 8;
        }



        switch (level)
        {
            case 2:
                ApplyCurseEffect(120, decreaseHealth: true);
                break;
            case 4:
                ApplyCurseEffect(0, applyGreenColor: true, magicDamagePercent: 0.06f);
                break;
            case 6:
                ApplyCurseEffect(0, additionalWitchDamage: 0.25f);
                break;
            case 8:
                ApplyCurseEffect(0, applyFrogTransformation: true, amplifyEffects: 0.5f);
                break;
        }

        Debug.Log($"[����] ���� {level} ���� ȿ�� ����");
    }

    protected override void RemoveEffects(UserData user)
    {
        throw new System.NotImplementedException();
    }

    public override void Activate(UserData user)
    {
        // ���� ��ų�� ���� ������ ���ָ� �Ŵ� ����
        CastCurseOnEnemy();
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
        Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    private void ApplyCurseEffect(int healthReduction, bool decreaseHealth = false, bool applyGreenColor = false,
                                  float magicDamagePercent = 0, float additionalWitchDamage = 0,
                                  bool applyFrogTransformation = false, float amplifyEffects = 0)
    {
        // ���� ȿ�� ���� ����
    }

    private void CastCurseOnEnemy()
    {
        // ���� �ߵ� ���� ����
    }
}
