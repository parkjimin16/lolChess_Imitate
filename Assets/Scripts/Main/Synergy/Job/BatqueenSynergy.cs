using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatqueenSynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 박쥐 여왕 로직 변수


    public BatqueenSynergy()
        : base("박쥐 여왕", ChampionLine.None, ChampionJob.Batqueen, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 1)
        {
            Deactivate(user);
        }
        else if (level >= 1)
        {

        }

        Debug.Log($"[박쥐 여왕] 레벨 {level} 효과 적용");
    }

    protected override void RemoveEffects(UserData user)
    {
        Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 1)
            return;
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion
}
