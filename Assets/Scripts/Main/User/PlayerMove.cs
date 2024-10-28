using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class PlayerMove : MonoBehaviour
{
    public MapGenerator mapGenerator;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float moveSpeed = 7f;
    private float rotationSpeed = 10f; // 회전 속도 조절을 위한 변수
    private void Start()
    {
        mapGenerator = GameObject.Find("Map").GetComponent<MapGenerator>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 마우스 우클릭 감지
        {
            // 레이캐스트를 사용하여 클릭한 위치를 얻습니다.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 clickedPosition = hit.point;

                // 클릭한 위치를 경계선 내로 제한합니다.
                float clampedX = Mathf.Clamp(clickedPosition.x, mapGenerator.mapInfos[0].minX, mapGenerator.mapInfos[0].maxX);
                float clampedZ = Mathf.Clamp(clickedPosition.z, mapGenerator.mapInfos[0].minZ+4f, mapGenerator.mapInfos[0].maxZ);
                targetPosition = new Vector3(clampedX, transform.position.y, clampedZ);
                isMoving = true;
            }
        }

        if (isMoving)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;

            // 방향이 존재하면 (0 벡터가 아니면)
            if (direction != Vector3.zero)
            {
                // 목표 방향으로 회전
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // 플레이어를 목표 위치로 이동시킵니다.
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            // 목표 위치에 도달했는지 확인합니다.
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
        }
    }
}
