using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionAttackController : MonoBehaviour
{
    #region 변수 & 프로퍼티
    private ChampionBase cBase;
    private IEnumerator attackCoroutine;

    [SerializeField] private GameObject targetChampion;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private bool isUseSkill;

    private float attack_Speed;
    private float attack_Range;

    public float attackRange;


    public bool IsAttack; // 전체 체크 용
    [SerializeField] private bool attackLogic;

    public bool IsUseSkill()
    {
        return isUseSkill;
    }

    

    public GameObject TargetChampion => targetChampion;

    #endregion

    #region Unity Flow
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

    #endregion


    #region 초기화
    public void Init(ChampionBase championBase, float _atk_Speed, float _atk_Range, float _curMana, float _maxMana)
    {
        cBase = championBase;
        attack_Speed = _atk_Speed;
        attack_Range = _atk_Range;

        IsAttack = false;
        attackLogic = false;
        isUseSkill = false;
    }

    #endregion

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
        if (attackLogic)
            yield break;

        attackLogic = true;

        targetChampion = FindTargetInRange();

        if (targetChampion == null)
        {
            Debug.Log("사거리 내에 없습니다.");
            attackLogic = false;
            yield break;
        }

        while (targetChampion != null)
        {
            Vector3 directionToTarget = (targetChampion.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

            while (Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
                yield return null;
            }

            cBase.UpdateStatWithItem(cBase.EquipItem);


            if (cBase.ChampionHpMpController.IsManaFull() && !isUseSkill)
            {
                CoroutineHelper.StartCoroutine(UseSkillCoroutine());
            }
            else if(!cBase.ChampionHpMpController.IsManaFull() && !isUseSkill)
            {
                CreateNormalAttack(targetChampion);
                cBase.ChampionHpMpController.NormalAttackMana();
            }




            Debug.Log($"{cBase.ChampionName} : 공격 {cBase.Champion_Atk_Spd}");

            yield return new WaitForSeconds(cBase.Champion_Atk_Spd);
            //yield return new WaitForSeconds(0.5f);

           
        }

        attackLogic = false;
    }

    private GameObject FindTargetInRange()
    {
        attackRange = cBase.Attack_Range * 2.75f;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

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
        if (target == null)
            return;

        if (cBase.Attack_Range > 1)
        {
            // 원거리 공격 - 발사체 생성
            GameObject projectileObject = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            NormalProjectile projectile = projectileObject.GetComponent<NormalProjectile>();

            if (projectile != null)
            {
                projectile.SetTarget(target, ChampionDamageSet());
            }
        }
        else 
        {
            // 근접 공격
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget <= 1.5f)
            {
                ChampionBase targetHealth = target.GetComponent<ChampionBase>();
                if (targetHealth != null)
                {
                    PlayMeleeAttackAnimation();
                    targetHealth.ChampionHpMpController.TakeDamage(ChampionDamageSet()); 
                }
            }
        }
    }
    private void PlayMeleeAttackAnimation()
    {
        // 근접 공격 애니메이션 재생 로직 (예: animator.SetTrigger("MeleeAttack"))
    }


    private int ChampionDamageSet()
    {
        return cBase.Champion_TotalDamage;
    }


    private IEnumerator UseSkillCoroutine()
    {
        Debug.Log(" 스킬 사용 ");
        isUseSkill = true;
        cBase.ChampionHpMpController.UseSkillMana();

        yield return new WaitForSeconds(3.0f);

        Debug.Log(" 스킬 끝 ");
        isUseSkill = false;
    }
}
