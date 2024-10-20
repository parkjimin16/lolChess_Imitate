using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChampionBase : MonoBehaviour
{
    [SerializeField] private Canvas UICanvas;
    [SerializeField] private ChampionAttackController championAttackController;
    [SerializeField] private ChampionAnimController championAnimController;
    [SerializeField] private ChampionHpMpController championHpMpController;
    [SerializeField] private ChampionStateController championStateController;
    [SerializeField] private ChampionView championView;

    #region Fields
    // Champion Info
    private string championName;
    private ChampionLine line_First;
    private ChampionLine line_Second;
    private ChampionJob job_First;
    private ChampionJob job_Second;
    private ChampionCost cost;


    // Champion Stats_1
    private List<ChampionLevelData> levelData;
    private int championLevel;
    private int maxHp;
    private int curHp;
    private int maxMana;
    private int curMana;
    private float speed;
    private int attack_Range;
    private SkillBlueprint skillBlueprint;


    // Champion Stats_2
    private float ad_Power;
    private float ap_Power;
    private float ad_Defense;
    private float ap_Defense;
    private float attack_Speed;
    private float critical_Percent;
    private float critical_Power;
    private float blood_Suck;
    private float power_Upgrade;
    private float total_Defense;


    // Champion Base
    private ChampionBlueprint championBlueprint;
    private GameObject skillObject;
    private BaseSkill baseSkill;
    private Rigidbody rigid;


    // Champion Base - Item
    [SerializeField] private List<ItemBlueprint> equipItem = new List<ItemBlueprint>();
    [SerializeField] private List<ChampionLine> newChampionLine = new List<ChampionLine>();
    [SerializeField] private List<ChampionJob> newChampionJob = new List<ChampionJob>();

    private int maxItemSlot;
    private bool isAttacking;
    #endregion


    #region Property

    public ChampionAttackController ChampionAttackController => championAttackController;
    public ChampionAnimController ChampionAnimController => championAnimController;
    public ChampionHpMpController ChampionHpMpController => championHpMpController;
    public ChampionStateController ChampionStateController => championStateController;
    public ChampionView ChampionView => championView;

    // Champion Info
    public string ChampionName => championName;
    public ChampionLine ChampionLine_First => line_First;
    public ChampionLine ChampionLine_Second => line_Second;
    public ChampionJob ChampionJob_First => job_First;
    public ChampionJob ChampionJob_Second => job_Second;



    // Champion Stats_1
    public int ChampionLevel
    {
        get { return championLevel; }
        set { championLevel = value; }
    }
    public int MaxHP
    {
        get { return curHp; }
        set { curHp = value; }
    }
    public int CurHP
    {
        get { return curHp; }
        set { curHp = value; }
    }
    public int MaxMana
    {
        get { return maxMana; }
        set { maxMana = value; }
    }
    public int CurMana
    {
        get { return curHp; }
        set { curHp = value; }
    }
    public int Attack_Range
    {
        get { return attack_Range; }
        set { attack_Range = value; }
    }
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    public SkillBlueprint SkillBlueprint => skillBlueprint;


    // Champion Stats_2


    public float AD_Power
    {
        get { return ad_Power; }
        set { ad_Power = value; }
    }
    public float AP_Power
    {
        get { return ap_Power; }
        set { ap_Power = value; }
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
    public float Attack_Speed
    {
        get { return attack_Speed; }
        set { attack_Speed = value; }
    }
    public float Critical_Percent
    {
        get { return critical_Percent; }
        set { critical_Percent = value; }
    }
    public float Critical_Power
    {
        get { return critical_Power; }
        set { critical_Power = value; }
    }
    public float Blood_Suck
    {
        get { return blood_Suck; }
        set { blood_Suck = value;}
    }
    public float Power_Upgrade
    {
        get { return power_Upgrade; }
        set { power_Upgrade = value; }
    }
    public float Total_Defense
    {
        get { return total_Defense; }
        set { total_Defense = value; }
    }

    public int ChampionSellCost(int cost, int level)
    {
        if (cost == 1)
            return (cost * 3) * level;

        return (cost * 3) * level - 1;
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
    public void SetChampion(ChampionBlueprint blueprint)
    {
        blueprint.ChampionSet(blueprint.ChampionLevel);

        championBlueprint = blueprint;
        skillBlueprint = blueprint.SkillBlueprint;
        skillObject = blueprint.SkillBlueprint.SkillObject;

        if(baseSkill != null)
            baseSkill = blueprint.SkillBlueprint.SkillObject.GetComponent<BaseSkill>();


        // Champion Info
        championName = blueprint.ChampionName;
        line_First = blueprint.ChampionLine_First;
        line_Second = blueprint.ChampionLine_Second;
        job_First = blueprint.ChampionJob_First;
        job_Second = blueprint.ChampionJob_Second;
        cost = blueprint.ChampionCost;


        // Champion Stats_1
        levelData = blueprint.ChampionLevelData;
        championLevel = blueprint.ChampionLevel;
        maxHp = (int)blueprint.MaxHP;
        curHp = (int)blueprint.CurHP;
        maxMana = (int)blueprint.MaxMana;
        curMana = (int)blueprint.CurMana;
        speed = blueprint.Speed;
        attack_Range = blueprint.Attack_Range;


        // Champion Stats_2
        ad_Power = blueprint.AD_Power;
        ap_Power = blueprint.AP_Power;
        ad_Defense = blueprint.AD_Defense;
        ap_Defense = blueprint.AP_Defense;
        attack_Speed = blueprint.AttackSpeed;
        critical_Percent = blueprint.Critical_Percent;
        critical_Power = blueprint.Critical_Power;
        blood_Suck = blueprint.Blood_Suck;
        power_Upgrade = blueprint.Power_Upgrade;
        total_Defense = blueprint.Total_Defense;


        // Champion Logic
        maxItemSlot = 3;
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
        championHpMpController.Init(this);
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
        championHpMpController = GetComponent<ChampionHpMpController>();
        championStateController = GetComponent<ChampionStateController>();
        championView = GetComponent<ChampionView>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(championName);
        }
    }


    #endregion

    #region Item

    public void GetItem(ItemBlueprint item)
    {
        if (!HasCombinedItem())
        {
            if (equipItem.Count <= maxItemSlot)
            {
                equipItem.Add(item);
            }
            else
            {
                Debug.Log("Inventory is full!");
            }
        }
        else
        {
            if (item.ItemType == ItemType.Normal && equipItem.Count <= maxItemSlot)
            {
                equipItem.Add(item);
            }
            else
            {
                Debug.Log("Cannot add CombinedItem or inventory is full!");
            }
        }

        CombineItem();
    }


    public void CombineItem()
    {
        List<ItemBlueprint> normalItems = equipItem.Where(item => item.ItemType == ItemType.Normal).ToList();

        if (normalItems.Count >= 2)
        {
            ItemBlueprint combineItem1 = normalItems[0];
            ItemBlueprint combineItem2 = normalItems[1];

            string newId = Manager.Item.ItemCombine(combineItem1.ItemId, combineItem2.ItemId);
            ItemBlueprint combinedItem = Manager.Item.FindItemById(newId);

            equipItem.Add(combinedItem);

            equipItem.Remove(combineItem1);
            equipItem.Remove(combineItem2);
        }
        else
        {
            Debug.Log("Not enough normal items to combine!");
        }
    }


    // 조합된 아이템이 있는지 확인
    private bool HasCombinedItem()
    {
        return equipItem.Exists(item => item.ItemType == ItemType.Combine);
    }
    #endregion
}
