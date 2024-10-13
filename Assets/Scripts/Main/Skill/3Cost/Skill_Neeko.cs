using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Neeko : BaseSkill
{
    /// <summary>
    /// 2초 동안 바위 게로 변신합니다. 지속시간 동안 체력을 12% + 200 회복하고 주변 아군 3명과 주변 적 3명에게 바위 게 광선을 방출합니다. 광선에 맞은 아군은 체력을 회복하고, 광선에 맞은 적은 마법 피해를 입고 1.25초 동안 기절합니다.
    /// </summary>
    public override void UseSkill()
    {
        base.UseSkill();
    }
}
