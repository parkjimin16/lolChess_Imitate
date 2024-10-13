using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Nomsy : BaseSkill
{
    /// <summary>
    /// 대상에게 불꽃 재채기를 내뿜어 물리 피해를 입힙니다. 대상 처치 후 남는 피해량의 50%에 해당하는 피해를 가장 가까운 2명의 대상에게 입힙니다.
    /// 용 업그레이드: 대상 처치 후 남는 피해량이 4명의 대상에게 적용됩니다.
    /// </summary>
    public override void UseSkill()
    {
        base.UseSkill();
    }
}
