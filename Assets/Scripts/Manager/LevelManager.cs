using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
    public readonly int[] ExperienceTable = {
        0, 2, 2, 6, 10, 20, 36, 48, 76, 76 // Lv1 ~ Lv10
    };

    public void AddExperience(UserData user, int amount)
    {
        if (IsMaxLevel(user))
        {
            Debug.Log($"{user.UserName}님은 이미 최대 레벨에 도달했습니다.");
            return;
        }

        user.UserExp += amount;
        CheckLevelUp(user);
    }
    private void CheckLevelUp(UserData user)
    {
        while (user.UserLevel < ExperienceTable.Length && user.UserExp >= ExperienceTable[user.UserLevel])
        {
            user.UserExp -= ExperienceTable[user.UserLevel];
            user.UserLevel++;
            Manager.User.UpdateMaxChampion(user);
            if (user.UIMain != null)
            {
                user.UIMain.UIShopPanel.UpdateChampionPercent(user);
            }

            user.Player.SetPlayerLevelText(user.UserLevel);
        }
    }

    private bool IsMaxLevel(UserData user)
    {
        return user.UserLevel >= ExperienceTable.Length;
    }
}
