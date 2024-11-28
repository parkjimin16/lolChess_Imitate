using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EldritchSynergy : SynergyBase
{
    // ½Ã³ÊÁö º¯¼ö
    private int level;

    // ¼¶¶àÇÑ Èû ·ÎÁ÷ º¯¼ö
    private float hp;
    private float ap_Power;
    private string eldritchName;

    private bool isActive;

    private Coroutine eldritchLogicCoroutine;


    private List<ChampionBase> eldritchChampion = new List<ChampionBase>();

    public EldritchSynergy()
        : base("¼¶¶àÇÑ Èû", ChampionLine.Eldritch, ChampionJob.None, 0) { }

    #region È°¼º & ºñÈ°¼ºÈ­

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 3)
        {
            hp = 0;
            ap_Power = 0;
            isActive = false;
            eldritchName = string.Empty;

            Deactivate(user);
            return;
        }
        else if (level >= 3 && level < 5)
        {
            hp = 0.25f;
            ap_Power = 0.1f;
            isActive = false;
            eldritchName = "´õ·´ÇôÁø °ñ·½";
        }
        else if (level >= 5 && level < 7)
        {
            hp = 0.25f;
            ap_Power = 0.1f;
            isActive = false;
            eldritchName = "¾ÏÈæ °Å¼®";
        }
        else if (level >= 7 && level < 10)
        {
            hp = 0.25f;
            ap_Power = 0.1f;
            isActive = false;
            eldritchName = "¼ö¸¹Àº ´«ÀÇ Áü½Â";
        }
        else if (level >= 10)
        {
            hp = 0.25f;
            ap_Power = 0.1f;
            isActive = false;
            eldritchName = "ÆøÇ³À» ºÎ¸£´Â ÀÚ";
        }

        //Debug.Log($"[¼¶¶àÇÑ Èû] ·¹º§ {level} Àû¿ë");
    }

    protected override void RemoveEffects(UserData user)
    {
        hp = 0;
        ap_Power = 0;
        isActive = false;
        //Debug.Log($"{Name} ½Ã³ÊÁö°¡ ºñÈ°¼ºÈ­µÇ¾ú½À´Ï´Ù.");
    }

    public override void Activate(UserData user)
    {
        if (level < 3)
            return;


        isActive = true;

        if (eldritchLogicCoroutine == null)
        {
            //eldritchLogicCoroutine = CoroutineHelper.StartCoroutine(EldritchLogicCoroutine(user));
        }
    }


    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region ¼¶¶àÇÑ Èû ·ÎÁ÷

    private IEnumerator EldritchLogicCoroutine(UserData user)
    {
        while (true)
        {
            eldritchChampion = GetEldritchChampionBase(user);

            if (isActive && CheckEldritchWarriorActive(eldritchChampion))
            {
                float setHp = 0;
                float setApPower = 0;

                foreach (var champion in eldritchChampion)
                {
                    setHp += champion.Champion_MaxHp * hp;
                    setApPower += champion.Champion_AP_Power * ap_Power;
                    champion.UpdateChampmionStat();
                }

                GameObject obj = Manager.Asset.InstantiatePrefab("EldritchWarrior");
                EldritchWarrior eWarrior = obj.GetComponent<EldritchWarrior>();
                var list = GetEnemyChampionBase(user);

                eWarrior.Init(eldritchName, setHp, setApPower, list, new Vector3(0, -8, 0));

                eldritchLogicCoroutine = null;
                yield break;
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    private List<ChampionBase> GetEldritchChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.Eldritch && cBase.ChampionLine_Second != ChampionLine.Eldritch)
                continue;

            list.Add(cBase);
        }

        return list;
    }

    private List<ChampionBase> GetEnemyChampionBase(UserData user)
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

    private bool CheckEldritchWarriorActive(List<ChampionBase> eldritchBase)
    {
        for(int i=0;i < eldritchBase.Count; i++)
        {
            if(eldritchBase[i].Champion_CurHp / eldritchBase[i].Champion_MaxHp < 0.8f)
            {
                return true;
            }
        }


        return false;
    }

    #endregion
}
