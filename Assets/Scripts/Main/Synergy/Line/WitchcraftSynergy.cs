using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchcraftSynergy : SynergyBase
{

    // 시너지 변수
    private int level;

    // 마녀 로직 변수
    private int healthReduction;
    private bool decreaseHealth;
    private bool applyGreenColor;
    private float magicDamagePercent;
    private float additionalWitchDamage;
    private bool applyFrogTransformation;
    private float amplifyEffects;


    public WitchcraftSynergy()
    : base("마녀", ChampionLine.Witchcraft, ChampionJob.None, 0) { }

    #region 활성 & 비활성화
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

        //Debug.Log($"[마녀] 레벨 {level} 저주 효과 적용");
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

        //Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
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


    #region 마녀 로직
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
                // 2레벨: 체력 120 감소
                if (healthReduction > 0)
                {
                    cBase.ChampionHpMpController.TakeDamage(healthReduction);
                }

                // 4레벨: 최대 체력의 6%만큼 마법 피해
                if (magicDamagePercent > 0)
                {
                    float damage = cBase.Synergy_MaxHP * magicDamagePercent;
                    cBase.ChampionHpMpController.TakeDamage(healthReduction);
                }

                // 6레벨: 마녀로부터 25%의 추가 고정 피해
                if (additionalWitchDamage > 0)
                {
                    float fixedDamage = additionalWitchDamage * cBase.Champion_TotalDamage;
                    cBase.ChampionHpMpController.TakeDamage(fixedDamage);
                }

                // 8레벨: 기절 효과 적용
                if (applyFrogTransformation)
                {
                    //cBase.Stun(2f); // 2초간 기절
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
