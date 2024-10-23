using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    private int width = 7;
    private int height = 8;
    private int rectWidth = 9; // 직사각형 타일의 개수
    private float desiredMapWidth = 20f; // 원하는 맵의 가로 크기 (단위: 유니티 월드 좌표)
    private float tileSize; // 타일의 크기 (자동 계산될 예정)

    public GameObject hexTilePrefab; // 헥사곤 타일 프리팹
    public GameObject rectTilePrefab; // 직사각형 타일 프리팹
    public GameObject itemTilePrefab; // 아이템 타일 프리팹
    public GameObject goldTilePrefeb; // 골드 타일 프리팹
    public GameObject goldPrefeb; // 골드 표시 프리팹
    public Camera minimapCamera;
    public StageManager stageManager;

    public float gapBetweenTiles = 0.1f; // 타일 간격 조정용 변수

    private float rectWidthSize; // 사각형 타일 폭

    public int maxGoldSlots = 5; // 최대 골드 표시 칸 수
    public float goldSlotSize = 1f; // 골드 표시 칸의 크기
    public float goldSlotSpacing = 0.1f; // 골드 표시 칸 간의 간격

    private float mapWidthSize;
    private float mapHeightSize;
    private float mapSpacing = 40f; // 각 맵 간의 거리

    private float boundaryExtraWidth = 10f;  // 가로 방향으로 추가할 길이
    private float boundaryExtraHeight = 10f;

    

    private void Awake()
    {
        CalculateTileSize();
        CreatUserMap();
        AdjustCamera();
        CreatePlayerUnits();
        PositionMinimapCamera();
    }
    void Start()
    {
        
    }

    void CalculateTileSize()
    {
        float hexWidth = Mathf.Sqrt(3); // 타일 크기가 1일 때의 헥사곤 타일 폭
        float totalHexWidth = hexWidth * width + hexWidth / 2f; // 타일 크기가 1일 때의 전체 맵 가로 크기

        tileSize = desiredMapWidth / totalHexWidth; // 원하는 맵 가로 크기에 맞게 tileSize 계산

        float rectWidthRatio = 0.90f; // 직사각형 타일 폭의 비율 (예: 80%)
        rectWidthSize = (Mathf.Sqrt(3) * tileSize) * rectWidthRatio; // 직사각형 타일의 폭
    }

    void PositionMinimapCamera()
    {
        float centerX = mapSpacing;
        float centerZ = mapSpacing;
        float cameraHeight = 50f; // 미니맵에 적절한 높이로 설정합니다.

        if (minimapCamera != null)
        {
            minimapCamera.transform.position = new Vector3(centerX, cameraHeight, centerZ);
            minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // 아래를 향하도록 설정
        }
        else
        {
            Debug.LogWarning("미니맵 카메라가 할당되지 않았습니다.");
        }
    }

    public class MapInfo
    {
        public int mapId;
        public Transform mapTransform;
        public Bounds mapBounds;
        public LineRenderer boundaryLineRenderer; // 경계선 LineRenderer 참조 추가
        public PlayerData playerData; // PlayerData 추가
    }

    public List<MapInfo> mapInfos = new List<MapInfo>();


    void CreatUserMap()
    {
        int totalUsers = 8; // 총 유저 수 (플레이어 포함)
        //int gridSize = 3; // 3x3 격자 (가운데는 비움)
        //float mapSpacing = 40f; // 각 맵 간의 거리
        PlayerData[] allPlayers = stageManager.allPlayers;
        // 3x3 격자에서 (1,1)은 비워두고, 나머지 8개의 칸에 맵을 배치
        int[,] gridPositions = new int[,]
        {
        {0, 0}, {0, 1}, {0, 2},
        {1, 0},        {1, 2},
        {2, 0}, {2, 1}, {2, 2}
        };

        for (int i = 0; i < totalUsers; i++)
        {
            // 격자의 (x, z) 위치 계산
            int gridX = gridPositions[i, 0];
            int gridZ = gridPositions[i, 1];

            // 각 유저의 맵 위치 계산
            Vector3 mapPosition = new Vector3(gridX * mapSpacing, 0, gridZ * mapSpacing);

            // 맵을 생성하고 해당 위치에 배치
            GameObject userMap = new GameObject($"User_{i}_Map");
            userMap.transform.position = mapPosition;
            userMap.transform.SetParent(this.transform);
            userMap.AddComponent<GoldDisplay>();
            

            GenerateMap(userMap.transform);

            //각 유저들 맵정보 저장
            MapInfo mapInfo = new MapInfo();
            mapInfo.mapId = i;
            mapInfo.mapTransform = userMap.transform;

            // 맵의 경계 영역 계산 (맵의 크기를 알고 있어야 함)
            float mapWidth = desiredMapWidth;
            float mapHeight = mapHeightSize + rectWidthSize * 2; // 육각형 타일 높이 + 직사각형 타일 높이

            Vector3 center = mapPosition;
            Vector3 size = new Vector3(mapWidth, 0, mapHeight);

            // 만약 경계선 크기를 반영하려면
            size.x += boundaryExtraWidth;
            size.z += boundaryExtraHeight;

            mapInfo.mapBounds = new Bounds(center, size);
            mapInfo.playerData = allPlayers[i];
            mapInfos.Add(mapInfo);

            CreateMapBoundary(userMap.transform, i, mapInfo);
        }
    }



    void GenerateMap(Transform parent)
    {
        float hexWidth = Mathf.Sqrt(3) * tileSize;
        float hexHeight = tileSize * 2f; // 헥사곤의 높이

        // 맵의 전체 크기 계산
        mapWidthSize = hexWidth * width - hexWidth / 2f;
        mapHeightSize = hexHeight * (height - 1) * 0.75f;

        // 그리드의 중앙을 기준으로 오프셋 계산
        float xOffset = mapWidthSize / 2f - hexWidth / 2f;
        float zOffset = mapHeightSize / 2f;

        // 헥사곤 타일 생성
        for (int r = 0; r < height; r++)
        {
            int numCols = width;

            for (int q = 0; q < numCols; q++)
            {
                float xPos = q * hexWidth - (r % 2) * (hexWidth / 2f) - xOffset;
                float zPos = r * (hexHeight * 0.75f) - zOffset;

                // 부모 오브젝트의 위치를 기준으로 위치 조정
                Vector3 position = parent.position + new Vector3(xPos, 0, zPos);

                CreateHexTile(position, q, r, parent);
            }
        }

        // 맨 위와 맨 아래에 직사각형 타일 생성 (위치 조정 포함)
        CreateRectangularRow(-1, -hexHeight * 0.75f - zOffset, parent);
        CreateRectangularRow(height, hexHeight * 0.75f * (height) - zOffset, parent);
        CreateItemTiles(parent);
        CreateGoldTiles(parent);

        //gameObject.transform.SetParent(User.transform);
    }

    void CreateHexTile(Vector3 position, int q, int r, Transform parent)
    {
        GameObject tile = Instantiate(hexTilePrefab, position, Quaternion.identity, parent);
        tile.name = $"Hex_{r}_{q}";
        if(r <= 3)
        {
            tile.layer = LayerMask.NameToLayer("PlayerTile");
        }

        HexTile hexTile = tile.GetComponent<HexTile>();
        hexTile.q = q;
        hexTile.r = r;
        hexTile.s = -q - r;
    }

    void CreateRectangularRow(int row, float zPos, Transform parent)
    {
        // zPos에 부모의 z 위치를 더합니다.
        zPos += parent.position.z;

        float rectRowWidth = rectWidthSize * rectWidth;
        float xOffset = rectRowWidth / 2f - rectWidthSize / 2f;

        float zOffset = (tileSize * 2f * 0.75f) / 2f;

        if (row == -1)
            zPos -= zOffset + gapBetweenTiles;
        else if (row == height)
            zPos += zOffset + gapBetweenTiles;

        for (int x = 0; x < rectWidth; x++)
        {
            float xPos = x * rectWidthSize - xOffset;
            // xPos는 parent.position.x와 합산됩니다.
            Vector3 position = new Vector3(xPos + parent.position.x, 0, zPos);

            GameObject tile = Instantiate(rectTilePrefab, position, Quaternion.identity, parent);
            tile.name = $"Rect_{x}_{row}";
            if (row == -1)
            {
                tile.layer = LayerMask.NameToLayer("PlayerTile");
            }
            HexTile hexTile = tile.GetComponent<HexTile>();
            hexTile.isRectangularTile = true;
        }
    }

    void AdjustCamera()
    {
        float hexWidth = Mathf.Sqrt(3) * tileSize;
        float hexHeight = tileSize * 2f;

        // 맵의 전체 크기 계산
        mapWidthSize = Mathf.Max(hexWidth * width + hexWidth / 2f, rectWidthSize * rectWidth);
        mapHeightSize = hexHeight * (height - 1) * 0.75f + hexHeight * 0.75f * 2f;

        // 카메라의 중심 위치 계산
        Vector3 centerPosition = new Vector3(0, 0, 0);

        // 카메라 위치 설정
        float cameraHeight = Mathf.Max(mapWidthSize, mapHeightSize); // 맵 크기에 따라 카메라 높이 조정
        Camera.main.transform.position = centerPosition + new Vector3(0, 18f, -21f);
        Camera.main.transform.rotation = Quaternion.Euler(45f, 0f, 0f);

        // 카메라 투영 방식 설정
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = cameraHeight / 1.8f;

        // 카메라 클리핑 플레인 설정
        Camera.main.nearClipPlane = -10f;
        Camera.main.farClipPlane = 100f;
    }

    void CreatePlayerUnits()
    {
        for (int x = 0; x < rectWidth; x++)
        {
            GameObject unitObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            unitObj.name = $"Unit_{x}";
            unitObj.tag = "Moveable";
            unitObj.transform.localScale = new Vector3(0.5f, 1f, 0.5f);

            Unit unit = unitObj.AddComponent<Unit>();
            //unitObj.AddComponent<UnitMove>();

            // 맨 아래 직사각형 타일을 가져옵니다.
            HexTile tile = GameObject.Find($"Rect_{x}_-1").GetComponent<HexTile>();

            unit.PlaceOnTile(tile);
        }
    }

    /*void CreateItemTile(Vector3 position, int cornerId)
    {
        GameObject tile = Instantiate(itemTilePrefab, position, Quaternion.identity, this.transform);
        tile.name = $"ItemTile_{cornerId}";
        if (cornerId == 1)
        {
            tile.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        GameObject item1 = Instantiate(itemPrefeb, gameObject.GetComponent<ItemHandler>()._items[0].transform.position, 
            Quaternion.identity, this.transform);
    }*/

    void CreateItemTiles(Transform parent)
    {
        float cornerTileOffset = rectWidthSize * 0.6f;
        float adjustedTileOffset = rectWidthSize;

        Vector3 bottomLeftPos = parent.position + new Vector3(-mapWidthSize / 2f - cornerTileOffset - 0.5f, 0, -mapHeightSize / 2f - cornerTileOffset - 0.3f);
        Vector3 topRightPos = parent.position + new Vector3(mapWidthSize / 2f + cornerTileOffset + 0.5f, 0, mapHeightSize / 2f + cornerTileOffset + 0.3f);

        for (int i = 0; i < 2; i++)
        {
            if (i == 0)
            {
                GameObject tile = Instantiate(itemTilePrefab, bottomLeftPos, Quaternion.identity, parent);
                tile.name = $"ItemTile_{0}";
            }
            else
            {
                GameObject tile1 = Instantiate(itemTilePrefab, topRightPos, Quaternion.identity, parent);
                tile1.name = $"ItemTile_{1}";
                tile1.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
        }
    }

    void CreateGoldTiles(Transform parent)
    {
        float xOffset = mapWidthSize / 2f + goldSlotSize / 2f + 2.3f;
        float xLeft = parent.position.x - xOffset;
        float xRight = parent.position.x + xOffset;

        goldSlotSpacing = 1f;
        float extraSpacing = 0.2f;

        float totalGoldSlotsHeight = maxGoldSlots * goldSlotSize + (maxGoldSlots - 1) * (goldSlotSpacing + extraSpacing);
        float startZ = parent.position.z - totalGoldSlotsHeight / 2f + goldSlotSize / 2f;

        for (int i = 0; i < maxGoldSlots; i++)
        {
            float zPos = startZ + i * (goldSlotSize + goldSlotSpacing);

            Vector3 leftPos = new Vector3(xLeft, 0, zPos);
            GameObject PlayerTile = Instantiate(goldTilePrefeb, leftPos, goldTilePrefeb.transform.rotation, parent);
            PlayerTile.name = $"PlayerGoldTile_{i}";
            GameObject PlayerGold = Instantiate(goldPrefeb, PlayerTile.transform.position, transform.rotation, this.transform);
            PlayerGold.name = $"PlayerGold_{i}";
            PlayerGold.transform.SetParent(PlayerTile.transform);

            Vector3 rightPos = new Vector3(xRight, 0, zPos);
            GameObject EnemyTile = Instantiate(goldTilePrefeb, rightPos, goldTilePrefeb.transform.rotation, parent);
            EnemyTile.name = $"EnemyGoldTile_{i}";
            GameObject EnemyGold = Instantiate(goldPrefeb, EnemyTile.transform.position, transform.rotation, this.transform);
            EnemyGold.name = $"EnemyGold_{i}";
            EnemyGold.transform.SetParent(EnemyTile.transform);
        }
    }

    void CreateMapBoundary(Transform parent, int mapId, MapInfo mapInfo)
    {
        //float boundaryExtraWidth = 10f;  // 가로 방향으로 추가할 길이
        //float boundaryExtraHeight = 10f; // 세로 방향으로 추가할 길이

        // 맵의 크기 계산
        float mapWidth = desiredMapWidth + boundaryExtraWidth;
        float mapHeight = mapHeightSize + rectWidthSize * 2f + boundaryExtraHeight;

        // 경계선을 그릴 좌표 계산
        Vector3[] corners = new Vector3[5];
        corners[0] = parent.position + new Vector3(-mapWidth / 2f, 0.1f, -mapHeight / 2f);
        corners[1] = parent.position + new Vector3(-mapWidth / 2f, 0.1f, mapHeight / 2f);
        corners[2] = parent.position + new Vector3(mapWidth / 2f, 0.1f, mapHeight / 2f);
        corners[3] = parent.position + new Vector3(mapWidth / 2f, 0.1f, -mapHeight / 2f);
        corners[4] = corners[0]; // 시작점으로 돌아와 사각형 완성

        // 경계선 오브젝트 생성
        GameObject boundaryObj = new GameObject($"MapBoundary_{mapId}");
        boundaryObj.transform.SetParent(parent);
        LineRenderer lineRenderer = boundaryObj.AddComponent<LineRenderer>();

        // LineRenderer 설정
        lineRenderer.positionCount = corners.Length;
        lineRenderer.SetPositions(corners);
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;

        // LineRenderer 설정 후
        mapInfo.boundaryLineRenderer = lineRenderer; // LineRenderer 참조 저장


        // 재질 및 색상 설정
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;

        // 레이어 설정
        int minimapLayer = LayerMask.NameToLayer("Minimap");
        boundaryObj.layer = minimapLayer;
        foreach (Transform child in boundaryObj.transform)
        {
            child.gameObject.layer = minimapLayer;
        }
    }
}
