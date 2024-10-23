using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public Camera mainCamera;
    public MapGenerator mapGenerator; // MapGenerator ��ũ��Ʈ ����

    private float cameraHeight = 18f; // ī�޶� ����
    private float cameraDistance = 26f; // ī�޶� �Ÿ�
    private float cameraRotationX = 40f; // ī�޶� X�� ȸ��

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MoveCameraToPlayer(PlayerData playerData)
    {
        // �ش� �÷��̾��� �� ������ ã��
        MapInfo targetMap = mapGenerator.mapInfos.Find(mapInfo => mapInfo.playerData == playerData);

        if (targetMap != null)
        {
            Vector3 targetPosition = targetMap.mapTransform.position;

            // ī�޶� ��ġ�� ȸ�� ����
            Vector3 cameraPosition = targetPosition + new Vector3(0, cameraHeight, -cameraDistance);
            //Quaternion cameraRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);

            mainCamera.transform.position = cameraPosition;
            //mainCamera.transform.rotation = cameraRotation;

            // �ʿ� �� ��輱 ���� ������Ʈ
            MinimapManager minimapClickHandler = FindObjectOfType<MinimapManager>();
            if (minimapClickHandler != null)
            {
                minimapClickHandler.UpdateBoundaryColors(targetMap.mapId);
            }
        }
        else
        {
            Debug.LogWarning("�ش� �÷��̾��� ���� ã�� �� �����ϴ�.");
        }
    }
}
