using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_GiantAndPowerful : BaseAugmenter
{
    // 로직 변수
    private int maxHp;
    private float hpRatio;
    private int plusHp;

    #region 초기화
    private void InitValue()
    {
        maxHp = 300;
        hpRatio = 0.04f;
        plusHp = 0;
    }
    #endregion

    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        InitValue();
    }

    public override void ApplyStartRound(UserData user)
    {
        var list = GetUserChampions(user);

        foreach (var cBase in list)
        {
            cBase.Augmenter_MaxHP += maxHp;
            cBase.UpdateChampmionStat();
            cBase.ChampionHpMpController.AddHealth(maxHp, 1.0f);
            
            plusHp = (int)(cBase.Champion_MaxHp * hpRatio);
            cBase.Augmenter_MaxHP += plusHp;
            cBase.UpdateChampmionStat();
            cBase.ChampionHpMpController.AddHealth(plusHp, 1.0f);

            cBase.UpdateChampmionStat();

        }
    }

    public override void ApplyEndRound(UserData user)
    {
        InitValue();

        var list = GetUserChampions(user);

        foreach (var cBase in list)
        {
            cBase.InitAugmenterStat();
            cBase.UpdateChampmionStat();
            cBase.ResetHealth();
        }
    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
