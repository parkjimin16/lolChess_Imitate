using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupAugmenter : UIPopup
{
    #region Field & Property
    [SerializeField] private List<AugmenterData> augmenterLists;


    [Header("UI º¯¼ö")]    
    [SerializeField] private List<AugmenterSlot> augmenterSlots;
    [SerializeField] private List<Button> btn_Reroll;
    [SerializeField] private GameObject backObject;
    [SerializeField] private Button btn_Back;
    
    #endregion


    #region Init

    protected override void Init()
    {
        base.Init();

        for (int i = 0; i < btn_Reroll.Count; i++)
        {
            int index = i;
            btn_Reroll[i].onClick.AddListener(() => RerollBtn(index));
        }

        btn_Back.onClick.AddListener(() => ClosePopup());
    }
    #endregion

    #region UI Update Logic

    public void InitAugmenterSilverPopup()
    {
        augmenterLists = Manager.Augmenter.GetSilverAugmenters();

        for(int i=0;i < btn_Reroll.Count; i++)
        {
            btn_Reroll[i].interactable = true;
        }

        UpdateAugmenterSlots();
    }

    public void InitAugmenterGoldPopup()
    {
        augmenterLists = Manager.Augmenter.GetGoldAugmenters();

        UpdateAugmenterSlots();
    }

    public void InitAugmenterPlatinumPopup()
    {
        augmenterLists = Manager.Augmenter.GetPlatinumAugmenters();

        UpdateAugmenterSlots();
    }

    public void UpdateAugmenterSlots()
    {
        for(int i=0; i < augmenterSlots.Count; i++)
        {
            augmenterSlots[i].InitAugmenterSlot(augmenterLists[i], Manager.User.GetHumanUserData());
            augmenterLists.Remove(augmenterLists[i]);
        }
    }

    private void RerollBtn(int buttonIndex)
    {
        augmenterSlots[buttonIndex].RerollButton(augmenterLists[0]);

        augmenterLists.Remove(augmenterLists[0]);

        btn_Reroll[buttonIndex].interactable = false;
    }

    private void ClosePopup()
    {

    }
    #endregion
}
