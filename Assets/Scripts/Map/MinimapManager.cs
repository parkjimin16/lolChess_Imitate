using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour, IPointerClickHandler
{
    public Camera minimapCamera; // 미니맵 카메라
    public MapGenerator mapGenerator; // MapGenerator 스크립트 참조
    public UserHpManager healthBarManager; // HealthBarManager 참조 추가
    private int currentMapId = -1; // 현재 활성화된 맵의 ID (-1은 초기값)

    private void Start()
    {
        //UpdateBoundaryColors(-1);
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

            // 클릭된 위치가 어느 맵에 속하는지 판단
            foreach (var mapInfo in mapGenerator.mapInfos)
            {
                if (mapInfo.mapBounds.Contains(worldPosition))
                {
                    // 해당 플레이어의 PlayerData 가져오기
                    Player playerData = mapInfo.playerData;

                    if (playerData != null)
                    {
                        // 카메라를 해당 플레이어의 맵으로 이동
                        CameraManager.Instance.MoveCameraToPlayer(playerData);

                        // 맵 경계선 색상 업데이트
                        UpdateBoundaryColors(mapInfo.mapId);

                        Manager.UserHp.ResetHealthBarSelection();
                        Manager.UserHp.HighlightHealthBar(playerData);
                    }
                    else
                    {
                        Debug.LogWarning("해당 맵에 연결된 PlayerData가 없습니다.");
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
