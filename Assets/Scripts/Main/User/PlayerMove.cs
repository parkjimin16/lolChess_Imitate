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
    private float rotationSpeed = 10f; // ȸ�� �ӵ� ������ ���� ����
    private Player player;

    public bool canMove = true; // �÷��̾ ������ �� �ִ��� ����

    private bool isInCarouselRound = false; // ���� ���� ���� �������� ����
    private Transform carouselMapTransform; // ���� ���� ���� Transform
    private MapInfo carouselMapInfo;        // ���� ���� ���� MapInfo

    private Vector3 originalPosition; // �÷��̾��� ���� ��ġ�� ����
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

            // �̵� �������� ȸ��
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // �÷��̾ ��ǥ ��ġ�� �̵�
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            // ��ǥ ��ġ�� �����ߴ��� Ȯ��
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;

                // è�Ǿ��� ������ �� ���� ��ġ�� ���ƿ��� ���
                if (hasSelectedChampion && isInCarouselRound)
                {
                    // è�Ǿ��� �θ� ����
                    //selectedChampion.transform.SetParent(null);

                    // �߰� ���� (��: �κ��丮�� �߰�)
                    // ...

                    // �� �÷��̾��� ���� ���� ���� ����
                    //canMove = false;
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
        Debug.Log($"{player.UserData.UserName}��(��) {selectedChampion.name}��(��) �����߽��ϴ�.");

        // è�Ǿ��� �÷��̾ ��������� ����
        selectedChampion.transform.SetParent(transform);
        selectedChampion.transform.localPosition = new Vector3(0, 0, -1); // �÷��̾� �ڿ� ��ġ

        // è�Ǿ��� ȸ���� �÷��̾�� �����ϰ� ����
        selectedChampion.transform.localRotation = Quaternion.identity;

        // ���� ��ġ�� �̵� ����
        if (player.UserData.PlayerType != PlayerType.Player1)
            targetPosition = startPosition;
        isMoving = true;
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
