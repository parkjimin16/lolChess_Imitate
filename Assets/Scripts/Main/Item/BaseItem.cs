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


    #endregion


    #region Properity

    public Sprite Icon => icon;
    public string ItemId => itemId;
    public string ItemName => itemName;
    public string Description => description;
    public ItemType ItemType => itemType;
    public List<ItemAttribute> ItemAttributes => itemAttributes;

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

    // 조합 함수 호출
    public virtual void CheckCombine()
    {

    }

    // 아이템 능력치 적용

    public virtual void ApplyItemStats()
    {
        ApplyItemStatsTotal();
    }

    private void ApplyItemStatsTotal()
    {
        foreach (var attribute in itemAttributes)
        {
            switch (attribute.ItemAttributeType)
            {
                case ItemAttributeType.AD_Power:
                    ApplyAdPower(attribute.AttributeValue);
                    break;
                case ItemAttributeType.AD_Speed:
                    ApplyAdSpeed(attribute.AttributeValue);
                    break;
                case ItemAttributeType.AD_Defense:
                    ApplyAdDefense(attribute.AttributeValue);
                    break;
                case ItemAttributeType.AP_Power:
                    ApplyApPower(attribute.AttributeValue);
                    break;
                case ItemAttributeType.AP_Defense:
                    ApplyApDefense(attribute.AttributeValue);
                    break;
                case ItemAttributeType.Mana:
                    ApplyMana(attribute.AttributeValue);
                    break;
                case ItemAttributeType.HP:
                    ApplyHP(attribute.AttributeValue);
                    break;
                case ItemAttributeType.CriticalPercent:
                    ApplyCriticalPercent(attribute.AttributeValue);
                    break;
                case ItemAttributeType.BloodSuck:
                    ApplyBloodSuck(attribute.AttributeValue);
                    break;
                case ItemAttributeType.Total_Power:
                    ApplyTotalPower(attribute.AttributeValue);
                    break;
                case ItemAttributeType.Special:
                    ApplySpecial(attribute.AttributeValue);
                    break;
            }
        }
    }

    #region Stats Apply

    private void ApplyAdPower(float value)
    {
        Debug.Log("ApplyAdPower : " + value);
    }

    private void ApplyAdSpeed(float value)
    {
        Debug.Log("ApplyAdSpeed : " + value);
    }

    private void ApplyAdDefense(float value) 
    { 
        Debug.Log("ApplyAdDefense : " + value);
    }
    private void ApplyApPower(float value)
    {
        Debug.Log("ApplyApPower : " + value);
    }
    private void ApplyApDefense(float value)
    {
        Debug.Log("ApplyApDefense : " + value);
    }
    private void ApplyMana(float value)
    {
        Debug.Log("ApplyMana : " + value);
    }
    private void ApplyHP(float value)
    {
        Debug.Log("ApplyHP : " + value);
    }
    private void ApplyCriticalPercent(float value)
    {
        Debug.Log("ApplyCriticalPercent : " + value);
    }
    private void ApplyBloodSuck(float value)
    {
        Debug.Log("ApplyBloodSuck : " + value);
    }
    private void ApplyTotalPower(float value)
    {
        Debug.Log("ApplyTotalPower : " + value);
    }
    private void ApplySpecial(float value)
    {
        Debug.Log("ApplySpecial : " + value);
    }
    #endregion
}
