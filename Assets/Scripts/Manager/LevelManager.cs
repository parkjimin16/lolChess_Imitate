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
            Debug.Log($"{user.UserName}���� �̹� �ִ� ������ �����߽��ϴ�.");
            return;
        }

        user.UserExp += amount;
        //Debug.Log($"{user.UserName}�Կ��� {amount} EXP�� �߰��Ǿ����ϴ�. ���� EXP: {user.UserExp}");

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
        // ������ �� ������ �۾��� ���⿡ �߰��ϼ��� (��: ���� ����, UI ������Ʈ ��)
        //Debug.Log($"{user.UserName}���� ���� {user.UserLevel}�� ������ �߽��ϴ�!");
    }
    public bool IsMaxLevel(UserData user)
    {
        return user.UserLevel >= ExperienceTable.Length;
    }
}
