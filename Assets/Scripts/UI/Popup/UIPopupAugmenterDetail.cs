using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPopupAugmenterDetail : UIPopup
{
    [SerializeField] private GameObject uiObject;
    [SerializeField] private AugmenterData augmenter;
    [SerializeField] private Image image_Augmenter;
    [SerializeField] private TextMeshProUGUI txt_Augmenter;
    [SerializeField] private TextMeshProUGUI txt_AugmenterDetail;

    public void InitAugmenterDetail(AugmenterData aug)
    {
        augmenter = aug;

        image_Augmenter.sprite = augmenter.AugmenterIcon;
        txt_Augmenter.text = augmenter.AugmenterName;
        txt_AugmenterDetail.text = augmenter.AugmenterDescription;
    }

    public void SetPosition(Vector2 position)
    {
        uiObject.transform.position = position;
    }
}
