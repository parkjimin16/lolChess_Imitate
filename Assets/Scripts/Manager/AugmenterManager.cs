using System.Collections.Generic;

public class AugmenterManager
{
    #region 변수 및 프로퍼티

    private AugmenterBlueprint augmenterBlueprint;


    public AugmenterBlueprint AugmenterBlueprint => augmenterBlueprint;
    #endregion


    #region 초기화 로직

    public void Init(AugmenterBlueprint augmenterBlueprint) 
    {
        this.augmenterBlueprint = augmenterBlueprint;
    }

    #endregion

    #region 증강 선택 및 세팅 로직
    public List<AugmenterData> GetSilverAugmenters()
    {
        return augmenterBlueprint.GetRandomSilverAugments();
    }
    public List<AugmenterData> GetGoldAugmenters()
    {
        return augmenterBlueprint.GetRandomGoldAugments();
    }
    public List<AugmenterData> GetPlatinumAugmenters()
    {
        return augmenterBlueprint.GetRandomPlatinumAugments();
    }

    public void SetAugmenter(UserData user, AugmenterData augData)
    {
        user.UserAugmenter.Add(augData);
    }
    #endregion

    #region 증강 적용 로직

    public void ApplyFirstAugmenter(UserData user)
    {
        foreach(var aug in user.UserAugmenter)
        {
            aug.BaseAugmenter.ApplyNow(user);
        }
    }


    public void ApplyStartRoundAugmenter(UserData user)
    {
        foreach (var aug in user.UserAugmenter)
        {
            aug.BaseAugmenter.ApplyStartRound(user);
        }
    }

    public void ApplyEndRoundAugmenter(UserData user)
    {
        foreach (var aug in user.UserAugmenter)
        {
            aug.BaseAugmenter.ApplyEndRound(user);
        }
    }
    #endregion
}
