using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SynergyBase : MonoBehaviour
{
    public UserData UserData { get; private set;}
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

    /// <summary>
    /// ������ �´� ���� �����ϴ� �޼���
    /// </summary>
    /// <param name="user"></param>
    /// <param name="level"></param>
    protected abstract void ApplyEffects(UserData user, int level);
    protected abstract void RemoveEffects(UserData user);

    /// <summary>
    /// ������ �ó��� ȿ�� Ȱ���ϴ� �޼���
    /// </summary>
    /// <param name="user"></param>
    public abstract void Activate(UserData user);

    public abstract void Deactivate(UserData user);
}
