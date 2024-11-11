using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_ItemBundle : BaseAugmenter
{
    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        ItemBlueprint item = Manager.Item.GetItemBlueprint(Manager.Item.CombineItem);

        user.MapInfo.ItemTile[0].GenerateItem(item.ItemId);
    }

    public override void ApplyStartRound(UserData user)
    {

    }

    public override void ApplyEndRound(UserData user)
    {

    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
