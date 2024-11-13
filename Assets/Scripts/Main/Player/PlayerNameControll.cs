using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameControll : MonoBehaviour
{
    private Transform mainCamera;

    void Start()
    {
        // 메인 카메라를 찾습니다.
        mainCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        // TextMeshPro가 항상 카메라를 바라보도록 설정합니다.
        transform.LookAt(transform.position + mainCamera.forward);
    }
}
