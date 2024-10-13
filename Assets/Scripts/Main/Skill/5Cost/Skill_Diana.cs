using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Diana : BaseSkill
{

    /// <summary>
    /// 2칸 내 가장 많은 적을 타격할 수 있는 위치로 돌진하고 체력을 회복합니다. 초과 체력 회복량은 3초 동안 지속되는 보호막으로 전환됩니다. 이후 주변 적에게 마법 피해를 입히고 2칸 멀리 있는 적에게 마법 피해를 입힙니다.
    /// 스킬을 2회 사용할 때마다 폭설을 내려 모든 적이 동상에 걸리게 하고 3초에 걸쳐 모든 아군의 체력을 회복시킵니다.또한 초과 체력 회복량은 3초 동안 지속되는 보호막으로 전환됩니다.
    /// 동상: 공격 속도 20% 감소
    /// </summary>
    public override void UseSkill()
    {
        base.UseSkill();
    }
}
