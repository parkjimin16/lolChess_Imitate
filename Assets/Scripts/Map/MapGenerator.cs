using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private int width;
    private int height;
    private int rectWidth; // 직사각형 타일의 개수
    private float desiredMapWidth; // 원하는 맵의 가로 크기 (단위: 유니티 월드 좌표)

    private float tileSize; // 타일의 크기 (자동 계산될 예정)

    private GameObject hexTilePrefab; // 헥사곤 타일 프리팹
    private GameObject rectTilePrefab; // 직사각형 타일 프리팹
    private GameObject itemTilePrefab; // 아이템 타일 프리팹
    private GameObject goldTilePrefeb; // 골드 타일 프리팹
    private GameObject goldPrefeb; // 골드 표시 프리팹
    public Camera minimapCamera;

    private GameObject[] allPlayers;

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

    private GameObject championPrefab; // 챔피언 프리팹
    private GameObject playerPrefab;   // 플레이어 프리팹

    public List<GameObject> championsInCarousel = new List<GameObject>();
    //[SerializeField] List<GameObject> hexTiles = new List<GameObject>();

    public Transform sharedSelectionMapTransform; // 공동 선택 맵의 Transform 참조
    public MapInfo sharedMapInfo; // 공동 선택 맵의 MapInfo

    private Vector3 storedLeftPosition;
    private Vector3 storedRightPosition;

    public void InitMapGenerator(GameDataBlueprint gdb)
    {
        hexTilePrefab = gdb.HexTilePrefab;
        rectTilePrefab = gdb.RectTilePrefab;
        itemTilePrefab = gdb.ItemTilePrefab;
        goldTilePrefeb = gdb.GoldTilePrefab;
        goldPrefeb = gdb.GoldPrefab;

        width = gdb.Width;
        height = gdb.Height;
        rectWidth = gdb.RectWidth;
        desiredMapWidth = gdb.DesiredMapWidth;

        championPrefab = gdb.ChampionPrefeb; // GameDataBlueprint에서 챔피언 프리팹 할당
        playerPrefab = gdb.PlayerPrefeb;     // GameDataBlueprint에서 플레이어 프리팹 할당

        CalculateTileSize();
        CreatUserMap();
        AdjustCamera();
        PositionMinimapCamera();

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
        public Player playerData; // PlayerData 추가
        public float minX;
        public float maxX;
        public float minZ;
        public float maxZ;

        public Dictionary<(int, int), HexTile> HexDictionary = new Dictionary<(int, int), HexTile>();
        public Dictionary<(int, int), HexTile> RectDictionary = new Dictionary<(int, int), HexTile>();
        public List<Transform> SugarcraftPosition = new List<Transform>();
        public List<Transform> PortalPosition = new List<Transform>();

        public List<ItemTile> ItemTile = new List<ItemTile>();

    }

    public List<MapInfo> mapInfos = new List<MapInfo>();


    void CreatUserMap()
    {
        //int totalUsers = 8; // 총 유저 수 (플레이어 포함)
        allPlayers = Manager.Game.PlayerListObject;

        if (allPlayers.Length <= 0)
            Debug.Log("AllPlayers is Null");

        // 3x3 격자에서 (1,1)은 공동 선택 맵으로 사용하고, 나머지 8개의 칸에 맵을 배치
        int[,] gridPositions = new int[,]
        {
        {0, 0}, {0, 1}, {0, 2},
        {1, 0},        {1, 2},
        {2, 0}, {2, 1}, {2, 2}
        };

        int userIndex = 0;

        for (int i = 0; i < 3; i++) // gridPositions의 행
        {
            for (int j = 0; j < 3; j++) // gridPositions의 열
            {
                if (i == 1 && j == 1)
                {
                    // 공동 선택 맵 생성
                    Vector3 sharedMapPosition = new Vector3(i * mapSpacing, 0, j * mapSpacing);
                    CreateSharedSelectionMap(sharedMapPosition);
                }
                else
                {
                    // 각 유저의 맵 위치 계산
                    Vector3 mapPosition = new Vector3(i * mapSpacing, 0, j * mapSpacing);

                    // 맵을 생성하고 해당 위치에 배치
                    GameObject userMap = new GameObject($"User_{userIndex}_Map");
                    userMap.transform.position = mapPosition;
                    userMap.transform.SetParent(this.transform);
                    userMap.AddComponent<GoldDisplay>();
                    userMap.AddComponent<RectTile>();
                    Vector3 PlayerPosition = userMap.transform.position + new Vector3(-13.5f, 0.8f, -5f);
                    allPlayers[userIndex].transform.position = PlayerPosition;

                    GameObject shop = GameObject.Find("ShopPanel");
                    UIShopPanel uIShop = shop.GetComponent<UIShopPanel>();
                    if (userIndex == 0) // 유저 정보 받아와서 수정하기
                    {
                        uIShop.SetChampionPos(userMap.GetComponent<RectTile>());
                    }

                    // 각 유저들 맵정보 저장
                    MapInfo mapInfo = new MapInfo();
                    mapInfo.mapId = userIndex;
                    mapInfo.mapTransform = userMap.transform;

                    // 맵의 경계 영역 계산
                    float mapWidth = desiredMapWidth;
                    float mapHeight = mapHeightSize + rectWidthSize * 2;

                    Vector3 center = mapPosition;
                    Vector3 size = new Vector3(mapWidth, 0, mapHeight);

                    // 경계선 크기 반영
                    size.x += boundaryExtraWidth;
                    size.z += boundaryExtraHeight;

                    mapInfo.mapBounds = new Bounds(center, size);
                    mapInfo.playerData = allPlayers[userIndex].GetComponent<Player>();
                    
                    mapInfos.Add(mapInfo);

                    GenerateMap(userMap.transform, mapInfo);
                    CreateMapBoundary(userMap.transform, userIndex, mapInfo);

                    userIndex++;
                }
            }
        }
    }



    void GenerateMap(Transform parent, MapInfo mapInfo)
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

                int adjustedQ = q - (r / 2); // q 좌표 조정

                CreateHexTile(position, q, r, parent, mapInfo);
            }
        }

        // 맨 위와 맨 아래에 직사각형 타일 생성 (위치 조정 포함)
        CreateRectTile(-1, -hexHeight * 0.75f - zOffset, parent, mapInfo);
        CreateRectTile(height, hexHeight * 0.75f * (height) - zOffset, parent, mapInfo);
        CreateItemTiles(parent, mapInfo);
        CreateGoldTiles(parent);

        //gameObject.transform.SetParent(User.transform);
    }

    void CreateHexTile(Vector3 position, int q, int r, Transform parent, MapInfo mapInfo)
    {
        GameObject tile = Instantiate(hexTilePrefab, position, Quaternion.identity, parent);
        tile.name = $"Hex_{q}_{r}";

        if (r <= 3)
        {
            tile.layer = LayerMask.NameToLayer("PlayerTile");
            tile.tag = "PlayerTile";
        }
        else
        {
            tile.tag = "EnemyTile";
        }

        HexTile hexTile = tile.GetComponent<HexTile>();
        hexTile.q = q;
        hexTile.r = r;
        hexTile.s = -q - r;

        //타일을 딕셔너리에 추가
        mapInfo.HexDictionary.Add((q, r), hexTile);
        
    }

    void CreateRectTile(int row, float zPos, Transform parent, MapInfo mapInfo)
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
                tile.tag = "PlayerTile";
                //hexTiles.Add(tile);
                //Debug.Log(hexTiles[x].transform.position);
            }
            
            HexTile hexTile = tile.GetComponent<HexTile>();
            hexTile.isRectangularTile = true;
            //hexTile.isOccupied = false;

            mapInfo.RectDictionary.Add((x, row), hexTile);
        }

        // 빈 오브젝트 생성 함수 호출
        CreatSynergyPosition(row, zPos, parent, xOffset, mapInfo);
        
    }

    void CreatSynergyPosition(int row, float zPos, Transform parent, float xOffset, MapInfo mapInfo)
    {
        Vector3 leftPosition, rightPosition;

        if (row == -1)
        {
            // row가 -1인 경우의 위치를 계산하고 저장합니다.
            float leftXPos = (-1) * rectWidthSize - xOffset + parent.position.x - 2f;
            leftPosition = new Vector3(leftXPos, 0, zPos + 6f);

            float rightXPos = (rectWidth) * rectWidthSize - xOffset + parent.position.x + 0.5f;
            rightPosition = new Vector3(rightXPos, 0, zPos + 4f);

            // 위치를 저장합니다.
            storedLeftPosition = leftPosition;
            storedRightPosition = rightPosition;

        }
        else if (row == height)
        {
            // 저장된 위치를 사용하여 대칭 위치를 계산합니다.
            leftPosition = new Vector3(
                2 * parent.position.x - storedLeftPosition.x,
                storedLeftPosition.y,
                2 * parent.position.z - storedLeftPosition.z
            );

            rightPosition = new Vector3(
                2 * parent.position.x - storedRightPosition.x,
                storedRightPosition.y,
                2 * parent.position.z - storedRightPosition.z
            );
        }
        else
        {
            // 일반적인 경우의 위치 계산
            float leftXPos = (-1) * rectWidthSize - xOffset + parent.position.x - 2f;
            leftPosition = new Vector3(leftXPos, 0, zPos);

            float rightXPos = (rectWidth) * rectWidthSize - xOffset + parent.position.x + 2f;
            rightPosition = new Vector3(rightXPos, 0, zPos);
        }

        // 빈 오브젝트 생성
        GameObject leftEmptyObject = new GameObject($"Empty_Left_{row}");
        leftEmptyObject.transform.position = leftPosition;
        leftEmptyObject.transform.SetParent(parent);
        mapInfo.PortalPosition.Add(leftEmptyObject.transform);

        GameObject rightEmptyObject = new GameObject($"Empty_Right_{row}");
        rightEmptyObject.transform.position = rightPosition;
        rightEmptyObject.transform.SetParent(parent);
        mapInfo.SugarcraftPosition.Add(rightEmptyObject.transform);

        if (row == -1)
        {
            // 플레이어의 오브젝트
            leftEmptyObject.tag = "PlayerPortal";
            rightEmptyObject.tag = "PlayerSugar";
        }
        else if (row == height)
        {
            // 적의 오브젝트
            leftEmptyObject.tag = "EnemyPortal";
            rightEmptyObject.tag = "EnemySugar";
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
        Camera.main.transform.position = centerPosition + new Vector3(0, 18f, -26f);
        Camera.main.transform.rotation = Quaternion.Euler(40f, 0f, 0f);

        // 카메라 투영 방식 설정
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = cameraHeight / 1.8f;

        // 카메라 클리핑 플레인 설정
        Camera.main.nearClipPlane = -10f;
        Camera.main.farClipPlane = 100f;
    }

    void CreateItemTiles(Transform parent, MapInfo mapinfo)
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
                ItemTile itemTile = tile.GetComponent<ItemTile>();
                itemTile.TileType1 = ItemOwner.Player;
                mapinfo.ItemTile.Add(itemTile);
            }
            else
            {
                GameObject tile1 = Instantiate(itemTilePrefab, topRightPos, Quaternion.identity, parent);
                tile1.name = $"ItemTile_{1}";
                tile1.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                ItemTile itemTile = tile1.GetComponent<ItemTile>();
                itemTile.TileType1 = ItemOwner.Another;
                mapinfo.ItemTile.Add(itemTile);
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

        float minX = Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
        float maxX = Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
        float minZ = Mathf.Min(corners[0].z, corners[1].z, corners[2].z, corners[3].z);
        float maxZ = Mathf.Max(corners[0].z, corners[1].z, corners[2].z, corners[3].z);

        // MapInfo에 경계선 좌표를 저장합니다.
        mapInfo.minX = minX;
        mapInfo.maxX = maxX;
        mapInfo.minZ = minZ;
        mapInfo.maxZ = maxZ;

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

    void CreateSharedSelectionMap(Vector3 position)
    {
        // 공동 선택 맵 오브젝트 생성
        GameObject sharedMap = new GameObject("SharedSelectionMap");
        sharedMap.transform.position = position;
        sharedMap.transform.SetParent(this.transform);

        // 공동 선택 맵의 Transform을 저장
        sharedSelectionMapTransform = sharedMap.transform;

        // 공동 선택 맵의 MapInfo 생성 및 설정
        sharedMapInfo = new MapInfo();
        sharedMapInfo.mapTransform = sharedMap.transform;

        // 경계선 생성
        CreateSharedMapBoundary(sharedMap.transform, sharedMapInfo);

        // 회전 축 오브젝트 생성 및 챔피언 배치
        GameObject carouselPivot = new GameObject("CarouselPivot");
        carouselPivot.transform.position = position;
        carouselPivot.transform.SetParent(sharedMap.transform);

        // 회전 스크립트 추가
        CarouselRotation carouselRotation = carouselPivot.AddComponent<CarouselRotation>();
        carouselRotation.rotationSpeed = -20f; // 원하는 회전 속도 설정

        // 챔피언 배치 (carouselPivot을 부모로 설정)
        PlaceChampionsInSharedMap(carouselPivot.transform);

        // 플레이어 배치 함수는 수정하여 플레이어들을 이동시키도록 변경
        // PlacePlayersInSharedMap(sharedMap.transform);
    }
    void CreateSharedMapBoundary(Transform parent, MapInfo mapInfo)
    {
        float boundaryRadius = 10f; // 경계선의 반지름
        int segments = 100;         // 원을 그리기 위한 세그먼트 수

        GameObject boundaryObj = new GameObject("SharedMapBoundary");
        boundaryObj.transform.SetParent(parent);
        LineRenderer lineRenderer = boundaryObj.AddComponent<LineRenderer>();

        lineRenderer.positionCount = segments + 1;
        lineRenderer.startWidth = 0.8f;
        lineRenderer.endWidth = 0.8f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;

        // LineRenderer 설정
        Vector3[] positions = new Vector3[segments + 1];
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * 2 * Mathf.PI / segments;
            float x = boundaryRadius * Mathf.Cos(angle);
            float z = boundaryRadius * Mathf.Sin(angle);
            positions[i] = new Vector3(x, 0.1f, z) + parent.position;

            // 경계 계산
            minX = Mathf.Min(minX, positions[i].x);
            maxX = Mathf.Max(maxX, positions[i].x);
            minZ = Mathf.Min(minZ, positions[i].z);
            maxZ = Mathf.Max(maxZ, positions[i].z);
        }
        lineRenderer.SetPositions(positions);

        // 재질 및 색상 설정
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.yellow;

        // MapInfo에 경계 정보 저장
        mapInfo.minX = minX;
        mapInfo.maxX = maxX;
        mapInfo.minZ = minZ;
        mapInfo.maxZ = maxZ;

        int minimapLayer = LayerMask.NameToLayer("Minimap");
        boundaryObj.layer = minimapLayer;
        foreach (Transform child in boundaryObj.transform)
        {
            child.gameObject.layer = minimapLayer;
        }
    }
    void PlaceChampionsInSharedMap(Transform parent)
    {
        float circleRadius = 5f; // 챔피언들이 배치될 원의 반지름
        int championCount = 9;

        for (int i = 0; i < championCount; i++)
        {
            float angle = i * 2 * Mathf.PI / championCount;
            float x = circleRadius * Mathf.Cos(angle);
            float z = circleRadius * Mathf.Sin(angle);
            Vector3 position = new Vector3(x, 0, z) + parent.position;

            // 챔피언 프리팹을 해당 위치에 생성하고, 회전 축을 부모로 설정
            GameObject champion = Instantiate(championPrefab, position, Quaternion.identity, parent);
            champion.name = $"Champion_{i}";
            champion.tag = "Champion";

            championsInCarousel.Add(champion);

            // 이동 방향(접선 방향) 계산
            Vector3 tangent = new Vector3(-Mathf.Sin(angle), 0, Mathf.Cos(angle));

            // 챔피언의 회전을 이동 방향으로 설정
            champion.transform.localRotation = Quaternion.LookRotation(tangent);
        }
    }
    public void PlacePlayersInSharedMap(Transform parent)
    {
        int totalPlayers = allPlayers.Length;
        float spawnRadius = 8f; // 플레이어들이 스폰될 원의 반지름

        for (int i = 0; i < totalPlayers; i++)
        {
            float angle = i * 2 * Mathf.PI / totalPlayers;
            float x = spawnRadius * Mathf.Cos(angle);
            float z = spawnRadius * Mathf.Sin(angle);
            Vector3 position = new Vector3(x, 0, z) + parent.position;

            // 기존 플레이어 오브젝트를 가져옵니다.
            GameObject player = allPlayers[i];

            // 플레이어의 원래 위치를 저장합니다.
            PlayerMove playerMove = player.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                playerMove.SaveOriginalPosition();
            }

            // 플레이어를 해당 위치로 이동시킵니다.
            player.transform.position = position;

            // 플레이어의 부모를 공동 선택 맵으로 설정하여 위치를 맞춥니다.
            player.transform.SetParent(parent);

            // 필요에 따라 플레이어의 회전을 조정합니다.
            player.transform.LookAt(parent.position);
        }
    }
    public List<GameObject> GetAvailableChampions()
    {
        List<GameObject> availableChampions = new List<GameObject>();

        foreach (GameObject champion in championsInCarousel)
        {
            if (champion != null)
            {
                availableChampions.Add(champion);
            }
        }

        return availableChampions;
    }
    public void RemoveChampion(GameObject champion)
    {
        championsInCarousel.Remove(champion);
        Destroy(champion);
    }

}