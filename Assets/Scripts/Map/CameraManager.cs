using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class CameraManager
{
    private Camera mainCamera;
    private MapGenerator mapGenerator;
    private UISceneMain uiMain;
    private MinimapController minimap;

    private float cameraHeight = 18f; // 카메라 높이
    private float cameraDistance = 26f; // 카메라 거리
    private float cameraRotationX = 40f; // 카메라 X축 회전

    public void Init(Camera cam, MapGenerator map, UISceneMain main, MinimapController mini)
    {
        mainCamera = cam;
        mapGenerator = map;
        uiMain = main;
        minimap = mini;
    }

    public void MoveCameraToPlayer(Player playerData)
    {
        if(mapGenerator == null)
        {
            GameObject obj = GameObject.Find("Map");
            mapGenerator = obj.GetComponent<MapGenerator>();
        }

        MapInfo targetMap = mapGenerator.mapInfos.Find(mapInfo => mapInfo.playerData == playerData);

        if (targetMap != null)
        {
            Vector3 targetPosition = targetMap.mapTransform.position;
            Vector3 cameraPosition = targetPosition + new Vector3(0, cameraHeight, -cameraDistance);

            mainCamera.transform.position = cameraPosition;
            minimap.UpdateBoundaryColors(targetMap.mapId);
        }
        else
        {
            Debug.LogWarning("해당 플레이어의 맵을 찾을 수 없습니다.");
        }

        uiMain.UISynergyPanel.UpdateSynergy(playerData.UserData);
    }

    public void MoveCameraToSharedSelectionMap()
    {
        if (mapGenerator.sharedSelectionMapTransform != null)
        {
            Vector3 targetPosition = mapGenerator.sharedSelectionMapTransform.position;

            Vector3 cameraPosition = targetPosition + new Vector3(0, cameraHeight, -cameraDistance);
            CoroutineHelper.StartCoroutine(MoveCameraCoroutine(cameraPosition));


            minimap.UpdateBoundaryColors(-1); // -1 또는 적절한 값 사용

        }
        else
        {
            Debug.LogWarning("공동 선택 맵의 Transform을 찾을 수 없습니다.");
        }
    }

    private IEnumerator MoveCameraCoroutine(Vector3 targetPosition)
    {
        float duration = 1.0f; 
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

        mainCamera.transform.position = targetPosition;
    }
}
