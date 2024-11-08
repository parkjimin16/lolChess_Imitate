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
    private Player player;

    public bool canMove = true; // �÷��̾ ������ �� �ִ��� ����

    private bool isInCarouselRound = false; // ���� ���� ���� �������� ����
    private Transform carouselMapTransform; // ���� ���� ���� Transform
    private MapInfo carouselMapInfo;        // ���� ���� ���� MapInfo

    private Vector3 originalPosition; // �÷��̾��� ���� ��ġ�� ����

    private void Start()
    {
        originalPosition = transform.position;

        player = gameObject.GetComponent<Player>();
        mapGenerator = GameObject.Find("Map").GetComponent<MapGenerator>();
    }
    void Update()
    {
        if (!canMove)
            return;

        if (isInCarouselRound)
        {
            HandleCarouselMovement();
        }
        else
        {
            HandleNormalMovement();
        }
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    public void StartCarouselRound(Transform carouselTransform, MapInfo carouselInfo)
    {
        isInCarouselRound = true;
        carouselMapTransform = carouselTransform;
        carouselMapInfo = carouselInfo;
    }

    public void EndCarouselRound()
    {
        isInCarouselRound = false;
        carouselMapTransform = null;
        carouselMapInfo = null;
    }

    void HandleCarouselMovement()
    {
        if (player.UserData.PlayerType == PlayerType.Player1)
        {
            // ���콺 ��Ŭ������ �̵�
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 clickedPosition = hit.point;

                    // Ŭ���� ��ġ�� ���� ���� ���� ��輱 ���� ����
                    float clampedX = Mathf.Clamp(clickedPosition.x, carouselMapInfo.minX, carouselMapInfo.maxX);
                    float clampedZ = Mathf.Clamp(clickedPosition.z, carouselMapInfo.minZ, carouselMapInfo.maxZ);
                    targetPosition = new Vector3(clampedX, transform.position.y, clampedZ);
                    isMoving = true;
                }
            }
        }
        else
        {
            // AI �÷��̾�� �̹� ��ǥ ��ġ�� �̵� ��
        }

        // �̵� ó�� (����)
        MoveTowardsTarget();
    }
    void HandleNormalMovement()
    {
        if (player.UserData.PlayerType == PlayerType.Player1)
        {
            // ���콺 ��Ŭ������ �̵�
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 clickedPosition = hit.point;

                    // Ŭ���� ��ġ�� �ڽ��� ���� ��輱 ���� ����
                    MapInfo playerMapInfo = mapGenerator.mapInfos[0]; // �÷��̾�1�� �� ����
                    float clampedX = Mathf.Clamp(clickedPosition.x, playerMapInfo.minX, playerMapInfo.maxX);
                    float clampedZ = Mathf.Clamp(clickedPosition.z, playerMapInfo.minZ + 4f, playerMapInfo.maxZ);
                    targetPosition = new Vector3(clampedX, transform.position.y, clampedZ);
                    isMoving = true;
                }
            }
        }
        else
        {
            // AI �÷��̾�� �Ϲ� ���忡�� �������� ����
        }

        // �̵� ó�� (����)
        MoveTowardsTarget();
    }
    void MoveTowardsTarget()
    {
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

                // ���� ���� ���忡�� è�Ǿ� �����ߴ��� Ȯ��
                if (isInCarouselRound && player.UserData.PlayerType != PlayerType.Player1)
                {
                    // è�Ǿ� ���� ó��
                    SelectChampionAtPosition();
                }
            }
        }
    }
    public void MoveToRandomChampion()
    {
        // ���� ���� ���� è�Ǿ�� �� ���� ���õ��� ���� è�Ǿ��� ã���ϴ�.
        List<GameObject> availableChampions = mapGenerator.GetAvailableChampions();

        if (availableChampions.Count > 0)
        {
            // �������� è�Ǿ� ����
            GameObject targetChampion = availableChampions[Random.Range(0, availableChampions.Count)];

            // è�Ǿ� ��ġ�� �̵�
            targetPosition = targetChampion.transform.position;

            isMoving = true;
        }
        else
        {
            Debug.Log("���� ������ è�Ǿ��� �����ϴ�.");
        }
    }
    void SelectChampionAtPosition()
    {
        // ���� ��ġ�� è�Ǿ��� �ִ��� Ȯ��
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Champion"))
            {
                // è�Ǿ� ���� ó��
                GameObject selectedChampion = collider.gameObject;
                mapGenerator.RemoveChampion(selectedChampion);

                Debug.Log($"{player.UserData.UserName}��(��) {selectedChampion.name}��(��) �����߽��ϴ�.");

                // �߰����� ���� ó�� ���� (�κ��丮�� �߰� ��)

                // ������ ��Ȱ��ȭ
                canMove = false;

                break;
            }
        }
    }

    public void SaveOriginalPosition()
    {
        originalPosition = transform.position;
    }

    // ���� ��ġ�� ���ư��� �Լ�
    public void ReturnToOriginalPosition()
    {
        transform.position = originalPosition;
    }
}
