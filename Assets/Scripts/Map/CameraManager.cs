using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public Camera mainCamera;
    public MapGenerator mapGenerator; // MapGenerator 스크립트 참조

    private float cameraHeight = 18f; // 카메라 높이
    private float cameraDistance = 26f; // 카메라 거리
    private float cameraRotationX = 40f; // 카메라 X축 회전

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
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
        // 해당 플레이어의 맵 정보를 찾음
        MapInfo targetMap = mapGenerator.mapInfos.Find(mapInfo => mapInfo.playerData == playerData);

        if (targetMap != null)
        {
            Vector3 targetPosition = targetMap.mapTransform.position;

            // 카메라 위치와 회전 설정
            Vector3 cameraPosition = targetPosition + new Vector3(0, cameraHeight, -cameraDistance);
            //Quaternion cameraRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);

            mainCamera.transform.position = cameraPosition;
            //mainCamera.transform.rotation = cameraRotation;

            // 필요 시 경계선 색상 업데이트
            MinimapManager minimapClickHandler = FindObjectOfType<MinimapManager>();
            if (minimapClickHandler != null)
            {
                minimapClickHandler.UpdateBoundaryColors(targetMap.mapId);
            }
        }
        else
        {
            Debug.LogWarning("해당 플레이어의 맵을 찾을 수 없습니다.");
        }
    }
    public void MoveCameraToSharedSelectionMap()
    {
        if (mapGenerator.sharedSelectionMapTransform != null)
        {
            Vector3 targetPosition = mapGenerator.sharedSelectionMapTransform.position;

            // 카메라 위치와 회전 설정
            Vector3 cameraPosition = targetPosition + new Vector3(0, cameraHeight, -cameraDistance);
            StartCoroutine(MoveCameraCoroutine(cameraPosition));

            // 필요 시 경계선 색상 업데이트 (공동 선택 맵의 경우 특별한 처리가 필요할 수 있음)
            MinimapManager minimapClickHandler = FindObjectOfType<MinimapManager>();
            if (minimapClickHandler != null)
            {
                // 공동 선택 맵의 경우 특별한 ID를 전달하거나, 경계선 색상을 기본값으로 설정
                minimapClickHandler.UpdateBoundaryColors(-1); // -1 또는 적절한 값 사용
            }
        }
        else
        {
            Debug.LogWarning("공동 선택 맵의 Transform을 찾을 수 없습니다.");
        }
    }

    private IEnumerator MoveCameraCoroutine(Vector3 targetPosition)
    {
        float duration = 1.0f; // 전환 시간
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

        // 전환 완료 후 정확한 위치와 회전으로 설정
        mainCamera.transform.position = targetPosition;
    }
}
