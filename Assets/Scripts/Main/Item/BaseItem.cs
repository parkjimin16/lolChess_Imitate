using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseItem : MonoBehaviour
{
    #region SerializeField 
    [Header("Item Info")]
    [SerializeField] private Sprite icon;
    [SerializeField] private string itemId;
    [SerializeField] private string itemName;
    [SerializeField, TextArea] private string description;
    [SerializeField] private ItemType itemType;
    [SerializeField] private List<ItemAttribute> itemAttributes;
    [SerializeField] private GameObject equipChampion;
    [SerializeField] private ChampionBase equipChampionBase;

    [SerializeField] private Player player;


    #endregion


    #region Properity

    public Sprite Icon => icon;
    public string ItemId => itemId;
    public string ItemName => itemName;
    public string Description => description;
    public ItemType ItemType => itemType;
    public List<ItemAttribute> ItemAttributes => itemAttributes;
    public GameObject EquipChampion => equipChampion;
    public ChampionBase EquipChampionBase => equipChampionBase;

    public Player Player => player;

    #endregion

    public virtual void Initialize(ItemBlueprint blueprint)
    {
        icon = blueprint.Icon;
        itemId = blueprint.ItemId;
        itemName = blueprint.ItemName;
        description = blueprint.Description;
        itemType = blueprint.ItemType;
        itemAttributes = blueprint.Attribute;
    }

    public void EquipChampionSetting(GameObject champion, Player player)
    {
        equipChampion = champion;
        this.player = player;

        equipChampionBase = champion.GetComponent<ChampionBase>();
    }
    
    /// <summary>
    /// 공격 할때마다 호출
    /// </summary>
    /// <param name="targetChampion"></param>
    public virtual void InitTargetObject(GameObject targetChampion)
    {
        Debug.Log("Base Item Init");
    }

    public virtual void CheckHp(int curHp, int maxHp)
    {

    }

    /// <summary>
    /// 아이템 장착시 호출
    /// </summary>
    public virtual void InitItemSkill()
    {

    }

    public virtual void ResetItem()
    {

    }

    /// <summary>
    /// 처음 생성 시 호출
    /// </summary>
    public virtual void FirstItem(UserData user)
    {

    }
}
