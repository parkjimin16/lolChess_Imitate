using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SynergyBase : MonoBehaviour
{
    public UserData userData { get; private set;}
    public string Name { get; private set; }                // 시너지 이름
    public ChampionLine Line { get; private set; }          // 시너지 계열
    public ChampionJob Job { get; private set; }            // 시너지 직업
    public int CurrentLevel { get; private set; }           // 현재 시너지 레벨

    protected SynergyBase(string name, ChampionLine line, ChampionJob job, int curLevel)
    {
        Name = name;
        Line = line;
        Job = job;
        CurrentLevel = curLevel;
    }

    public void UpdateLevel(UserData user, int level)
    {
        CurrentLevel = level;
        ApplyEffects(user, level);
    }

    // 효과 적용을 위한 추상 메서드
    protected abstract void ApplyEffects(UserData user, int level);
    protected abstract void RemoveEffects(UserData user);

    // 시너지 활성화
    public abstract void Activate(UserData user);

    public abstract void Deactivate(UserData user);
}
