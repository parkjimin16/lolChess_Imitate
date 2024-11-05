using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CripData", menuName = "Blueprints/CripData")]
public class CripObjectData : ScriptableObject
{
    [SerializeField] private int Hp;

    public int HP => Hp;

}
