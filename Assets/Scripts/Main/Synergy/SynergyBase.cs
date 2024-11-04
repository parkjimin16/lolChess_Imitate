using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SynergyBase : MonoBehaviour
{
    public UserData userData { get; private set;}
    public string Name { get; private set; }                // �ó��� �̸�
    public ChampionLine Line { get; private set; }          // �ó��� �迭
    public ChampionJob Job { get; private set; }            // �ó��� ����
    public int CurrentLevel { get; private set; }           // ���� �ó��� ����

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

    // ȿ�� ������ ���� �߻� �޼���
    protected abstract void ApplyEffects(UserData user, int level);
    protected abstract void RemoveEffects(UserData user);

    // �ó��� Ȱ��ȭ
    public abstract void Activate(UserData user);

    public abstract void Deactivate(UserData user);
}
