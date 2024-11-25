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

    private float cameraHeight = 18f; // ī�޶� ����
    private float cameraDistance = 26f; // ī�޶� �Ÿ�

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
            Debug.LogWarning("�ش� �÷��̾��� ���� ã�� �� �����ϴ�.");
        }

        uiMain.UISynergyPanel.UpdateSynergy(playerData.UserData);
    }
    private void MovePlayerCharacterToMap(GameObject playerObject, MapInfo targetMap)
    {
        // �÷��̾� ĳ���͸� ���ο� ���� ��ġ�� �̵�
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


            minimap.UpdateBoundaryColors(-1); // -1 �Ǵ� ������ �� ���

        }
        else
        {
            Debug.LogWarning("���� ���� ���� Transform�� ã�� �� �����ϴ�.");
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
