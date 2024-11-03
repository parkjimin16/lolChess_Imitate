using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarcraftSynergy : SynergyBase
{
    private int sugarCount;
    private float attackPower;
    private float spellPower;

    public SugarcraftSynergy()
        : base("���޼���", ChampionLine.Sugarcraft, ChampionJob.None) { }

    protected override void ApplyEffects(int level)
    {
        sugarCount = 0;
        attackPower = 0;
        spellPower = 0;

        if (level >= 2 && level <= 3)
        {
            sugarCount = 3;
            attackPower = 22;
            spellPower = 22;
        }
        else if (level >= 4 && level <= 5)
        {
            sugarCount = 4;
            attackPower = 33;
            spellPower = 33;
        }
        else if (level >= 6)
        {
            sugarCount = 6;
            attackPower = 36;
            spellPower = 36;
            IncreaseAlliesHealth(100);
        }
        Debug.Log($"[���޼���] ���� {level} ����: ���� {sugarCount}, ���ݷ� {attackPower}, �ֹ��� {spellPower}");
    }

    protected override void RemoveEffects()
    {
        throw new System.NotImplementedException();
    }

    public override void Activate()
    {
        // ���� �� �Ʊ� è�Ǿ��� ������ ���� ������ ������ŭ ���� ȹ�� ���� �߰�
        GainSugarFromAlliedItems();
    }

    private void GainSugarFromAlliedItems()
    {
        // ���� ���� ȹ�� ���� ����
    }

    private void IncreaseAlliesHealth(int amount)
    {
        // �Ʊ� ü�� ���� ���� ����
    }
}
