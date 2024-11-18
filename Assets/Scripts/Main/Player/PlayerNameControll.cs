using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameControll : MonoBehaviour
{
    private Transform mainCamera;

    void Start()
    {
        mainCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.forward);
    }
}
