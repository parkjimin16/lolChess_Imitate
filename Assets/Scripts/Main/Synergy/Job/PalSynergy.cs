using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalSynergy : SynergyBase
{
    // �ó��� ����
    private int level;

    // ��¦ ���� ����

    public PalSynergy()
        : base("��¦", ChampionLine.None, ChampionJob.Pal, 0) { }

    #region Ȱ�� & ��Ȱ��ȭ

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 1)
        {
            Deactivate(user);
        }
        else if(level >= 1)
        {

        }

        Debug.Log($"[��¦] ���� {level} ȿ�� ����");
    }

    protected override void RemoveEffects(UserData user)
    {
        Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    public override void Activate(UserData user)
    {
        if (level < 1)
            return;
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion
}
