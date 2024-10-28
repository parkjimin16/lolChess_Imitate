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
    private float rotationSpeed = 10f; // ȸ�� �ӵ� ������ ���� ����
    private void Start()
    {
        mapGenerator = GameObject.Find("Map").GetComponent<MapGenerator>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // ���콺 ��Ŭ�� ����
        {
            // ����ĳ��Ʈ�� ����Ͽ� Ŭ���� ��ġ�� ����ϴ�.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 clickedPosition = hit.point;

                // Ŭ���� ��ġ�� ��輱 ���� �����մϴ�.
                float clampedX = Mathf.Clamp(clickedPosition.x, mapGenerator.mapInfos[0].minX, mapGenerator.mapInfos[0].maxX);
                float clampedZ = Mathf.Clamp(clickedPosition.z, mapGenerator.mapInfos[0].minZ+4f, mapGenerator.mapInfos[0].maxZ);
                targetPosition = new Vector3(clampedX, transform.position.y, clampedZ);
                isMoving = true;
            }
        }

        if (isMoving)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;

            // ������ �����ϸ� (0 ���Ͱ� �ƴϸ�)
            if (direction != Vector3.zero)
            {
                // ��ǥ �������� ȸ��
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // �÷��̾ ��ǥ ��ġ�� �̵���ŵ�ϴ�.
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            // ��ǥ ��ġ�� �����ߴ��� Ȯ���մϴ�.
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
        }
    }
}
