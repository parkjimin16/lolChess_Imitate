// 챔피언 속성 - 계열
public enum ChampionLine
{
    None = 0,
    Sugarcraft,     // 달콤술사
    Druid,          // 드루이드
    Witchcraft,     // 마녀  
    Meadist,        // 벌꿀술사 
    Frost,          // 서리 
    Eldritch,       // 섬뜩한 힘
    SpaceAndTime,   // 시공간
    Arcana,         //
    Fairy,          // 요정
    Dragon,
    Portal,         // 차원문
    Hunger,
    Pyro            // 화염
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
public enum SymbolColor
{
    None = 0,
    Bronze,
    Sliver,
    Gold,
    Special
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