using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_ThreeMusketeers : BaseAugmenter
{
    #region ����ü ����
    public override void ApplyNow(UserData user)
    {
        for (int i = 0; i < 5; i++)
        {
            string championName = Manager.Champion.GetRandomChapmion(3);
            Champion(user, championName);
        }
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

    #region è�Ǿ� ����

    private void Champion(UserData user, string championName)
    {
        HexTile tile = Manager.Champion.FindChampionPos(user);

        if (tile == null)
            return;

        Transform transform = tile.transform;

        ChampionBlueprint cBlueprint = Manager.Asset.GetBlueprint(championName) as ChampionBlueprint;

        Manager.Champion.InstantiateChampion(user, cBlueprint, tile, transform);
    }



    #endregion
}
