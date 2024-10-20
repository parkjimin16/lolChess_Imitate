// è�Ǿ� �Ӽ� - �迭
public enum ChampionLine
{
    None = 0,
    Sugarcraft,   // ���޼���
    Druid,
    Witchcraft,   // ����  
    Meadist,            // ���ܼ��� 
    Frost,        // ���� 
    Eldritch,     // ������ ��
    SpaceAndTime,
    Arcana,
    Fairy,
    Dragon,
    Portal,
    Hunger,
    Pyro          // ȭ��
}


// è�Ǿ� �Ӽ� - ����

public enum ChampionJob
{
    None = 0,
    Pal, // ��¦
    Mage, // ������
    Batqueen, // ���㿩��
    Shelter, // ��ȣ����
    Hunter, // ��ɲ�
    Vanguard, // ������
    Rusher, // �⵵��
    Bastion, // ���
    Enchantress, // �����
    Warrior, // ����
    Overmind, // �ʿ�ü
    Demolition, // ���Ĵ�
    Scholar, // ����
    Transmogrifier // ����ȯ��
}


// è�Ǿ� �ڽ�Ʈ
public enum ChampionCost
{
    None = 0,
    OneCost,
    TwoCost,
    ThreeCost,
    FourCost,
    FiveCost
}

public enum ChampionState
{
    Idle,
    Move,
    Attack,
    Die
}


// ������ Ÿ��
public enum ItemType
{
    None = 0,
    Normal,
    Combine,
    Using, // �Ҹ� ������ (����, ������, ������)
    Symbol, // Ư�� ������
    Relics, // ����
    Special, // ����
    Support // ����
}

public enum ItemAttributeType
{
    None = 0,
    AD_Power,
    AD_Speed,
    AD_Defense,
    AP_Power,
    AP_Defense,
    Mana,
    HP,
    CriticalPercent,
    BloodSuck,
    Total_Power,
    Special
}

// ��ų Ÿ��
public enum SkillType
{
    None = 0,
    Active,
    Passive
}

public enum DamageType
{
    Normal,
    Critical,
    Player
}

// UI Event Ÿ�� ������
public enum UIEventType
{
    Click, PointerDown, PointerUp, Drag,
}
