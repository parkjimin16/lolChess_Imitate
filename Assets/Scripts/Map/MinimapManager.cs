using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour, IPointerClickHandler
{
    public Camera minimapCamera; // �̴ϸ� ī�޶�
    public MapGenerator mapGenerator; // MapGenerator ��ũ��Ʈ ����
    public UserHpManager healthBarManager; // HealthBarManager ���� �߰�
    private int currentMapId = -1; // ���� Ȱ��ȭ�� ���� ID (-1�� �ʱⰪ)

    private void Start()
    {
        //UpdateBoundaryColors(-1);
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

            // Ŭ���� ��ġ�� ��� �ʿ� ���ϴ��� �Ǵ�
            foreach (var mapInfo in mapGenerator.mapInfos)
            {
                if (mapInfo.mapBounds.Contains(worldPosition))
                {
                    // �ش� �÷��̾��� PlayerData ��������
                    Player playerData = mapInfo.playerData;

                    if (playerData != null)
                    {
                        // ī�޶� �ش� �÷��̾��� ������ �̵�
                        CameraManager.Instance.MoveCameraToPlayer(playerData);

                        // �� ��輱 ���� ������Ʈ
                        UpdateBoundaryColors(mapInfo.mapId);

                        Manager.UserHp.ResetHealthBarSelection();
                        Manager.UserHp.HighlightHealthBar(playerData);
                    }
                    else
                    {
                        Debug.LogWarning("�ش� �ʿ� ����� PlayerData�� �����ϴ�.");
                    }

                    break;
                }
            }
        }
    }

    public void UpdateBoundaryColors(int activeMapId)
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
