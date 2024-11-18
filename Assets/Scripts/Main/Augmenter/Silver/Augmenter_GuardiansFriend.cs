using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_GuardiansFriend : BaseAugmenter
{
    // 로직 변수
    private string championName;
    private int level;
    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        level = user.UserLevel;
        championName = Manager.Champion.GetRandomChapmion(2);
        GetChampion(user, championName);
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

    public override void ApplyLevelUp(UserData user)
    {
        base.ApplyLevelUp(user);

        if(level < user.UserLevel)
        {
            level = user.UserLevel;

            if (!string.IsNullOrEmpty(championName))
                GetChampion(user, championName);
        }
       
    }
    #endregion

    #region 챔피언 생성

    private void GetChampion(UserData user, string championName)
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
        foreach(var tileEntry in user.MapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (!tile.isOccupied)
                return tile;
        }

        return null;
    }

    #endregion
}
