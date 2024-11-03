using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SynergyBase : MonoBehaviour
{
    public string Name { get; private set; }                // �ó��� �̸�
    public ChampionLine Line { get; private set; }          // �ó��� �迭
    public ChampionJob Job { get; private set; }            // �ó��� ����
    public int CurrentLevel { get; private set; }           // ���� �ó��� ����

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

    // ȿ�� ������ ���� �߻� �޼���
    protected abstract void ApplyEffects(int level);
    protected abstract void RemoveEffects();

    // �ó��� Ȱ��ȭ
    public abstract void Activate();

    public void Deactivate()
    {
        RemoveEffects();
        CurrentLevel = 0;  // ���� �ʱ�ȭ
        Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }
}
