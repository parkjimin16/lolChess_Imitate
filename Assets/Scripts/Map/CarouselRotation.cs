using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarouselRotation : MonoBehaviour
{
    public float rotationSpeed = 20f; // �ʴ� ȸ�� ���� (degree per second)

    void Update()
    {
        // Y���� �������� ȸ��
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
