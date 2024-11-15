using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
    private readonly int[] ExperienceTable = {
        0, 2, 2, 6, 10, 20, 36, 48, 76, 76 // Lv1 ~ Lv10
    };

    public void AddExperience(UserData user, int amount)
    {
        if (user.UserLevel >= 10)
        {
            Debug.Log($"{user.UserName}님은 이미 최대 레벨에 도달했습니다.");
            return;
        }

        user.UserExp += amount;
        //Debug.Log($"{user.UserName}님에게 {amount} EXP가 추가되었습니다. 현재 EXP: {user.UserExp}");

        CheckLevelUp(user);
    }
    private void CheckLevelUp(UserData user)
    {
        while (user.UserLevel < ExperienceTable.Length && user.UserExp >= ExperienceTable[user.UserLevel])
        {
            user.UserExp -= ExperienceTable[user.UserLevel];
            user.UserLevel++;
            OnLevelUp(user);
        }
    }
    private void OnLevelUp(UserData user)
    {
        // 레벨업 시 수행할 작업을 여기에 추가하세요 (예: 스탯 증가, UI 업데이트 등)
        //Debug.Log($"{user.UserName}님이 레벨 {user.UserLevel}로 레벨업 했습니다!");
    }
    public bool IsMaxLevel(UserData user)
    {
        return user.UserLevel >= ExperienceTable.Length;
    }
}
