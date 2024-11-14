using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_ThreeMusketeers : BaseAugmenter
{
    #region 증강체 로직
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

    #region 챔피언 생성

    private void Champion(UserData user, string championName)
    {
        HexTile tile = FindChampionPos(user);

        if (tile == null)
            return;

        Transform transform = tile.transform;

        ChampionBlueprint cBlueprint = Manager.Asset.GetBlueprint(championName) as ChampionBlueprint;

        Manager.Champion.InstantiateChampion(Manager.User.GetHumanUserData(), cBlueprint, tile, transform);
    }

    private HexTile FindChampionPos(UserData user)
    {
        foreach (var tileEntry in user.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (!tile.isOccupied)
                return tile;
        }

        return null;
    }

    #endregion
}
