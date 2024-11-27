using System.Collections.Generic;

public class AugmenterManager
{
    #region ���� �� ������Ƽ

    private AugmenterBlueprint augmenterBlueprint;

    public AugmenterBlueprint AugmenterBlueprint => augmenterBlueprint;
    #endregion


    #region �ʱ�ȭ ����

    public void Init(AugmenterBlueprint augmenterBlueprint) 
    {
        this.augmenterBlueprint = augmenterBlueprint;
    }

    #endregion

    #region ���� ���� �� ���� ����
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

        if (Manager.UI.CheckPopupStack())
            Manager.UI.CloseAllPopupUI();


    }
    #endregion

    #region ���� ���� ����

    public void ApplyFirstAugmenter(UserData user, BaseAugmenter aug)
    {
        aug.ApplyNow(user);
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
    public void ApplyWheneverAugmenter(UserData user)
    {
        foreach (var aug in user.UserAugmenter)
        {
            aug.BaseAugmenter.ApplyWhenever(user);
        }
    }

    public void ApplyRerollAugmenter(UserData user) 
    {
        foreach (var aug in user.UserAugmenter)
        {
            aug.BaseAugmenter.ApplyReroll(user);
        }
    }

    public void ApplyLevelUpAugmenter(UserData user)
    {
        foreach (var aug in user.UserAugmenter)
        {
            aug.BaseAugmenter.ApplyLevelUp(user);
        }
    }
    #endregion
}
