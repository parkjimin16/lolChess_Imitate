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

    public void EquipChampionSetting(GameObject champion)
    {
        equipChampion = champion;

        equipChampionBase = champion.GetComponent<ChampionBase>();
    }
    // 타겟 가져오기(스킬 사용 여부)
    public virtual void InitTargetObject(GameObject targetChampion)
    {
        Debug.Log("Base Item Init");
    }

    public virtual void CheckHp(int curHp, int maxHp)
    {

    }

    public virtual void InitItemSkill()
    {

    }

    public virtual void ResetItem()
    {

    }

    // 테스트용
    public void OnApplicationQuit()
    {
        ResetItem();
    }
}
