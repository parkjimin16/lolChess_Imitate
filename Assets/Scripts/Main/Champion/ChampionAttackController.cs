using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionAttackController : MonoBehaviour
{
    private ChampionBase cBase;
    private IEnumerator attackCoroutine;
    
    [SerializeField] 
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform shootPoint;

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

        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

        while (Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
            yield return null; // 다음 프레임까지 대기
        }

        ChampionBase targetHealth = target.GetComponent<ChampionBase>();

        CreateNormalAttack(target);

        /*
        while (targetHealth != null && !targetHealth.ChampionHealthController.IsDie())
        {
            if (IsManaFull())
            {
                Debug.Log("Mana");
                UseSkill(target);
            }
            else
            {
                Debug.Log("Mana Non");
                CreateNormalAttack(target);
            }

            yield return new WaitForSeconds(cBase.Attack_Speed);
        }
        */

        IsAttack = false;
    }

    private GameObject FindTargetInRange()
    {
        Debug.Log("Find Target");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 30);  //cBase.Attack_Range);

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
        Debug.Log("Normal Attack");

        GameObject projectileObject = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        NormalProjectile projectile = projectileObject.GetComponent<NormalProjectile>();

        if (projectile != null)
        {
            projectile.SetTarget(target, 10); 
        }
    }

    public void UseSkill(GameObject target)
    {

    }
}
