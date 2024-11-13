using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameControll : MonoBehaviour
{
    private Transform mainCamera;

    void Start()
    {
        // ���� ī�޶� ã���ϴ�.
        mainCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        // TextMeshPro�� �׻� ī�޶� �ٶ󺸵��� �����մϴ�.
        transform.LookAt(transform.position + mainCamera.forward);
    }
}
