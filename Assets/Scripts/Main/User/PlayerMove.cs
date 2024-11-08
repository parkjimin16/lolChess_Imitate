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
    private Player player;

    public bool canMove = true; // 플레이어가 움직일 수 있는지 여부

    private bool isInCarouselRound = false; // 현재 공동 선택 라운드인지 여부
    private Transform carouselMapTransform; // 공동 선택 맵의 Transform
    private MapInfo carouselMapInfo;        // 공동 선택 맵의 MapInfo

    private Vector3 originalPosition; // 플레이어의 원래 위치를 저장

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
            // 마우스 우클릭으로 이동
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 clickedPosition = hit.point;

                    // 클릭한 위치를 공동 선택 맵의 경계선 내로 제한
                    float clampedX = Mathf.Clamp(clickedPosition.x, carouselMapInfo.minX, carouselMapInfo.maxX);
                    float clampedZ = Mathf.Clamp(clickedPosition.z, carouselMapInfo.minZ, carouselMapInfo.maxZ);
                    targetPosition = new Vector3(clampedX, transform.position.y, clampedZ);
                    isMoving = true;
                }
            }
        }
        else
        {
            // AI 플레이어는 이미 목표 위치로 이동 중
        }

        // 이동 처리 (공통)
        MoveTowardsTarget();
    }
    void HandleNormalMovement()
    {
        if (player.UserData.PlayerType == PlayerType.Player1)
        {
            // 마우스 우클릭으로 이동
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 clickedPosition = hit.point;

                    // 클릭한 위치를 자신의 맵의 경계선 내로 제한
                    MapInfo playerMapInfo = mapGenerator.mapInfos[0]; // 플레이어1의 맵 정보
                    float clampedX = Mathf.Clamp(clickedPosition.x, playerMapInfo.minX, playerMapInfo.maxX);
                    float clampedZ = Mathf.Clamp(clickedPosition.z, playerMapInfo.minZ + 4f, playerMapInfo.maxZ);
                    targetPosition = new Vector3(clampedX, transform.position.y, clampedZ);
                    isMoving = true;
                }
            }
        }
        else
        {
            // AI 플레이어는 일반 라운드에서 움직이지 않음
        }

        // 이동 처리 (공통)
        MoveTowardsTarget();
    }
    void MoveTowardsTarget()
    {
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

                // 공동 선택 라운드에서 챔피언에 도달했는지 확인
                if (isInCarouselRound && player.UserData.PlayerType != PlayerType.Player1)
                {
                    // 챔피언 선택 처리
                    SelectChampionAtPosition();
                }
            }
        }
    }
    public void MoveToRandomChampion()
    {
        // 공동 선택 맵의 챔피언들 중 아직 선택되지 않은 챔피언을 찾습니다.
        List<GameObject> availableChampions = mapGenerator.GetAvailableChampions();

        if (availableChampions.Count > 0)
        {
            // 무작위로 챔피언 선택
            GameObject targetChampion = availableChampions[Random.Range(0, availableChampions.Count)];

            // 챔피언 위치로 이동
            targetPosition = targetChampion.transform.position;

            isMoving = true;
        }
        else
        {
            Debug.Log("선택 가능한 챔피언이 없습니다.");
        }
    }
    void SelectChampionAtPosition()
    {
        // 현재 위치에 챔피언이 있는지 확인
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Champion"))
            {
                // 챔피언 선택 처리
                GameObject selectedChampion = collider.gameObject;
                mapGenerator.RemoveChampion(selectedChampion);

                Debug.Log($"{player.UserData.UserName}이(가) {selectedChampion.name}을(를) 선택했습니다.");

                // 추가적인 선택 처리 로직 (인벤토리에 추가 등)

                // 움직임 비활성화
                canMove = false;

                break;
            }
        }
    }

    public void SaveOriginalPosition()
    {
        originalPosition = transform.position;
    }

    // 원래 위치로 돌아가는 함수
    public void ReturnToOriginalPosition()
    {
        transform.position = originalPosition;
    }
}
