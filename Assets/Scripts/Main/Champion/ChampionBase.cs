using System.Collections.Generic;
using System.Data;
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


    [Header("è�Ǿ� �⺻ ����")]
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
    [SerializeField] private float healHpValue;

    [Header("������ ����")]
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

    [Header("è�Ǿ� ��ü ���� = è�Ǿ� �⺻ ���� + ������ ����")]
    [SerializeField] private int champion_MaxHp;
    [SerializeField] private int champion_CurHp;
    [SerializeField] private int champion_MaxMana;
    [SerializeField] private int champion_CurMana;
    [SerializeField] private float champion_Speed;
    [SerializeField] private float champion_AD_Power;
    [SerializeField] private float champion_AP_Power;
    [SerializeField] private float champion_AD_Def;
    [SerializeField] private float champion_AP_Def;
    [SerializeField] private float champion_Atk_Spd;
    [SerializeField] private float champion_Critical_Percent;
    [SerializeField] private float champion_Critical_Power;
    [SerializeField] private float champion_Blood_Suck;
    [SerializeField] private float champion_Power_Upgrade;
    [SerializeField] private float champion_Total_Def;
    [SerializeField] private int champion_Shield;
    [SerializeField] private int champion_TotalDamage;  // ���� ������


    [Header("�������� ����")]
    // Display Stats
    private int display_MaxHp;
    private int display_CurHp;
    private int display_MaxMana;
    private int display_CurMana;
    private float display_Speed;
    private float display_AD_Power;
    private float display_AP_Power;
    private float display_AD_Def;
    private float display_AP_Def;
    private float display_Atk_Spd;
    private float display_Critical_Percent;
    private float display_Critical_Power;
    private float display_Blood_Suck;
    private float display_Power_Upgrade;
    private float display_Total_Def;
    private int display_Shield;
    private int display_TotalDamage;  // ���� ������


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
    public SkillBlueprint SkillBlueprint => skillBlueprint;


    // Champion Stats_2
    public float HealHpValue
    {
        get { return healHpValue; }
        set { healHpValue = value; }
    }

    public int Item_MaxMana
    {
        get { return item_MaxMana; }
        set { item_MaxMana = value; }
    }

    // è�Ǿ� ���� ����
    public int Champion_MaxHp
    {
        get => champion_MaxHp;
        set => champion_MaxHp = value;
    }
    public int Champion_CurHp
    {
        get => champion_CurHp;
        set => champion_CurHp = value;
    }
    public int Champion_MaxMana
    {
        get => champion_MaxMana;
        set => champion_MaxMana = value;
    }
    public int Champion_CurMana
    {
        get => champion_CurMana;
        set => champion_CurMana = value;
    }

    public float Champion_Speed => champion_Speed;
    public float Champion_AD_Power => champion_AD_Power;
    public float Champion_AP_Power => champion_AP_Power;
    public float Champion_AD_Def
    {
        get => champion_AD_Def;
        set => champion_AD_Def = value;
    }
    public float Champion_AP_Def
    {
        get => champion_AP_Def;
        set => champion_AP_Def = value;
    }
    public float Champion_Atk_Spd
    {
        get => champion_Atk_Spd;
        set => champion_Atk_Spd = value;
    }
    public float Champion_Critical_Percent => champion_Critical_Percent;
    public float Champion_Critical_Power => champion_Critical_Power;
    public float Champion_Blood_Suck => champion_Blood_Suck;
    public float Champion_Power_Upgrade => champion_Power_Upgrade; // �� ���ط�
    public float Champion_Total_Def => champion_Total_Def; // ������
    public int Champion_Shield => champion_Shield; // ����
    public int Champion_TotalDamage => champion_TotalDamage; // ���� ������

    public int ChampionSellCost(int cost, int level)
    {
        if (cost == 1)
            return (cost * 3) * level;

        return (cost * 3) * level - 1;
    }

    public void SetShield(int shield)
    {
        champion_Shield = shield;
    }

    public void SetTotalDamage(int damage)
    {
        champion_TotalDamage = damage;
    }

    public void SetTotalDamagePlus(float value)
    {
        float temp = champion_TotalDamage * value;
        champion_TotalDamage = (int)temp;
    }
    #endregion

    #region Init

    /// <summary>
    /// blueprint�� UI �����ϰ� Ŭ���ؼ� �����ϸ� SetChampion ȣ��
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
        healHpValue = 1.0f;

        SetTotalDamage(GetDamage());

        // Champion Logic
        maxItemSlot = 3;

        UpdateStat(EquipItem);
        UpdateChampmionStat();
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
    /// <summary>
    /// ������ ���� �ʱ�ȭ
    /// </summary>
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

    public int GetDamage()
    {
        return (int)(champion_AD_Power * (1 + champion_Total_Def));
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
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            for (int i = 0; i < equipItem.Count; i++)
            {
                var item = equipItem[i];
                item.BaseItem.ResetItem();
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
        if (equipItem.Count > maxItemSlot || equipItem.Count == 0)
            return;

        for (int i = 0; i < equipItem.Count; i++)
        {
            var item = equipItem[i];
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


    // ���յ� �������� �ִ��� Ȯ��
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


    // ���� �� ������ ���� ���ϱ�
    private void ItemSkillUpdate(ItemBlueprint itemblueprint)
    {
        BaseItem bItemSkill = itemblueprint.BaseItem;

        if (bItemSkill == null)
            Debug.Log("bItemSkill is Null");
        
        bItemSkill.InitTargetObject(championAttackController.TargetChampion);
    }
    #endregion

    #region Stat
    
    /// <summary>
    /// SetChampion ���� ȣ���ϰ� ���� �����۵� üũ
    /// </summary>
    public void UpdateChampmionStat()
    {
        champion_MaxHp = maxHp + item_MaxHP;
        champion_MaxMana = maxMana + item_MaxMana;
        champion_Speed =  speed + item_Speed;
        champion_AD_Power = ad_Power + item_AD_Power;
        champion_AP_Power = ap_Power + item_AP_Power;
        champion_AD_Def = ad_Defense + item_AD_Def;
        champion_AP_Def = ap_Defense + item_AP_Def;
        champion_Atk_Spd = attack_Speed + item_Atk_Spd;
        champion_Critical_Percent = critical_Percent + item_Critical_Percent;
        champion_Critical_Power = critical_Power + item_Cirtical_Power;
        champion_Blood_Suck = blood_Suck + item_Blood_Suck;
        champion_Power_Upgrade = power_Upgrade + item_Power_Upgrade;
        champion_Total_Def = total_Defense + item_Total_Def;
        champion_Shield = 0;

        UpdateDisplayStat();
    }

    private void UpdateDisplayStat()
    {
        display_MaxHp = champion_MaxHp;
        display_CurHp = champion_CurHp;
        display_MaxMana = champion_MaxMana;
        display_CurMana = champion_CurMana;
        display_Speed = champion_Speed;
        display_AD_Power = champion_AD_Power;
        display_AP_Power = champion_AP_Power;
        display_AD_Def = champion_AD_Def;
        display_AP_Def = champion_AP_Def;
        display_Atk_Spd = champion_Atk_Spd;
        display_Critical_Percent = champion_Critical_Percent;
        display_Critical_Power = champion_Critical_Power;
        display_Blood_Suck = champion_Blood_Suck;
        display_Power_Upgrade = champion_Power_Upgrade;
        display_Total_Def = champion_Total_Def;
        display_Shield = champion_Shield;
    }

    public void UpdateStat(List<ItemBlueprint> equipItem)
    {

        InitItemStat();

        if (equipItem.Count <= 0)
            return;

        foreach (ItemBlueprint blueprint in equipItem)
        {
            ItemSkillUpdate(blueprint);

            foreach (ItemAttribute item in blueprint.Attribute)
            {
                switch (item.ItemAttributeType)
                {
                    case ItemAttributeType.HP:
                        item_MaxHP += (int)item.GetAttributeValue();
                        item_CurHP += (int)item.GetAttributeValue();
                        break;
                    case ItemAttributeType.Mana:
                        item_CurMana += (int)item.GetAttributeValue();
                        break;
                    case ItemAttributeType.AD_Power:
                        item_AD_Power += item.GetAttributeValue();
                        break;
                    case ItemAttributeType.AP_Power:
                        item_AP_Power += item.GetAttributeValue();
                        break;
                    case ItemAttributeType.AD_Defense:
                        item_AD_Def += (int)item.GetAttributeValue();
                        break;
                    case ItemAttributeType.AP_Defense:
                        item_AP_Def += (int)item.GetAttributeValue();
                        break;
                    case ItemAttributeType.AD_Speed:
                        item_Atk_Spd += item.GetAttributeValue();
                        break;
                    case ItemAttributeType.CriticalPercent:
                        item_Critical_Percent += item.GetAttributeValue();
                        break;
                    case ItemAttributeType.BloodSuck:
                        item_Blood_Suck += item.GetAttributeValue();
                        break;
                    case ItemAttributeType.TotalPower:
                        item_Power_Upgrade += item.GetAttributeValue();
                        break;
                    case ItemAttributeType.TotalDefense:
                        item_Total_Def += item.GetAttributeValue();
                        break;
                }
            }
        }

        UpdateChampmionStat();
    }

    public void ChampionLevelUp()
    {
        if (championLevel < levelData.Count)
        {
            championLevel++;
        }
        else
        {
            championLevel = levelData.Count;
        }

        ApplyLevelData();
    }

    private void ApplyLevelData()
    {
        if (championLevel <= levelData.Count)
        {
            ChampionLevelData data = levelData[championLevel - 1];

            maxHp = data.Hp;
            curHp = maxHp;

            ad_Power = data.Power;
            UpdateStat(equipItem);
        }
    }


    #endregion
}