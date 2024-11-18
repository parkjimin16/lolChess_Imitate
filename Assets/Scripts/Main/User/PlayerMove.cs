using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;
using static UnityEditor.Progress;

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
    private Vector3 startPosition;

    private bool hasSelectedChampion = false;
    private GameObject selectedChampion = null;

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
        startPosition = transform.position;
    }

    public void EndCarouselRound()
    {
        isInCarouselRound = false;
        carouselMapTransform = null;
        carouselMapInfo = null;
        //SetCanMove(true);
        //player.UserData.NonBattleChampionObject.Add(selectedChampion);
        AddCarouselChampion();
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

            // 이동 방향으로 회전
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // 플레이어를 목표 위치로 이동
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            // 목표 위치에 도달했는지 확인
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;

                // 챔피언을 선택한 후 원래 위치로 돌아왔을 경우
                if (hasSelectedChampion && isInCarouselRound)
                {
                    // 챔피언의 부모를 해제
                    //selectedChampion.transform.SetParent(null);

                    // 추가 로직 (예: 인벤토리에 추가)
                    // ...

                    // 이 플레이어의 공동 선택 라운드 종료
                    //canMove = false;
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

    void OnTriggerEnter(Collider other)
    {
        if (!isInCarouselRound || hasSelectedChampion)
            return;

        if (other.CompareTag("Champion"))
        {
            GameObject selectedChampion = other.gameObject;
            SelectChampion(selectedChampion);
        }
    }

    void SelectChampion(GameObject champion)
    {
        hasSelectedChampion = true;
        selectedChampion = champion;
        mapGenerator.RemoveChampion(selectedChampion);
        Debug.Log($"{player.UserData.UserName}이(가) {selectedChampion.name}을(를) 선택했습니다.");

        // 챔피언이 플레이어를 따라오도록 설정
        selectedChampion.transform.SetParent(transform);
        selectedChampion.transform.localPosition = new Vector3(0, 0, -1); // 플레이어 뒤에 위치

        // 챔피언의 회전을 플레이어와 동일하게 설정
        selectedChampion.transform.localRotation = Quaternion.identity;

        // 원래 위치로 이동 시작
        if (player.UserData.PlayerType != PlayerType.Player1)
            targetPosition = startPosition;
        isMoving = true;
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
    private void AddCarouselChampion()
    {
        GameObject obj = Manager.Asset.InstantiatePrefab("Capsule");
        obj.transform.position = player.UserData.MapInfo.mapTransform.position;
        Capsule cap = obj.GetComponent<Capsule>();

        List<ItemBlueprint> item = new List<ItemBlueprint>();
        List<string> itemid = new List<string>();
        List<string> champion = new List<string>();
        //GameObject obj = Manager.Asset.InstantiatePrefab("Capsule");
        //Capsule cap = obj.GetComponent<Capsule>();

        ChampionBase cBase = selectedChampion.GetComponent<ChampionBase>();
        ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint(cBase.ChampionName) as ChampionBlueprint;
        champion.Add(championBlueprint.ChampionInstantiateName);
        Debug.Log(champion[0]);

        item = cBase.EquipItem;
        foreach (var itemid1 in item)
        {
            itemid.Add(itemid1.ItemId);
        }

        cap.InitCapsule(0, itemid, champion);

        Destroy(selectedChampion);
        /*HexTile emptyTile = FindEmptyRectTile();
        if(emptyTile != null)
        {
            selectedChampion.transform.position = emptyTile.transform.position;
            selectedChampion.transform.SetParent(emptyTile.transform);
            emptyTile.championOnTile.Add(selectedChampion);
        }*/
    }
    private HexTile FindEmptyRectTile()
    {
        MapGenerator.MapInfo mapInfo = player.UserData.MapInfo;
        foreach (var tileEntry in mapInfo.RectDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (!tile.isOccupied)
            {
                return tile;
            }
        }
        return null;
    }
}
