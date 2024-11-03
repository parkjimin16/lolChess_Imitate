using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarcraftSynergy : SynergyBase
{
    private int sugarCount;
    private float attackPower;
    private float spellPower;

    public SugarcraftSynergy()
        : base("달콤술사", ChampionLine.Sugarcraft, ChampionJob.None) { }

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
        Debug.Log($"[달콤술사] 레벨 {level} 적용: 설탕 {sugarCount}, 공격력 {attackPower}, 주문력 {spellPower}");
    }

    protected override void RemoveEffects()
    {
        throw new System.NotImplementedException();
    }

    public override void Activate()
    {
        // 전투 후 아군 챔피언이 장착한 조합 아이템 개수만큼 설탕 획득 로직 추가
        GainSugarFromAlliedItems();
    }

    private void GainSugarFromAlliedItems()
    {
        // 실제 설탕 획득 로직 구현
    }

    private void IncreaseAlliesHealth(int amount)
    {
        // 아군 체력 증가 로직 구현
    }
}
