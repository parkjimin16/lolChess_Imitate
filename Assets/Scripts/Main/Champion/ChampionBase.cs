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
    private ChampionFrame championFrame;

    #region Fields
    // Champion Info
    private string championName;
    private ChampionLine line_First;
    private ChampionLine line_Second;
    private ChampionJob job_First;
    private ChampionJob job_Second;
    private ChampionCost cost;


    [Header("챔피언 기본 스탯")]
    // Champion Stats_1
    [SerializeField] private List<ChampionLevelData> levelData;
    [SerializeField] private int championLevel;
    [SerializeField] private int maxHp;
    [SerializeField] private int curHp;
    [SerializeField] private int maxMana;
    [SerializeField] private int curMana;
    [SerializeField] private float speed;
    [SerializeField] private int attack_Range;
    [SerializeField] private SkillBlueprint skillBlueprint;


    // Champion Stats_2
    [SerializeField] private float ad_Power;
    [SerializeField] private float ap_Power;
    [SerializeField] private float ad_Defense;
    [SerializeField] private float ap_Defense;
    [SerializeField] private float attack_Speed;
    [SerializeField] private float critical_Percent;
    [SerializeField] private float critical_Power;
    [SerializeField] private float blood_Suck;
    [SerializeField] private float power_Upgrade;
    [SerializeField] private float total_Defense;

    [Header("아이템 스탯")]
    // Item Stats
    [SerializeField] private int item_MaxHP;
    [SerializeField] private int item_CurHP;
    [SerializeField] private int item_MaxMana;
    [SerializeField] private int item_CurMana;
    [SerializeField] private float item_Speed;

    [SerializeField] private float item_AD_Power;
    [SerializeField] private float item_AP_Power;
    [SerializeField] private float item_AD_Def;
    [SerializeField] private float item_AP_Def;
    [SerializeField] private float item_Atk_Spd;
    [SerializeField] private float item_Critical_Percent;
    [SerializeField] private float item_Cirtical_Power;
    [SerializeField] private float item_Blood_Suck;
    [SerializeField] private float item_Power_Upgrade;
    [SerializeField] private float item_Total_Def;


    [Header("최종 스탯")]
    // Display Stats
    [SerializeField] private int display_MaxHp;
    [SerializeField] private int display_CurHp;
    [SerializeField] private int display_MaxMana;
    [SerializeField] private int display_CurMana;
    [SerializeField] private float display_Speed;
    [SerializeField] private float display_AD_Power;
    [SerializeField] private float display_AP_Power;
    [SerializeField] private float display_AD_Def;
    [SerializeField] private float display_AP_Def;
    [SerializeField] private float display_Atk_Spd;
    [SerializeField] private float display_Critical_Percent;
    [SerializeField] private float display_Critical_Power;
    [SerializeField] private float display_Blood_Suck;
    [SerializeField] private float display_Power_Upgrade;
    [SerializeField] private float display_Total_Def;
    [SerializeField] private int display_Shield;
    [SerializeField] private int display_TotalDamage;


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
    public ChampionFrame ChampionFrame => championFrame;
    public List<ItemBlueprint> EquipItem => equipItem;

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
        get { return curMana; }
        set { curMana = value; }
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


    // 최종 스탯
    public int Display_MaxHp => display_MaxHp;
    public int Display_CurHp => display_CurHp;
    public int Display_MaxMana => display_MaxMana;
    public int DIsplay_CurMana => display_CurMana;

    public float Display_Speed => display_Speed;
    public float Display_AD_Power => display_AD_Power;
    public float Display_AP_Power => display_AP_Power;
    public float Display_AD_Def => display_AD_Def;
    public float Display_AP_Def => display_AP_Def;
    public float Display_Atk_Spd => display_Atk_Spd;
    public float Display_Critical_Percent => display_Critical_Percent;
    public float Display_Critical_Power => display_Critical_Power;
    public float Display_Blood_Suck => display_Blood_Suck;
    public float Display_Power_Upgrade => display_Power_Upgrade;
    public float Display_Total_Def => display_Total_Def;
    public int Display_Shield => display_Shield;
    public int Display_TotalDamage => display_TotalDamage;

    public int ChampionSellCost(int cost, int level)
    {
        if (cost == 1)
            return (cost * 3) * level;

        return (cost * 3) * level - 1;
    }

    public void SetShield(int shield)
    {
        display_Shield = shield;
    }

    public void SetTotalDamage(int damage)
    {
        display_TotalDamage = damage;
    }
    #endregion

    #region Init

    /// <summary>
    /// blueprint로 UI 생성하고 클릭해서 구매하면 SetChampion 호출
    /// </summary>
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
        curHp = maxHp;
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

        SetTotalDamage(10);

        // Champion Logic
        maxItemSlot = 3;

        UpdateDisplayStat();
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

    public void InitChampion(ChampionFrame frame)
    {
        championFrame = frame;
        championAnimController.Init(this);
        championAttackController.Init(this, attack_Speed, attack_Range, curMana, maxMana);
        championHpMpController.Init(this);
        championStateController.Init(this);
        championView.Init(this); 
        championFrame.Init(this, championBlueprint);
    }

    public void InitItemStat()
    {
        item_MaxHP = 0;
        item_CurHP = 0;
        item_MaxMana = 0;
        item_CurMana = 0;
        item_Speed = 0;
        item_AD_Power = 0;
        item_AP_Power = 0;
        item_AD_Def = 0;
        item_AP_Def = 0;
        item_Atk_Spd = 0;
        item_Critical_Percent = 0;
        item_Cirtical_Power = 0;
        item_Blood_Suck = 0;
        item_Power_Upgrade = 0;
        item_Total_Def = 0;
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            championHpMpController.TakeDamage(100);
            UpdateStat(equipItem);
        }
        else if (Input.GetKeyDown(KeyCode.Q)) // 전투종료시 아이템 증가량 초기화
        {
            foreach(ItemBlueprint it in EquipItem)
            {
                it.BaseItem.ResetItem();
            }
        }
    }


    #endregion

    #region Item

    public bool CanGetItem()
    {
        List<ItemBlueprint> Items = equipItem.Where(item => item.ItemType == ItemType.Combine || item.ItemType == ItemType.Symbol).ToList();


        return Items.Count < maxItemSlot;
    }

    public void GetItem(ItemBlueprint item)
    {
        if (!CanGetItem())
        {
            Debug.Log("Item Full");
            return;
        }
            

        if (!HasCombinedItem())
        {
            if (equipItem.Count < maxItemSlot)
            {
                if (CheckSymbol(item))
                {
                    equipItem.Add(item);
                    CombineItem();
                    championFrame.SetEquipItemImage(equipItem);     
                }
            }
            else
            {
                Debug.Log("Inventory is full!");
            }


        }
        else
        {
            if (item.ItemType == ItemType.Normal ||
                item.ItemType == ItemType.Combine ||
                item.ItemType == ItemType.Symbol)
            {
                if (CheckSymbol(item))
                {
                    equipItem.Add(item);
                    CombineItem();
                    championFrame.SetEquipItemImage(equipItem);
                }
            }
            else
            {
                Debug.Log("Cannot add CombinedItem or inventory is full!");
            }
        }

        EquipItemChampionSetting();
        UpdateStat(equipItem);
    }

    private void EquipItemChampionSetting()
    {
        foreach(var item in equipItem)
        {
            item.BaseItem.EquipChampionSetting(this.gameObject);
            item.BaseItem.InitItemSkill();
        }
    }

    public void UpdateItemStats(List<ItemBlueprint> equip)
    {
        UpdateStat(equip);
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

            if (CheckSymbol(combinedItem))
            {
                equipItem.Add(combinedItem);
            }


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

    private bool CheckSymbol(ItemBlueprint symbol)
    {
        if (symbol.CompareLine(line_First) || symbol.CompareLine(line_Second) ||
            symbol.CompareJob(job_First) || symbol.CompareJob(job_Second))
        {
            Debug.Log("Error");
            return false;
        }

        return true;
    }


    // 공격 전 아이템 스탯 정하기
    private void ItemSkillUpdate(ItemBlueprint itemblueprint)
    {
        BaseItem bItemSkill = itemblueprint.BaseItem;

        if (bItemSkill == null)
            Debug.Log("bItemSkill is Null");
        
        bItemSkill.InitTargetObject(championAttackController.TargetChampion);
    }
    #endregion

    #region Stat
    
    private void UpdateDisplayStat()
    {
        display_MaxHp = maxHp + item_MaxHP;
        display_CurHp = curHp + item_CurHP;
        display_MaxMana = maxMana + item_MaxMana;
        display_CurMana = curMana + item_CurMana;
        display_Speed =  speed + item_Speed;
        display_AD_Power = ad_Power + item_AD_Power;
        display_AP_Power = ap_Power + item_AP_Power;
        display_AD_Def = ad_Defense + item_AD_Def;
        display_AP_Def = ap_Defense + item_AP_Def;
        display_Atk_Spd = attack_Speed + item_Atk_Spd;
        display_Critical_Percent = critical_Percent + item_Critical_Percent;
        display_Critical_Power = critical_Power + item_Cirtical_Power;
        display_Blood_Suck = blood_Suck + item_Blood_Suck;
        display_Power_Upgrade = power_Upgrade + item_Power_Upgrade;
        display_Total_Def = total_Defense + item_Total_Def;
        display_Shield = 0;
    }


    public void UpdateStat(List<ItemBlueprint> equipItem)
    {
        InitItemStat();

        List<ItemAttribute> equipItemAttribute = new List<ItemAttribute>();
        
        foreach (ItemBlueprint blueprint in equipItem)
        {
            ItemSkillUpdate(blueprint);

            equipItemAttribute = blueprint.Attribute;

            foreach (ItemAttribute item in equipItemAttribute)
            {
                switch (item.ItemAttributeType)
                {
                    case ItemAttributeType.HP:
                        item_MaxHP += (int)item.AttributeValue;
                        break;
                    case ItemAttributeType.Mana:
                        item_MaxMana += (int)item.AttributeValue;
                        break;
                    case ItemAttributeType.AD_Power:
                        item_AD_Power += item.AttributeValue;
                        break;
                    case ItemAttributeType.AP_Power:
                        item_AP_Power += item.AttributeValue;
                        break;
                    case ItemAttributeType.AD_Defense:
                        item_AD_Def += (int)item.AttributeValue;
                        break;
                    case ItemAttributeType.AP_Defense:
                        item_AP_Def += (int)item.AttributeValue;
                        break;
                    case ItemAttributeType.AD_Speed:
                        item_Atk_Spd += item.AttributeValue;
                        break;
                    case ItemAttributeType.CriticalPercent:
                        item_Critical_Percent += item.AttributeValue;
                        break;
                    case ItemAttributeType.BloodSuck:
                        item_Blood_Suck += item.AttributeValue;
                        break;
                    case ItemAttributeType.TotalPower:
                        item_Power_Upgrade += item.AttributeValue;
                        break;
                    case ItemAttributeType.TotalDefense:
                        item_Total_Def += item.AttributeValue;
                        break;
                }
            }
        }

        UpdateDisplayStat();
    }

    public void ChampionLevelUp()
    {
        championLevel++;
    }

    #endregion
}