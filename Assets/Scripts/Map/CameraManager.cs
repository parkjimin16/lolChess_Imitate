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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            MoveCameraToSharedSelectionMap();
        }
    }

    public void MoveCameraToPlayer(Player playerData)
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
    public void MoveCameraToSharedSelectionMap()
    {
        if (mapGenerator.sharedSelectionMapTransform != null)
        {
            Vector3 targetPosition = mapGenerator.sharedSelectionMapTransform.position;

            // ī�޶� ��ġ�� ȸ�� ����
            Vector3 cameraPosition = targetPosition + new Vector3(0, cameraHeight, -cameraDistance);
            StartCoroutine(MoveCameraCoroutine(cameraPosition));

            // �ʿ� �� ��輱 ���� ������Ʈ (���� ���� ���� ��� Ư���� ó���� �ʿ��� �� ����)
            MinimapManager minimapClickHandler = FindObjectOfType<MinimapManager>();
            if (minimapClickHandler != null)
            {
                // ���� ���� ���� ��� Ư���� ID�� �����ϰų�, ��輱 ������ �⺻������ ����
                minimapClickHandler.UpdateBoundaryColors(-1); // -1 �Ǵ� ������ �� ���
            }
        }
        else
        {
            Debug.LogWarning("���� ���� ���� Transform�� ã�� �� �����ϴ�.");
        }
    }

    private IEnumerator MoveCameraCoroutine(Vector3 targetPosition)
    {
        float duration = 1.0f; // ��ȯ �ð�
        float elapsedTime = 0f;

        Vector3 startingPosition = mainCamera.transform.position;
        Quaternion startingRotation = mainCamera.transform.rotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            mainCamera.transform.position = Vector3.Lerp(startingPosition, targetPosition, t);

            yield return null;
        }

        // ��ȯ �Ϸ� �� ��Ȯ�� ��ġ�� ȸ������ ����
        mainCamera.transform.position = targetPosition;
    }
}
