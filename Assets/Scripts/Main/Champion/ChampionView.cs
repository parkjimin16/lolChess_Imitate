using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChampionView : MonoBehaviour
{
    private ChampionBase cBase;

    [SerializeField] private TextMeshPro text_ChampionName;
    public void Init(ChampionBase championBase)
    {
        cBase = championBase;
    }
}
