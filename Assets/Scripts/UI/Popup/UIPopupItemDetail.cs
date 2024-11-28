using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPopupItemDetail : UIPopup
{
    #region Field & Property
    [Header("UI Object")]
    [SerializeField] private GameObject uiObject;

    [Header("UI")]
    // Top
    [SerializeField]    private GameObject attributeContainer;
    [SerializeField]    private GameObject[] attributeSlot;

    private Image image_ItemIcon;
    private TextMeshProUGUI txt_ItemName;

    // Middle
    private TextMeshProUGUI txt_ItemDesc;


    [SerializeField] private GameObject normalContainer;

    private Image image_FirstItem, image_SecondItem;
    private ItemBlueprint itemData;

    #endregion

    protected override void Init()
    {
        base.Init();

        SetImage();
        SetTextMeshProUGUI();

        SetUIItemDetail();
    }

    public void SetItemData(ItemBlueprint itemBlueprint)
    {
        itemData = itemBlueprint;
    }

    private void SetImage()
    {
        SetUI<Image>();

        image_ItemIcon = GetUI<Image>("Image_ItemIcon");
        image_FirstItem = GetUI<Image>("Image_FirstItem");
        image_SecondItem = GetUI<Image>("Image_SecondItem");
    }

    private void SetTextMeshProUGUI()
    {
        SetUI<TextMeshProUGUI>();

        txt_ItemName = GetUI<TextMeshProUGUI>("Txt_ItemName");
        txt_ItemDesc = GetUI<TextMeshProUGUI>("Txt_ItemDesc");
    }


    private void SetUIItemDetail()
    {
        // top
        image_ItemIcon.sprite = itemData.Icon;
        txt_ItemName.text = itemData.ItemName;

        for(int i=0; i < attributeSlot.Length; i++)
        {
            if(i < itemData.Attribute.Count)
            {
                attributeSlot[i].SetActive(true);
                UpdateAttributeElement(attributeSlot[i], itemData.Attribute[i]);
            }
            else
            {
                attributeSlot[i].SetActive(false);
            }
        }

        // mid
        txt_ItemDesc.text = itemData.Description;


        // bottom
        normalContainer.SetActive(true);
    }

    private void UpdateAttributeElement(GameObject attributeSlot, ItemAttribute itemAttribute)
    {
        AttributeSlot aSlot = attributeSlot.GetComponent<AttributeSlot>();

        if (aSlot == null)
            return;

        aSlot.SetSlot(itemAttribute);
    }
   
    public void SetPosition(Vector2 position)
    {
        uiObject.transform.position = position;
    }
}
