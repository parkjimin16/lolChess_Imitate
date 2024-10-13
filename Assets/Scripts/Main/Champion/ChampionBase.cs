using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class ChampionBase : MonoBehaviour
{
    [SerializeField] private Canvas UICanvas;

    #region Fields
    private ChampionBlueprint championBlueprint;
    private SkillBlueprint skillBlueprint;

    private ChampionAttackController championAttackController;
    private ChampionAnimController championAnimController;
    private ChampionHealthController championHealthController;
    private ChampionStateController championStateController;
    private GameObject skillObject;
    private BaseSkill baseSkill;
    private Rigidbody rigid;
    private ChampionView championView;
    private List<ItemBlueprint> items = new List<ItemBlueprint>();

    private string championName;
    private ChampionLine line_first;
    private ChampionLine line_second;
    private ChampionJob job_first;
    private ChampionJob job_second;
    private ChampionCost cost;

    private int championLevel;
    private int curHp;
    private int maxHp;
    private int power;

    private float attack_Speed;
    private float ad_Defense;
    private float ap_Defense;
    private float speed;
    private int curMana;
    private int maxMana;
    private int attack_Range;

    private int purchase_Cost;
    private int sell_Cost;


    // 챔피언 로직 변수
    private int maxItemSlot;
    private bool isAttacking;

    #endregion


    // 체크 전용
    private ItemDataContainerBlueprint iDataBP;


    #region Property

    public ChampionAttackController ChampionAttackController => championAttackController;
    public ChampionAnimController ChampionAnimController => championAnimController;
    public ChampionHealthController ChampionHealthController => championHealthController;
    public ChampionStateController ChampionStateController => championStateController;
    public ChampionView ChampionView => championView;

    public int CurHP
    {
        get { return curHp; }
        set { curHp = value; }
    }
    public int MaxHP
    {
        get { return curHp; }
        set { curHp = value; }
    }
    public int CurMana
    {
        get { return curHp; }
        set { curHp = value; }
    }
    public int MaxMana
    {
        get { return maxMana; }
        set { maxMana = value; }
    }
    public int Attack_Speed
    {
        get { return curHp; }
        set { curHp = value; }
    }
    public int Ad_Defense
    {
        get { return curHp; }
        set { curHp = value; }
    }
    public int Ap_Defense
    {
        get { return curHp; }
        set { curHp = value; }
    }

    public int Attack_Range
    {
        get { return attack_Range; }
        set { attack_Range = value; }
    }
    #endregion

    #region Init

    /// <summary>
    /// blueprint로 UI 생성하고 클릭해서 구매하면 SetChampion 호출
    /// </summary>
    /// <param name="blueprint"></param>
    /// <param name="position"></param>
    /// <param name="hpWeight"></param>
    /// <param name="atkWeight"></param>
    /// <param name="goldWeight"></param>
    public void SetChampion(ChampionBlueprint blueprint, Vector3 position, long hpWeight, long atkWeight, long goldWeight)
    {
        championBlueprint = blueprint;
        skillBlueprint = blueprint.SkillBlueprint;
        skillObject = blueprint.SkillBlueprint.SkillObject;
        baseSkill = blueprint.SkillBlueprint.SkillObject.GetComponent<BaseSkill>();

        championName = blueprint.ChampionName;
        line_first = blueprint.ChampionLine_First;
        line_second = blueprint.ChampionLine_Second;
        job_first = blueprint.ChampionJob_First;
        job_second = blueprint.ChampionJob_Second;
        cost = blueprint.ChampionCost;

        power = blueprint.ChampionLevelData[0].Power;
        maxHp = blueprint.ChampionLevelData[0].Hp;
        curHp = maxHp;

        attack_Speed = blueprint.AttackSpeed;
        ad_Defense = blueprint.AD_Defense;
        ap_Defense = blueprint.AP_Defense;
        speed = blueprint.Speed;
        curMana = (int)blueprint.Mana_Cur;
        maxMana = (int)blueprint.Mana_Total;
        attack_Range = blueprint.Attack_Range;

        purchase_Cost = 1;
        sell_Cost = purchase_Cost * championLevel - 1;

        SetChampionLogic();
    }

    public void SetHpBar()
    {

    }

    public void ResetHealth()
    {
        curHp = maxHp;
    }

    public void SetChampionLogic()
    {
        maxItemSlot = 3;
        isAttacking = false;
    }

    public void ChampionInit()
    {
        championAnimController.Init(this);
        championAttackController.Init(this, attack_Speed, attack_Range, curMana, maxMana);
        championHealthController.Init(this);
        championStateController.Init(this);
        championView.Init(this);
    }
    #endregion

    #region Unity Flow
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        championAttackController = GetComponent<ChampionAttackController>();
        championAnimController = GetComponent<ChampionAnimController>();
        championHealthController = GetComponent<ChampionHealthController>();
        championStateController = GetComponent<ChampionStateController>();
        championView = GetComponent<ChampionView>();
    }

    private void Start()
    {
        ChampionInit();
    }


    private void Update()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    #endregion

    #region Attack Method

    public void CreateNormalAttack(GameObject target)
    {
        ChampionBase targetHealth = target.GetComponent<ChampionBase>();
        if (targetHealth != null)
        {
            Debug.Log("Damage");
            targetHealth.TakeDamage(10);
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        GameObject target = FindTargetInRange();

        if (target == null)
        {
            Debug.Log("사거리 내에 없습니다.");
            yield break;
        }

        ChampionBase targetHealth = target.GetComponent<ChampionBase>();

        while (targetHealth != null && targetHealth.curHp > 0)
        {
            if (curMana >= maxMana)
            {
                UseSkill(target);
            }
            else
            {
                CreateNormalAttack(target);
            }

            yield return new WaitForSeconds(attack_Speed);
        }

        isAttacking = false; 
    }

    private GameObject FindTargetInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attack_Range);

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Enemy")) 
            {
                return collider.gameObject; 
            }
        }

        return null; 
    }

    public void UseSkill(GameObject target)
    {
        if (baseSkill == null)
            return;

        baseSkill.UseSkill();
    }

    public void FloatingDamage(Vector3 position, float damage)
    {

    }

    #endregion

    #region Health Method

    public void TakeDamage(float damage)
    {

    }

    private void Die()
    {

    }

    #endregion

    #region Item

    public void EquipItem(ItemBlueprint itemBlueprint)
    {
        itemBlueprint.BaseItem.ApplyItemStats();

        // 조합된 아이템이 없을 경우
        if (!HasCombinedItem())
        {
            // 칸이 비어있으면 추가
            if (items.Count < maxItemSlot)
            {
                items.Add(itemBlueprint);
            }
            else
            {
                Debug.Log("Inventory is full!");
            }
        }
        else
        {
            // 조합된 아이템이 있을 경우, 나머지 칸에만 조합 아이템 추가
            if (itemBlueprint.ItemType == ItemType.Combine && items.Count < maxItemSlot)
            {
                items.Add(itemBlueprint);
            }
            else
            {
                Debug.Log("Cannot add CombinedItem or inventory is full!");
            }
        }

        CombineItems();
    }

    // 아이템 조합
    public void CombineItems()
    {
        if (items.Count >= 2)
        {
            ItemBlueprint combineItem1 = items[0];
            ItemBlueprint combineItem2 = items[1]; 

            if (combineItem1.ItemType == ItemType.Combine && combineItem2.ItemType == ItemType.Combine)
            {
                string newId = iDataBP.FindCombineItem(combineItem1.ItemId, combineItem2.ItemId);
                ItemBlueprint combinedItem = Manager.Item.FindItemById(newId);

                items.Add(combinedItem);

                items.RemoveAt(0);
                items.RemoveAt(0);
            }
            else
            {
                Debug.Log("Cannot combine items. Both must be CombineItems.");
            }
        }
        else
        {
            Debug.Log("Not enough items to combine!");
        }
    }

    // 조합된 아이템이 있는지 확인
    private bool HasCombinedItem()
    {
        return items.Exists(item => item.ItemType == ItemType.Combine);
    }
    #endregion
}
