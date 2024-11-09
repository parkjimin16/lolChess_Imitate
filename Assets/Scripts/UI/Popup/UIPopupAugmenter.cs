using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupAugmenter : UIPopup
{
    #region Field & Property

    [SerializeField] private List<AugmenterSlot> augmenterSlots;
    [SerializeField] private GameObject backObject;
    [SerializeField] private Button btn_Back;
    #endregion


    #region Init

    protected override void Init()
    {
        base.Init();

    }
    #endregion

    #region UI Update Logic

    #endregion
}
