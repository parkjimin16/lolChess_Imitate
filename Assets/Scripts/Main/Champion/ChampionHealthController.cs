using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionHealthController : MonoBehaviour
{
    private ChampionBase cBase;

    public bool IsDie()
    {
        return cBase.CurHP <= 0;
    }

    public void Init(ChampionBase championBase)
    {
        cBase = championBase;
    }

    public void TakeDamage(float damage)
    {
        cBase.CurHP -= (int)damage;
    }

    private void Die()
    {

    }
}
