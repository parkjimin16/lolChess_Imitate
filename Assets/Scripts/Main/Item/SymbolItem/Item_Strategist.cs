using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Strategist : BaseItem
{
    public override void FirstItem(UserData user)
    {
        Debug.Log("������ ó�� ����");
        user.MaxPlaceChampion++;
    }
}
