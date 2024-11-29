using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AugmenterContainerSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image image_Augmenter;
    [SerializeField] private TextMeshProUGUI txt_Augmenter;
    [SerializeField] private AugmenterData augmenterData;

    public void InitAugmenterContainerSlot(AugmenterData augmenterData = null)
    {
        if(augmenterData == null)
        {
            Color color = new Color(0, 0, 0, 0);
            image_Augmenter.color = color;
            txt_Augmenter.text = string.Empty;
        }
        else
        {
            this.augmenterData = augmenterData;
            image_Augmenter.sprite = augmenterData.AugmenterIcon;
            txt_Augmenter.text = augmenterData.AugmenterName;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (augmenterData != null)
        {
            var augmenter = Manager.UI.ShowPopup<UIPopupAugmenterDetail>();

            Vector2 mousePosition = Input.mousePosition + new Vector3(200, 0, 0);
            augmenter.SetPosition(mousePosition);
            augmenter.InitAugmenterDetail(augmenterData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
