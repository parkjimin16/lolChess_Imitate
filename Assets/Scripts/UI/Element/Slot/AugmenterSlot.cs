using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AugmenterSlot : MonoBehaviour
{
    [SerializeField] private AugmenterData augmenterData;
    [SerializeField] private UserData userData;
    

    [SerializeField] private Image augmenter_Icon;
    [SerializeField] private TextMeshProUGUI augmenter_Name;
    [SerializeField] private TextMeshProUGUI augmenter_Desc;
    [SerializeField] private Button btn_Reroll;
    [SerializeField] private Button btn_AugmenterSlot;


    // 시너지 정보 필요함

    public void InitAugmenterSlot(AugmenterData aug, UserData user)
    {
        userData = user;

        augmenterData = aug;
        augmenter_Icon.sprite = augmenterData.AugmenterIcon;
        augmenter_Name.text = augmenterData.AugmenterName;
        augmenter_Desc.text = augmenterData.AugmenterDescription;

        btn_AugmenterSlot.onClick.AddListener(() => AddAugmenter(userData));
    }

    public void RerollButton(AugmenterData aug)
    {
        augmenterData = aug;
        augmenter_Icon.sprite = augmenterData.AugmenterIcon;
        augmenter_Name.text = augmenterData.AugmenterName;
        augmenter_Desc.text = augmenterData.AugmenterDescription;
    }

    private void AddAugmenter(UserData user)
    {
        Manager.Augmenter.SetAugmenter(user, augmenterData);
        Manager.Augmenter.ApplyFirstAugmenter(user, augmenterData.BaseAugmenter);
    }
}
