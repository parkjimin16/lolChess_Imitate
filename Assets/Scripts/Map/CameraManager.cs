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
    private float cameraRotationX = 40f; // ī�޶� X�� ȸ��

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
            Debug.LogWarning("�ش� �÷��̾��� ���� ã�� �� �����ϴ�.");
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
