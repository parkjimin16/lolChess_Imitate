using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHeatlhBarPanel : UIBase
{
    [SerializeField] private List<HealthBarSlot> healthBarList;


    #region Init Slots

    public void InitHpBarPanel()
    {
        Manager.UserHp.InitUserHp(healthBarList);
    }
    #endregion
}
