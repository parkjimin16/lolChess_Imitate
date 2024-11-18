using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAugmenter : MonoBehaviour
{
    public UserData UserData { get; private set; }

    #region 필수 메서드
    /// <summary>
    /// 증강 선택하는 순간 적용되는 메서드
    /// </summary>
    /// <param name="user"></param>
    public abstract void ApplyNow(UserData user);

    /// <summary>
    /// 매 라운드 시작 시 적용하는 메서드
    /// </summary>
    /// <param name="user"></param>
    public abstract void ApplyStartRound(UserData user);

    /// <summary>
    /// 매 라운드가 종료될 때 적용하는 메서드
    /// </summary>
    /// <param name="user"></param>
    public abstract void ApplyEndRound(UserData user);

    /// <summary>
    /// 특정 조건에 적용하는 메서드
    /// </summary>
    /// <param name="user"></param>
    public abstract void ApplyWhenever(UserData user);

    #endregion

    #region 조건 메서드
    /// <summary>
    /// 리롤 시 호출
    /// </summary>
    /// <param name="user"></param>
    public virtual void ApplyReroll(UserData user)
    {

    }

    /// <summary>
    /// 경험치 버튼 클릭 시 호출
    /// </summary>
    /// <param name="user"></param>
    public virtual void ApplyLevelUp(UserData user) 
    {
        
    }
    #endregion

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
