using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchcraftSynergy : SynergyBase
{

    // �ó��� ����
    private int level;

    // ���� ���� ����
    private int healthReduction;
    private bool decreaseHealth;
    private bool applyGreenColor;
    private float magicDamagePercent;
    private float additionalWitchDamage;
    private bool applyFrogTransformation;
    private float amplifyEffects;


    public WitchcraftSynergy()
    : base("����", ChampionLine.Witchcraft, ChampionJob.None, 0) { }

    #region Ȱ�� & ��Ȱ��ȭ
    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if(level >= 0 && level < 2)
        {
            Deactivate(user);
        }
        else if(level >= 2 && level < 4)
        {
            healthReduction = 120;
            decreaseHealth = true;
            applyGreenColor = false;
            magicDamagePercent = 0;
            additionalWitchDamage = 0;
            applyFrogTransformation = false;
            amplifyEffects = 0;
        }
        else if (level >= 4 && level < 6)
        {
            healthReduction = 120;
            decreaseHealth = true;
            applyGreenColor = true;
            magicDamagePercent = 0.06f;
            additionalWitchDamage = 0;
            applyFrogTransformation = false;
            amplifyEffects = 0;
        }
        else if (level >= 6 && level < 8)
        {
            healthReduction = 120;
            decreaseHealth = true;
            applyGreenColor = true;
            magicDamagePercent = 0.06f;
            additionalWitchDamage = 0.25f;
            applyFrogTransformation = true;
            amplifyEffects = 0;
        }
        else if (level >= 8)
        {
            healthReduction = 120;
            decreaseHealth = true;
            applyGreenColor = true;
            magicDamagePercent = 0.06f;
            additionalWitchDamage = 0.25f;
            applyFrogTransformation = true;
            amplifyEffects = 0.5f;
        }

        //Debug.Log($"[����] ���� {level} ���� ȿ�� ����");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            cBase.InitSynergyStat();

            cBase.UpdateChampmionStat();
        }

        //Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    public override void Activate(UserData user)
    {
        CastCurseOnEnemy(level, healthReduction, decreaseHealth, applyGreenColor, magicDamagePercent, additionalWitchDamage, applyFrogTransformation, amplifyEffects);
    }

    public override void Deactivate(UserData user)
    {
        level = 0;
        healthReduction = 0;
        decreaseHealth = false;
        applyGreenColor = false;
        magicDamagePercent = 0;
        additionalWitchDamage = 0;
        applyFrogTransformation = false;
        amplifyEffects = 0;

        RemoveEffects(user);
    }

    #endregion


    #region ���� ����
    private void ApplyCurseEffect(int healthReduction, bool decreaseHealth = false, bool applyGreenColor = false,
                                  float magicDamagePercent = 0, float additionalWitchDamage = 0,
                                  bool applyFrogTransformation = false, float amplifyEffects = 0)
    {
        List<GameObject> enemies = Manager.User.GetHumanUserData().BattleChampionObject;

        foreach (var enemy in enemies)
        {
            ChampionBase cBase = enemy.GetComponent<ChampionBase>();
            if (cBase != null)
            {
                // 2����: ü�� 120 ����
                if (healthReduction > 0)
                {
                    cBase.ChampionHpMpController.TakeDamage(healthReduction);
                }

                // 4����: �ִ� ü���� 6%��ŭ ���� ����
                if (magicDamagePercent > 0)
                {
                    float damage = cBase.Synergy_MaxHP * magicDamagePercent;
                    cBase.ChampionHpMpController.TakeDamage(healthReduction);
                }

                // 6����: ����κ��� 25%�� �߰� ���� ����
                if (additionalWitchDamage > 0)
                {
                    float fixedDamage = additionalWitchDamage * cBase.Champion_TotalDamage;
                    cBase.ChampionHpMpController.TakeDamage(fixedDamage);
                }

                // 8����: ���� ȿ�� ����
                if (applyFrogTransformation)
                {
                    //cBase.Stun(2f); // 2�ʰ� ����
                }

                if (amplifyEffects > 0)
                {
                    float amplifiedDamage = (healthReduction + (cBase.Synergy_MaxHP * magicDamagePercent)) * amplifyEffects;
                    cBase.ChampionHpMpController.TakeDamage(healthReduction);
                }


                cBase.UpdateChampmionStat();
            }
        }
    }

    private void CastCurseOnEnemy(int level, int healthReduction, bool decreaseHealth, bool applyGreenColor,
                                  float magicDamagePercent, float additionalWitchDamage,
                                  bool applyFrogTransformation, float amplifyEffects)
    { 
        if (level >= 2 && level < 4)
        {
            ApplyCurseEffect(healthReduction, decreaseHealth, applyGreenColor, magicDamagePercent, additionalWitchDamage, applyFrogTransformation, amplifyEffects);
        }
        else if (level >= 4 && level < 6)
        {
            ApplyCurseEffect(healthReduction, decreaseHealth, applyGreenColor, magicDamagePercent, additionalWitchDamage, applyFrogTransformation, amplifyEffects);
        }
        else if (level >= 6 && level < 8)
        {
            ApplyCurseEffect(healthReduction, decreaseHealth, applyGreenColor, magicDamagePercent, additionalWitchDamage, applyFrogTransformation, amplifyEffects);
        }
        else if (level >= 8)
        {
            ApplyCurseEffect(healthReduction, decreaseHealth, applyGreenColor, magicDamagePercent, additionalWitchDamage, applyFrogTransformation, amplifyEffects);

        }
    }
    #endregion
}
