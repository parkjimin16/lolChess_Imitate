using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcanaSynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 아르카나 로직 변수

    public ArcanaSynergy()
        : base("아르카나", ChampionLine.Arcana, ChampionJob.None, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {

            Deactivate(user);
            return;
        }
        else if (level == 2)
        {

        }
        else if (level == 3)
        {

        }
        else if (level == 4)
        {

        }
        else if (level == 5)
        {

        }

        Debug.Log($"[아르카나] 레벨 {level} 적용");
    }

    protected override void RemoveEffects(UserData user)
    {

        Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 3)
            return;


    }


    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion
}
