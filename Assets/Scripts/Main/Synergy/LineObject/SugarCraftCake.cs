using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarCraftCake : MonoBehaviour
{
    [SerializeField] private GameObject[] cake;

    public UserData OwnerUserData;

    public void SetCake(int count)
    {
        for(int i=0; i < cake.Length; i++)
        {
            if( i <= count)
            {
                cake[i].SetActive(true);
                Debug.Log("ÄÑÁü");
            }
            else
            {
                cake[i].SetActive(false);
                Debug.Log("²¨Áü2");
            }
        }
    }
    public class SugarCakeOriginalState
    {
        public Vector3 originalPosition;
        public Transform originalParent;
        public bool wasActive;
    }
}
