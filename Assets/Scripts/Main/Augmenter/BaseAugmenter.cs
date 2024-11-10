using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAugmenter : MonoBehaviour
{
    public UserData UserData { get; private set; }


    /// <summary>
    /// ���� �����ϴ� ���� ����Ǵ� �޼���
    /// </summary>
    /// <param name="user"></param>
    public abstract void ApplyNow(UserData user);

    /// <summary>
    /// �� ���� ���� �� �����ϴ� �޼���
    /// </summary>
    /// <param name="user"></param>
    public abstract void ApplyStartRound(UserData user);

    /// <summary>
    /// �� ���尡 ����� �� �����ϴ� �޼���
    /// </summary>
    /// <param name="user"></param>
    public abstract void ApplyEndRound(UserData user);

    public List<ChampionBase> GetUserChampions(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach(var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if(cBase != null)
                list.Add(cBase);
        }

        return list;
    }

    public List<ChampionBase> GetEnemyChampions(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase != null)
                list.Add(cBase);
        }

        return list;
    }
}
