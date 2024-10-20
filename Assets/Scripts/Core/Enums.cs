// 챔피언 속성 - 계열
public enum ChampionLine
{
    None = 0,
    Sugarcraft,   // 달콤술사
    Druid,
    Witchcraft,   // 마녀  
    Meadist,            // 벌꿀술사 
    Frost,        // 서리 
    Eldritch,     // 섬뜩한 힘
    SpaceAndTime,
    Arcana,
    Fairy,
    Dragon,
    Portal,
    Hunger,
    Pyro          // 화염
}


// 챔피언 속성 - 직업

public enum ChampionJob
{
    None = 0,
    Pal, // 단짝
    Mage, // 마도사
    Batqueen, // 박쥐여왕
    Shelter, // 보호술사
    Hunter, // 사냥꾼
    Vanguard, // 선봉대
    Rusher, // 쇄도자
    Bastion, // 요새
    Enchantress, // 요술사
    Warrior, // 전사
    Overmind, // 초월체
    Demolition, // 폭파단
    Scholar, // 학자
    Transmogrifier // 형상변환자
}


// 챔피언 코스트
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


// 아이템 타입
public enum ItemType
{
    None = 0,
    Normal,
    Combine,
    Using, // 소모 아이템 (니코, 자제기, 재조합)
    Symbol, // 특성 아이템
    Relics, // 유물
    Special, // 찬템
    Support // 지원
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

// 스킬 타입
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

// UI Event 타입 열거형
public enum UIEventType
{
    Click, PointerDown, PointerUp, Drag,
}
