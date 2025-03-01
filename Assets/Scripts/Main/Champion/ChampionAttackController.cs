using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChampionAttackController : MonoBehaviour
{
    #region 변수 & 프로퍼티
    private ChampionBase cBase;

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

    [SerializeField] private HexTile nextTile;
    [SerializeField] private HexTile curTile;
    [SerializeField] List<HexTile> path;

    public bool IsAttack; // 전체 체크 용

    [SerializeField] private bool attackLogic;

    public GameObject TargetChampion 
    {
        get { return targetChampion; }
        set { targetChampion = value; }
    }
    
    public bool IsUseSkill()
    {
        return isUseSkill;
    }

    public bool CanAttack(GameObject target)
    {
        if(target == null) return false;

        return Vector3.Distance(gameObject.transform.position, target.transform.position) <= realAttackRange;
    }

    public List<HexTile> Path
    {
        get { return path; }
        set { path = value; }
    }

    #endregion

    #region 초기화
    public void Init(ChampionBase championBase, float _atk_Speed, float _atk_Range, float _curMana, float _maxMana)
    {
        cBase = championBase;
        attack_Speed = _atk_Speed;
        attack_Range = _atk_Range;

        realAttackRange = cBase.Attack_Range * 3f;

        IsAttack = false;
        attackLogic = false;
        isUseSkill = false;
    }

    public void EndBattle()
    {
        StopAllCoroutines();
        cBase.ChampionHpMpController.InitBattleEnd();

        attackCoroutine = null;

        EnemyPlayer = null;
        targetChampion = null;
        attackLogic = false;

        path.Clear();

        cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
    }
    #endregion

    #region Unity Flow

    private void Update()
    {
        if (Manager.Stage.isCripRound)
        {
            if(!Manager.Stage.IsBattleOngoing)
                cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);

            if (targetChampion == null && Manager.Stage.IsBattleOngoing &&
           cBase.Player.UserData.CripObjectList.Count != 0 && cBase.Player.UserData.BattleChampionObject.Contains(this.gameObject))
            {
                targetChampion = null;
                AttackLogicStop();
                StopAllCoroutines();
                SetTargetEnemy();
            }
            else if (cBase.Player.UserData.CripObjectList.Count == 0 && Manager.Stage.IsBattleOngoing && cBase.Player.UserData.BattleChampionObject.Contains(this.gameObject))
            {
                cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
            }
        }
        else
        {
            if (targetChampion == null && Manager.Stage.IsBattleOngoing &&
           cBase.Player.UserData.BattleChampionObject.Contains(this.gameObject))
            {
                targetChampion = null;
                AttackLogicStop();
                StopAllCoroutines();
                SetTargetEnemy();
            }
            else if (targetChampion == null && Manager.Stage.IsBattleOngoing && cBase.Player.UserData.BattleChampionObject.Contains(this.gameObject))
            {
                cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
            }
        }
       
    }

    #endregion

    #region 탐색 로직

    public void FindPathToTarget()
    {
        SetTargetEnemy();
    }

    private void SetTargetEnemy()
    {
        if (Manager.Stage.isCripRound)
        {
            UserData userData = cBase.Player.UserData;

            if (userData == null || userData.CripObjectList.Count == 0)
            {
                cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
                return;
            }

            int aliveCount = 0;
            GameObject closestCrip = null;
            float minDistance = float.MaxValue;

            Vector3 currentPosition = transform.position;

            foreach (var crip in userData.CripObjectList)
            {
                if (crip == null)
                    continue;

                Crip cripComponent = crip.GetComponent<Crip>();
                if (cripComponent == null)
                    continue;

                aliveCount++;

                float distance = Vector3.Distance(currentPosition, crip.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCrip = crip;
                }
            }

            if (aliveCount == 0)
            {
                cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
                return;
            }

            targetChampion = closestCrip;

            if (targetChampion == null)
            {
                cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
                return;
            }


            Manager.Stage.SetNearestTile(gameObject);
            curTile = Manager.Stage.GetParentTileInHex(gameObject);

            path.Clear();
            path = Manager.Stage.FindShortestPath_Crip(gameObject, targetChampion);



            StopAllCoroutines();
            StartCoroutine(StartMoveAndCheck_Crip());
        }
        else
        {
            if (EnemyPlayer == null || EnemyPlayer.UserData.BattleChampionObject.Count <= 0)
            {
                cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
                return;
            }

            int aliveCount = 0;
            GameObject closestChampion = null;
            float minDistance = float.MaxValue;

            Vector3 currentPosition = transform.position;

            foreach (var champion in EnemyPlayer.UserData.BattleChampionObject)
            {
                if (champion == null)
                    continue;

                ChampionBase cBase = champion.GetComponent<ChampionBase>();
                if (cBase.ChampionHpMpController.IsDie())
                    continue; // 사망한 챔피언 무시

                aliveCount++;

                float distance = Vector3.Distance(currentPosition, champion.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestChampion = champion;
                }
            }

            if (aliveCount == 0)
            {
                cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
                return;
            }

            targetChampion = closestChampion;

            if (targetChampion == null)
            {
                cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
                return;
            }

            Manager.Stage.SetNearestTile(gameObject);
            curTile = Manager.Stage.GetParentTileInHex(gameObject);

            path.Clear();
            path = Manager.Stage.FindShortestPath(gameObject, targetChampion);


            StopAllCoroutines();
            StartCoroutine(StartMoveAndCheck());
        }
    }
    #endregion

    #region 이동 로직

    private IEnumerator StartMoveAndCheck()
    {
        if (targetChampion == null)
        {
            cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
            yield break;
        }


        ChampionBase tcBase = targetChampion.GetComponent<ChampionBase>();

        if (tcBase == null)
        {
            StopAllCoroutines();
            FindPathToTarget();
            yield break;
        }


        while (targetChampion != null && MergeScene.BatteStart && path.Count > 0)
        {
            if (CanAttack(targetChampion))
            {
                StopAllCoroutines();
                path.Clear();
                cBase.ChampionStateController.ChangeState(ChampionState.Attack, cBase);
                yield break;
            }
            else
            {
                HexTile curTile = Manager.Stage.FindNearestTile(gameObject, cBase.BattleStageIndex);

                path = Manager.Stage.FindShortestPath(gameObject, targetChampion);

                if (path.Count == 0)
                {
                    cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
                    yield break;
                }

                nextTile = path[0];
            }

           
            yield return StartCoroutine(MoveOneStepAlongPath(curTile));
        }
    }

    /// <summary>
    /// 크립이동
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartMoveAndCheck_Crip()
    {
        if (targetChampion == null)
        {
            cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
            yield break;
        }

        Crip tcBase = targetChampion.GetComponent<Crip>();

        if (tcBase == null)
        {
            StopAllCoroutines();
            FindPathToTarget();
            yield break;
        }


        while (targetChampion != null && MergeScene.BatteStart && path.Count > 0)
        {
            if (CanAttack(targetChampion))
            {
                StopAllCoroutines();
                path.Clear();
                cBase.ChampionStateController.ChangeState(ChampionState.Attack, cBase);
                yield break;
            }
            else
            {
                HexTile curTile = Manager.Stage.FindNearestTile_Crip(gameObject, cBase.Player.UserData.UserId);

                path = Manager.Stage.FindShortestPath_Crip(gameObject, targetChampion);

                if (path.Count == 0)
                {
                    cBase.ChampionStateController.ChangeState(ChampionState.Idle, cBase);
                    yield break;
                }
            }

            

            yield return StartCoroutine(MoveOneStepAlongPath(curTile));
        }
    }

    private IEnumerator MoveOneStepAlongPath(HexTile curTile)
    {
        if (path == null || path.Count == 0)
            yield break;

        nextTile = path[0];

        Vector3 targetPosition = nextTile.transform.position;

        curTile.championOnTile.Remove(gameObject);
        nextTile.championOnTile.Add(gameObject);
        this.curTile = nextTile;
        gameObject.transform.SetParent(nextTile.transform);


        yield return StartCoroutine(MoveTo(targetPosition));

        float stoppingDistance = 0.1f; 

        if (Vector3.Distance(transform.position, targetPosition) <= stoppingDistance)
        {
            path.RemoveAt(0);
        }
    }

    private IEnumerator MoveTo(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            if(CanAttack(targetChampion) && targetChampion != null)
            {
                //transform.position = targetPosition;
                path.Clear();
                StopAllCoroutines();
                cBase.ChampionStateController.ChangeState(ChampionState.Attack, cBase);
                yield return null;
            }

            cBase.ChampionStateController.ChangeState(ChampionState.Move, cBase);

            Vector3 direction = (targetPosition - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, cBase.Champion_Speed  * 5f * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        
    }
    #endregion

    #region 공격 로직

    public void AttackLogic()
    {
        if(!attackLogic)
        {
            attackCoroutine = AttackRoutine();
            StartCoroutine(attackCoroutine);
        }
    }

    public void AttackLogic_Crip()
    {
        if (!attackLogic)
        {
            attackCoroutine = AttackRoutine_Crip();
            StartCoroutine(attackCoroutine);
        }
    }

    public void AttackLogicStop()
    {
        if (!attackLogic)
            return;

        attackLogic = false;
        StopAllCoroutines();
    }

    private IEnumerator AttackRoutine()
    {
        if (attackLogic)
            yield break;

        attackLogic = true;

        while (targetChampion != null && targetChampion.activeInHierarchy)
        {
            if (!CanAttack(targetChampion))
            {
                StopAllCoroutines();
                path.Clear();
                FindPathToTarget();
            }

            ChampionBase tcBase = targetChampion.GetComponent<ChampionBase>();

            if (tcBase.ChampionHpMpController.IsDie() || !CanAttack(targetChampion))
            {
                attackLogic = false;
                path.Clear();
                FindPathToTarget();
                yield break;
            }

            Vector3 directionToTarget = (targetChampion.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

            while (Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
                yield return null;
            }

            cBase.UpdateStatWithItem(cBase.EquipItem);


            if (cBase.ChampionHpMpController.IsManaFull())
            {
                cBase.ChampionHpMpController.UseSkillMana();
                CreateNormalAttack(targetChampion);
                cBase.ChampionHpMpController.NormalAttackMana();
                //CoroutineHelper.StartCoroutine(UseSkillCoroutine());
            }
            else if (!cBase.ChampionHpMpController.IsManaFull())
            {
                CreateNormalAttack(targetChampion);
                cBase.ChampionHpMpController.NormalAttackMana();
            }

            yield return new WaitForSeconds(cBase.Champion_Atk_Spd);
        }

        attackLogic = false;
    }

    private IEnumerator AttackRoutine_Crip()
    {
        if (attackLogic)
            yield break;

        attackLogic = true;


        while (targetChampion != null && targetChampion.activeInHierarchy)
        {
            if (!CanAttack(targetChampion))
            {
                StopAllCoroutines();
                path.Clear();
                FindPathToTarget();
            }

            Crip tcBase = targetChampion.GetComponent<Crip>();

            if (tcBase == null || !CanAttack(targetChampion))
            {
                attackLogic = false;
                path.Clear();
                FindPathToTarget();
                yield break;
            }

            Vector3 directionToTarget = (targetChampion.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

            while (Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
                yield return null;
            }

            cBase.UpdateStatWithItem(cBase.EquipItem);


            if (cBase.ChampionHpMpController.IsManaFull())
            {
                cBase.ChampionHpMpController.UseSkillMana();
                CreateNormalAttack(targetChampion);
                //CoroutineHelper.StartCoroutine(UseSkillCoroutine());
            }
            else if (!cBase.ChampionHpMpController.IsManaFull())
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
            GameObject projectileObject = Manager.ObjectPool.GetGo("NormalProjectile");
            projectileObject.transform.position = shootPoint.position;
            NormalProjectile projectile = projectileObject.GetComponent<NormalProjectile>();

            if (projectile != null)
            {
                projectile.SetTarget(cBase, target, cBase.GetDamage());
            }
        }
        else 
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (Manager.Stage.isCripRound)
            {
                if (distanceToTarget <= 3f)
                {                    
                    Crip targetHealth = target.GetComponent<Crip>();
                    if (targetHealth != null)
                    {
                        targetHealth.TakeDamage(cBase.GetDamage());
                    }
                }
            }
            else
            {
                if (distanceToTarget <= 3f)
                {
                    ChampionBase targetHealth = target.GetComponent<ChampionBase>();
                    if (targetHealth != null)
                    {
                        targetHealth.ChampionHpMpController.TakeDamage(cBase.GetDamage());
                    }
                }
            }
        }
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
    #endregion

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, realAttackRange);
    }
}
