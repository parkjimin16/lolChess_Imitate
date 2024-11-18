using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttributeSlot : MonoBehaviour
{
    [SerializeField] private Image image_Icon;
    [SerializeField] private TextMeshProUGUI text_Value;

    public void SetSlot(ItemAttribute iAttribute)
    {
        // 이미지 매핑
        image_Icon.sprite = Manager.Item.GetIcon(iAttribute.ItemAttributeType);

        string value = Utilities.SetDescriptionValueReturnString(iAttribute);
        text_Value.text = $"+ {value}";
    }
}
