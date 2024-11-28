using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Strategist : BaseItem
{
    public override void FirstItem(UserData user)
    {
        Manager.User.UpdateMaxChampion(user);
    }
}
