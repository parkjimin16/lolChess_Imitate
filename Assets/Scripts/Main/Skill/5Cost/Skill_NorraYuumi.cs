using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_NorraYuumi : BaseSkill
{

    /// <summary>
    /// 현재 대상에게 공을 발사합니다. 공은 폭발해 마법 피해를 입히고, 5초 동안 해당 지역을 색칠해 지속시간 동안 마법 피해를 입힙니다. 이미 색이 칠해진 지역에 공이 떨어진다면, 공은 2칸 내 새로운 위치로 튕긴 후 폭발합니다.
    /// </summary>
    public override void UseSkill()
    {
        base.UseSkill();
    }
}
