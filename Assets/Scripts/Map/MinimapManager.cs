using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour, IPointerClickHandler
{
    public Camera minimapCamera; // 미니맵 카메라
    public Camera mainCamera;    // 메인 카메라
    public float cameraHeight = 18f; // 메인 카메라 높이
    public float cameraDistance = 21f; // 메인 카메라 거리
    public float cameraRotationX = 45f; // 메인 카메라 X축 회전

    public MapGenerator mapGenerator; // MapGenerator 스크립트 참조

    private int currentMapId = -1; // 현재 활성화된 맵의 ID (-1은 초기값)

    private void Start()
    {
        UpdateBoundaryColors(-1);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭한 스크린 좌표를 가져옵니다.
        Vector2 clickPosition = eventData.position;

        // 미니맵 RectTransform 가져오기
        RectTransform minimapRect = GetComponent<RectTransform>();

        // RectTransform에서 클릭 위치를 Viewport 좌표로 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, clickPosition, eventData.pressEventCamera, out localPoint);

        // Viewport 좌표로 변환 (0~1 사이 값)
        Vector2 rectSize = minimapRect.rect.size;
        Vector2 pivot = minimapRect.pivot;
        Vector2 viewportPoint = new Vector2(
            (localPoint.x / rectSize.x) + pivot.x,
            (localPoint.y / rectSize.y) + pivot.y
        );

        // 미니맵 카메라에서 클릭한 위치를 월드 좌표로 변환
        Ray ray = minimapCamera.ViewportPointToRay(viewportPoint);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float enter = 0f;
        if (groundPlane.Raycast(ray, out enter))
        {
            Vector3 worldPosition = ray.GetPoint(enter);
            // 메인 카메라를 이동시킵니다.
            foreach (var mapInfo in mapGenerator.mapInfos)
            {
                if (mapInfo.mapBounds.Contains(worldPosition))
                {
                    // 해당 맵으로 카메라 이동
                    MoveMainCameraToPosition(mapInfo.mapId, mapInfo.mapTransform.position);
                    break;
                }
            }
        }
    }

    void MoveMainCameraToPosition(int mapId, Vector3 targetPosition)
    {
        // 메인 카메라의 목표 위치와 회전 설정
        Vector3 cameraPosition = targetPosition + new Vector3(0, cameraHeight, -cameraDistance);
        Quaternion cameraRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);

        // 카메라 위치와 회전을 즉시 설정
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
                    // 현재 활성화된 맵의 경계선을 검정색으로 변경
                    mapInfo.boundaryLineRenderer.material.color = Color.white;
                }
                else
                {
                    // 다른 맵의 경계선을 기본 색상(예: 빨간색)으로 변경
                    mapInfo.boundaryLineRenderer.material.color = Color.red;
                }
            }
        }
    }
}
