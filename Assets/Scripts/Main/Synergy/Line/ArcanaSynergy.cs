using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcanaSynergy : SynergyBase
{
    // �ó��� ����
    private int level;

    // �Ƹ�ī�� ���� ����

    public ArcanaSynergy()
        : base("�Ƹ�ī��", ChampionLine.Arcana, ChampionJob.None, 0) { }

    #region Ȱ�� & ��Ȱ��ȭ

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {

            Deactivate(user);
            return;
        }
        else if (level == 2)
        {

        }
        else if (level == 3)
        {

        }
        else if (level == 4)
        {

        }
        else if (level == 5)
        {

        }

        Debug.Log($"[�Ƹ�ī��] ���� {level} ����");
    }

    protected override void RemoveEffects(UserData user)
    {

        Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    public override void Activate(UserData user)
    {
        if (level < 3)
            return;


    }


    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion
}
