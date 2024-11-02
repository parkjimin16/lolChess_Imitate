// è�Ǿ� �Ӽ� - �迭
public enum ChampionLine
{
    None = 0,
    Sugarcraft,     // ���޼���
    Druid,          // ����̵�
    Witchcraft,     // ����  
    Meadist,        // ���ܼ��� 
    Frost,          // ���� 
    Eldritch,       // ������ ��
    SpaceAndTime,   // �ð���
    Arcana,         //
    Fairy,          // ����
    Dragon,
    Portal,         // ������
    Hunger,
    Pyro            // ȭ��
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
public enum SymbolColor
{
    None = 0,
    Bronze,
    Sliver,
    Gold,
    Special
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
    HP,
    Mana,
    AD_Power,
    AP_Power,
    AD_Defense,
    AP_Defense,
    AD_Speed,
    CriticalPercent,
    BloodSuck,
    TotalPower,
    TotalDefense
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

public enum PlayerType
{
    Player1, Player2, Player3, Player4, Player5, Player6, Player7, Player8
}
public enum ItemOwner
{
    Player, Another
}
public enum MovableObjectType
{
    Item,
    Champion
}

public enum InteractionState
{
    None,
    Dragging,
    Returning
}