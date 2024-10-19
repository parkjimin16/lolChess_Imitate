using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour, IPointerClickHandler
{
    public Camera minimapCamera; // �̴ϸ� ī�޶�
    public Camera mainCamera;    // ���� ī�޶�
    public float cameraHeight = 18f; // ���� ī�޶� ����
    public float cameraDistance = 21f; // ���� ī�޶� �Ÿ�
    public float cameraRotationX = 45f; // ���� ī�޶� X�� ȸ��

    public MapGenerator mapGenerator; // MapGenerator ��ũ��Ʈ ����

    private int currentMapId = -1; // ���� Ȱ��ȭ�� ���� ID (-1�� �ʱⰪ)

    private void Start()
    {
        UpdateBoundaryColors(-1);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // Ŭ���� ��ũ�� ��ǥ�� �����ɴϴ�.
        Vector2 clickPosition = eventData.position;

        // �̴ϸ� RectTransform ��������
        RectTransform minimapRect = GetComponent<RectTransform>();

        // RectTransform���� Ŭ�� ��ġ�� Viewport ��ǥ�� ��ȯ
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, clickPosition, eventData.pressEventCamera, out localPoint);

        // Viewport ��ǥ�� ��ȯ (0~1 ���� ��)
        Vector2 rectSize = minimapRect.rect.size;
        Vector2 pivot = minimapRect.pivot;
        Vector2 viewportPoint = new Vector2(
            (localPoint.x / rectSize.x) + pivot.x,
            (localPoint.y / rectSize.y) + pivot.y
        );

        // �̴ϸ� ī�޶󿡼� Ŭ���� ��ġ�� ���� ��ǥ�� ��ȯ
        Ray ray = minimapCamera.ViewportPointToRay(viewportPoint);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float enter = 0f;
        if (groundPlane.Raycast(ray, out enter))
        {
            Vector3 worldPosition = ray.GetPoint(enter);
            // ���� ī�޶� �̵���ŵ�ϴ�.
            foreach (var mapInfo in mapGenerator.mapInfos)
            {
                if (mapInfo.mapBounds.Contains(worldPosition))
                {
                    // �ش� ������ ī�޶� �̵�
                    MoveMainCameraToPosition(mapInfo.mapId, mapInfo.mapTransform.position);
                    break;
                }
            }
        }
    }

    void MoveMainCameraToPosition(int mapId, Vector3 targetPosition)
    {
        // ���� ī�޶��� ��ǥ ��ġ�� ȸ�� ����
        Vector3 cameraPosition = targetPosition + new Vector3(0, cameraHeight, -cameraDistance);
        Quaternion cameraRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);

        // ī�޶� ��ġ�� ȸ���� ��� ����
        mainCamera.transform.position = cameraPosition;
        mainCamera.transform.rotation = cameraRotation;

        if (currentMapId != mapId)
        {
            UpdateBoundaryColors(mapId);
            currentMapId = mapId;
        }
    }
    void UpdateBoundaryColors(int activeMapId)
    {
        foreach (var mapInfo in mapGenerator.mapInfos)
        {
            if (mapInfo.boundaryLineRenderer != null)
            {
                if (mapInfo.mapId == activeMapId)
                {
                    // ���� Ȱ��ȭ�� ���� ��輱�� ���������� ����
                    mapInfo.boundaryLineRenderer.material.color = Color.white;
                }
                else
                {
                    // �ٸ� ���� ��輱�� �⺻ ����(��: ������)���� ����
                    mapInfo.boundaryLineRenderer.material.color = Color.red;
                }
            }
        }
    }
}
