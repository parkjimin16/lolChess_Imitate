using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionAttackController : MonoBehaviour
{
    private ChampionBase cBase;

    private IEnumerator attackCoroutine;
    private float attack_Speed;
    private float attack_Range;
    private float curMana;
    private float maxMana;

    public bool IsAttack;

    public bool IsManaFull()
    {
        return curMana >= maxMana;
    }

    public void Init(ChampionBase championBase, float _atk_Speed, float _atk_Range, float _curMana, float _maxMana)
    {
        cBase = championBase;
        attack_Speed = _atk_Speed;
        attack_Range = _atk_Range;
        curMana = _curMana;
        maxMana = _maxMana;

        IsAttack = false;
        attackCoroutine = AttackRoutine();
    }

    public void AttackLogic()
    {
        StartCoroutine(attackCoroutine);
    }

    private IEnumerator AttackRoutine()
    {
        IsAttack = true;

        GameObject target = FindTargetInRange();

        if (target == null)
        {
            Debug.Log("사거리 내에 없습니다.");
            yield break;
        }

        ChampionBase targetHealth = target.GetComponent<ChampionBase>();

        while (targetHealth != null && !targetHealth.ChampionHealthController.IsDie())
        {
            if (IsManaFull())
            {
                UseSkill(target);
            }
            else
            {
                CreateNormalAttack(target);
            }

            yield return new WaitForSeconds(cBase.Attack_Speed);
        }

        IsAttack = false;
    }

    private GameObject FindTargetInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, cBase.Attack_Range);

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                return collider.gameObject;
            }
        }

        return null;
    }

    public void CreateNormalAttack(GameObject target)
    {
        ChampionBase targetHealth = target.GetComponent<ChampionBase>();

        if (targetHealth.ChampionHealthController != null)
        {
            targetHealth.ChampionHealthController.TakeDamage(10);
        }
    }

    public void UseSkill(GameObject target)
    {

    }
}
