using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarouselRotation : MonoBehaviour
{
    public float rotationSpeed = 20f; // 초당 회전 각도 (degree per second)

    void Update()
    {
        // Y축을 기준으로 회전
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
