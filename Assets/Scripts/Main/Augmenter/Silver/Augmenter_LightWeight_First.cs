using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_LightWeight_First : BaseAugmenter
{
    // 로직 변수

    private ChampionCost cost1 = ChampionCost.OneCost;
    private ChampionCost cost2 = ChampionCost.TwoCost;

    private float speed;
    private float atk_Speed;

    #region 초기화

    private void InitValue()
    {
        speed = 0.15f;
        atk_Speed = 0.15f;
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
            if (cBase.ChampionCost == cost1 || cBase.ChampionCost == cost2)
            {
                cBase.Augmenter_Atk_Spd += atk_Speed;
                cBase.Augmenter_Speed += speed;
                cBase.UpdateChampmionStat();
            }
        }
    }

    public override void ApplyEndRound(UserData user)
    {
        InitValue();

        var list = GetUserChampions(user);

        foreach (var cBase in list)
        {
            if (cBase.ChampionCost == cost1 || cBase.ChampionCost == cost2)
            {
                cBase.InitAugmenterStat();
                cBase.UpdateChampmionStat();
            }
        }
    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
