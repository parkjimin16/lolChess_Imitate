using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_TwoPerson : BaseAugmenter
{
    // 로직 변수
    private string championName;

    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        ItemBlueprint item = Manager.Item.GetItemBlueprint(Manager.Item.NormalItem);

        for (int i = 0; i < 2; i++)
        {
            championName = Manager.Champion.GetRandomChapmion(5);
            Champion(user, championName);

            user.MapInfo.ItemTile[0].GenerateItem(item.ItemId);
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
        if (!string.IsNullOrEmpty(championName))
            Champion(user, championName);
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

        Manager.Champion.InstantiateChampion(user, cBlueprint, tile, transform);
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
