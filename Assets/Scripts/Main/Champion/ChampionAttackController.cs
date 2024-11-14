using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionAttackController : MonoBehaviour
{
    #region ���� & ������Ƽ
    private ChampionBase cBase;

    // State : Move
    private IEnumerator findCoroutine;
    private IEnumerator moveCoroutine;

    // State : Attack
    private IEnumerator attackCoroutine;

    [SerializeField] private GameObject targetChampion;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private bool isUseSkill;

    private float attack_Speed;
    private float attack_Range;

    public float realAttackRange;
    public Player EnemyPlayer;






    [SerializeField] List<HexTile> path = new List<HexTile>();

    public bool IsAttack; // ��ü üũ ��

    [SerializeField] private bool attackLogic;

    public bool IsUseSkill()
    {
        return isUseSkill;
    }

    public bool CanAttack(GameObject target)
    {
        if(target == null) return false;

        return Vector3.Distance(gameObject.transform.position, target.transform.position) <= realAttackRange;
    }

    public GameObject TargetChampion => targetChampion;

    #endregion

    #region �ʱ�ȭ
    public void Init(ChampionBase championBase, float _atk_Speed, float _atk_Range, float _curMana, float _maxMana)
    {
        cBase = championBase;
        attack_Speed = _atk_Speed;
        attack_Range = _atk_Range;

        realAttackRange = cBase.Attack_Range * 2.75f;

        IsAttack = false;
        attackLogic = false;
        isUseSkill = false;

    }

    #endregion

    #region Ž�� ����

    public void FindPathToTarget()
    {
        SetTargetEnemy();
    }

    private void SetTargetEnemy()
    {
        if (EnemyPlayer == null || EnemyPlayer.UserData.BattleChampionObject.Count == 0)
            return;

        GameObject closestChampion = null;
        float minDistance = float.MaxValue;

        Vector3 currentPosition = transform.position;

        foreach (var champion in EnemyPlayer.UserData.BattleChampionObject)
        {
            if (champion == null)
                continue;

            float distance = Vector3.Distance(currentPosition, champion.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestChampion = champion;
            }
        }
        targetChampion = closestChampion;
        Manager.Stage.SetNearestTile(gameObject);

        path = Manager.Stage.FindShortestPath(gameObject, targetChampion);

        HexTile prevTile = Manager.Stage.FindNearestTile(gameObject, cBase.BattleStageIndex);
        //StartCoroutine(MoveOneStepAlongPath(prevTile));
        StartCoroutine(StartMoveAndCheck());
    }


    #endregion

    #region �̵� ����


    private IEnumerator StartMoveAndCheck()
    {
        ChampionBase tcBase = targetChampion.GetComponent<ChampionBase>();

        while (targetChampion != null && path.Count > 0 && !tcBase.ChampionHpMpController.IsDie() && MergeScene.BatteStart)
        {
            HexTile prevTile = Manager.Stage.FindNearestTile(gameObject, cBase.BattleStageIndex);
            yield return StartCoroutine(MoveOneStepAlongPath(prevTile));


            // ���� ���� ���� ���� �ִٸ�
            if (CanAttack(targetChampion))
            {
                cBase.ChampionStateController.ChangeState(ChampionState.Attack, cBase);
                path.Clear();
                yield break;
            }


            // �̵� �� �ִ� ��� �ٽ� ���
            path = Manager.Stage.FindShortestPath(gameObject, targetChampion);
        }
    }

    private IEnumerator MoveOneStepAlongPath(HexTile prevTile)
    {
        if (path == null || path.Count == 0)
            yield break;

        HexTile nextTile = path[0];

        Vector3 targetPosition = nextTile.transform.position;
        yield return StartCoroutine(MoveTo(targetPosition));

        float stoppingDistance = 0.1f; 
        if (Vector3.Distance(transform.position, targetPosition) <= stoppingDistance)
        {
            prevTile.championOnTile.Remove(gameObject);
            nextTile.championOnTile.Add(gameObject);
            gameObject.transform.SetParent(nextTile.transform);

            path.RemoveAt(0);
        }
    }

    private IEnumerator MoveTo(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, cBase.Champion_Speed  * 5f * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        
    }
    #endregion

    #region ���� ����

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

        if (CanAttack(targetChampion))
        {
            Debug.Log("��Ÿ� ���� �����ϴ�.");
            attackLogic = false;
            SetTargetEnemy();
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

            yield return new WaitForSeconds(cBase.Champion_Atk_Spd);
        }

        attackLogic = false;
    }

    public void CreateNormalAttack(GameObject target)
    {
        if (target == null)
            return;

        if (cBase.Attack_Range > 1)
        {
            // ���Ÿ� ���� - �߻�ü ����
            GameObject projectileObject = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            NormalProjectile projectile = projectileObject.GetComponent<NormalProjectile>();

            if (projectile != null)
            {
                projectile.SetTarget(target, ChampionDamageSet());
            }
        }
        else 
        {
            // ���� ����
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
        // ���� ���� �ִϸ��̼� ��� ���� (��: animator.SetTrigger("MeleeAttack"))
    }

    private int ChampionDamageSet()
    {
        return cBase.Champion_TotalDamage;
    }

    private IEnumerator UseSkillCoroutine()
    {
        Debug.Log(" ��ų ��� ");
        isUseSkill = true;
        cBase.ChampionHpMpController.UseSkillMana();

        yield return new WaitForSeconds(3.0f);

        Debug.Log(" ��ų �� ");
        isUseSkill = false;
    }
    #endregion

}
