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

    public void Init(Camera cam, MapGenerator map, UISceneMain main, MinimapController mini)
    {
        mainCamera = cam;
        mapGenerator = map;
        uiMain = main;
        minimap = mini;
    }

    public void MoveCameraToPlayer(Player playerData)
    {
        MapInfo targetMap = mapGenerator.mapInfos.Find(mapInfo => mapInfo.playerData == playerData);

        if (targetMap != null)
        {
            Vector3 targetPosition = targetMap.mapTransform.position;
            Vector3 cameraPosition = targetPosition + new Vector3(0, cameraHeight, -cameraDistance);

            mainCamera.transform.position = cameraPosition;
            minimap.UpdateBoundaryColors(targetMap.mapId);

            MovePlayerCharacterToMap(Manager.Stage.AllPlayers[0], targetMap);
        }
        else
        {
            Debug.LogWarning("해당 플레이어의 맵을 찾을 수 없습니다.");
        }

        uiMain.UISynergyPanel.UpdateSynergy(playerData.UserData);
    }
    private void MovePlayerCharacterToMap(GameObject playerObject, MapInfo targetMap)
    {
        // 플레이어 캐릭터를 새로운 맵의 위치로 이동
        Vector3 newPosition = targetMap.mapTransform.position + new Vector3(-12f, 0.8f, 6f);
        playerObject.transform.position = newPosition;

        PlayerMove playerMove = playerObject.GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            playerMove.SetCurrentMapInfo(targetMap);
        }
    }
    
    public void MoveCameraToSharedSelectionMap()
    {
        if (mapGenerator.SharedSelectionMapTransform != null)
        {
            Vector3 targetPosition = mapGenerator.SharedSelectionMapTransform.position + new Vector3(0, 3.5f, 0);

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
