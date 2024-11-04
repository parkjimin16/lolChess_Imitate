using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarCraftCake : MonoBehaviour
{
    [SerializeField] private GameObject[] cake;

    public void SetCake(int count)
    {
        for(int i=0; i < cake.Length; i++)
        {
            if( i <= count)
            {
                cake[i].SetActive(true);
            }
            else
            {
                cake[i].SetActive(false);
            }
        }
    }
}
