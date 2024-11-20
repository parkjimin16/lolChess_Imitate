using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Strategist : BaseItem
{
    public override void FirstItem(UserData user)
    {
        Debug.Log("아이템 처음 구매");
        user.MaxPlaceChampion++;
    }
}
