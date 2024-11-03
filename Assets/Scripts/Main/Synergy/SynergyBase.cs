using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SynergyBase : MonoBehaviour
{
    public string Name { get; private set; }                // 시너지 이름
    public ChampionLine Line { get; private set; }          // 시너지 계열
    public ChampionJob Job { get; private set; }            // 시너지 직업
    public int CurrentLevel { get; private set; }           // 현재 시너지 레벨

    protected SynergyBase(string name, ChampionLine line, ChampionJob job)
    {
        Name = name;
        Line = line;
        Job = job;
    }

    public void UpdateLevel(int level)
    {
        CurrentLevel = level;
        ApplyEffects(level);
    }

    // 효과 적용을 위한 추상 메서드
    protected abstract void ApplyEffects(int level);
    protected abstract void RemoveEffects();

    // 시너지 활성화
    public abstract void Activate();

    public void Deactivate()
    {
        RemoveEffects();
        CurrentLevel = 0;  // 레벨 초기화
        Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }
}
