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

    public bool IsAttack; // 전체 체크 용
    private bool attackLogic;

    private void Update()
    {
        if(FindTargetInRange() == null)
        {
            IsAttack = false;
        }
        else if(FindTargetInRange() != null)
        {
            IsAttack = true;
        }
    }


    public void Init(ChampionBase championBase, float _atk_Speed, float _atk_Range, float _curMana, float _maxMana)
    {
        cBase = championBase;
        attack_Speed = _atk_Speed;
        attack_Range = _atk_Range;

        IsAttack = false;
        attackLogic = false;
    }

    public void AttackLogic()
    {
        if(!attackLogic)
        {
            attackCoroutine = AttackRoutine();
            StartCoroutine(attackCoroutine);
        }

    }

    public void AttackLogicStop()
    {
        attackCoroutine = null;
    }

    private IEnumerator AttackRoutine()
    {
        attackLogic = true;

        GameObject target = FindTargetInRange();

        if (target == null)
        {
            Debug.Log("사거리 내에 없습니다.");
            attackLogic = false;
            yield break;
        }

        while (target != null)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

            while (Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
                yield return null;
            }


            if (cBase.ChampionHpMpController.IsManaFull())
            {
                UseSkill(target);
                cBase.ChampionHpMpController.UseSkillMana();
            }
            else
            {
                CreateNormalAttack(target);
            }

            cBase.ChampionHpMpController.NormalAttackMana();


            //yield return new WaitForSeconds(cBase.Attack_Speed);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private GameObject FindTargetInRange()
    {
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
        Debug.Log("스킬 사용");
    }
}
