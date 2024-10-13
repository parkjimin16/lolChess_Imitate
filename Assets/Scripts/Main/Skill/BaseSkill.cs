using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkill : MonoBehaviour
{
    public Coroutine skillCoroutine;

    /// <summary>
    /// 능력치 증가 등의 스킬
    /// </summary>
    public virtual void UseSkill() { }


    /// <summary>
    /// 발사체 발사 스킬
    /// </summary>
    /// <param name="target"></param>
    public virtual void UseSkillTarget(GameObject target) { }
}
