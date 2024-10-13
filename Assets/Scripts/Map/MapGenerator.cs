using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public GameObject goldPrefeb;

    public float gapBetweenTiles = 0.1f; // 타일 간격 조정용 변수

    private float rectWidthSize; // 사각형 타일 폭

    public int maxGoldSlots = 5; // 최대 골드 표시 칸 수
    public float goldSlotSize = 1f; // 골드 표시 칸의 크기
    public float goldSlotSpacing = 0.1f; // 골드 표시 칸 간의 간격

    private float mapWidthSize;
    private float mapHeightSize;

    private void Awake()
    {
        CalculateTileSize();
        GenerateMap();
        CreateItemTiles();
        AdjustCamera();
        CreatePlayerUnits();
        CreateGoldTiles();
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

    void GenerateMap()
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
            int numCols = width; // 모든 행에서 타일 개수를 동일하게 설정

            for (int q = 0; q < numCols; q++)
            {
                float xPos = q * hexWidth - (r % 2) * (hexWidth / 2f) - xOffset;
                float zPos = r * (hexHeight * 0.75f) - zOffset;

                Vector3 position = new Vector3(xPos, 0, zPos);

                CreateHexTile(position, q, r);
            }
        }

        // 맨 위와 맨 아래에 직사각형 타일 생성
        CreateRectangularRow(-1, -hexHeight * 0.75f - zOffset);
        CreateRectangularRow(height, hexHeight * 0.75f * (height) - zOffset);
    }

    void CreateHexTile(Vector3 position, int q, int r)
    {
        GameObject tile = Instantiate(hexTilePrefab, position, Quaternion.identity, this.transform);
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

    void CreateRectangularRow(int row, float zPos)
    {
        float rectRowWidth = rectWidthSize * rectWidth;
        float xOffset = rectRowWidth / 2f - rectWidthSize / 2f;

        float zOffset = (tileSize * 2f * 0.75f) / 2f; // 육각형 타일 높이의 절반

        if (row == -1)
            zPos -= zOffset + gapBetweenTiles;
        else if (row == height)
            zPos += zOffset + gapBetweenTiles;

        for (int x = 0; x < rectWidth; x++)
        {
            float xPos = x * rectWidthSize - xOffset;

            Vector3 position = new Vector3(xPos, 0, zPos);

            GameObject tile = Instantiate(rectTilePrefab, position, Quaternion.identity, this.transform);
            tile.name = $"Rect_{x}_{row}";
            if(row == -1)
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
    void CreateItemTile(Vector3 position, int cornerId)
    {
        GameObject tile = Instantiate(itemTilePrefab, position, Quaternion.identity, this.transform);
        tile.name = $"ItemTile_{cornerId}";
        if (cornerId == 1)
        {
            tile.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        /*GameObject item1 = Instantiate(itemPrefeb, gameObject.GetComponent<ItemHandler>()._items[0].transform.position, 
            Quaternion.identity, this.transform);*/
    }
    void CreateItemTiles()
    {
        float cornerTileOffset = rectWidthSize * 0.6f; // 맵 가장자리에서 약간 떨어뜨리기 위한 오프셋
        float adjustedTileOffset = rectWidthSize; // 간격을 고려한 타일 오프셋

        Vector3 bottomLeftPos = new Vector3(-mapWidthSize / 2f - cornerTileOffset - 0.5f, 0, -mapHeightSize / 2f - cornerTileOffset - 0.3f);
        CreateItemTile(bottomLeftPos, 0);

        Vector3 topRightPos = new Vector3(mapWidthSize / 2f + cornerTileOffset + 0.5f, 0, mapHeightSize / 2f + cornerTileOffset + 0.3f);
        CreateItemTile(topRightPos, 1);
    }
    void CreateGoldTiles()
    {
        // 왼쪽과 오른쪽의 x 위치 계산
        float xOffset = mapWidthSize / 2f + goldSlotSize / 2f + 0.5f;
        float xLeft = -xOffset;
        float xRight = xOffset;

        // 골드 타일을 수직으로 중앙에 배치하기 위한 시작 z 위치 계산
        goldSlotSpacing = 1f; // 간격을 늘림
        float extraSpacing = 0.2f; // 추가 간격 설정

        float totalGoldSlotsHeight = maxGoldSlots * goldSlotSize + (maxGoldSlots - 1) * (goldSlotSpacing + extraSpacing);
        float startZ = -totalGoldSlotsHeight / 2f + goldSlotSize / 2f;

        for (int i = 0; i < maxGoldSlots; i++)
        {
            float zPos = startZ + i * (goldSlotSize + goldSlotSpacing);

            // 왼쪽 골드 타일 생성
            Vector3 leftPos = new Vector3(xLeft, 0, zPos);
            GameObject PlayerTile = Instantiate(goldTilePrefeb, leftPos, goldTilePrefeb.transform.rotation, this.transform);
            PlayerTile.name = $"PlayerGoldTile_{i}";
            GameObject PlayerGold = Instantiate(goldPrefeb, PlayerTile.transform.position, transform.rotation, this.transform);
            PlayerGold.name = $"PlayerGold_{i}";
            PlayerGold.transform.SetParent(PlayerTile.transform);
            
            // 오른쪽 골드 타일 생성
            Vector3 rightPos = new Vector3(xRight, 0, zPos);
            GameObject EnemyTile = Instantiate(goldTilePrefeb, rightPos, goldTilePrefeb.transform.rotation, this.transform);
            EnemyTile.name = $"EnemyGoldTile_{i}";
            GameObject EnemyGold = Instantiate(goldPrefeb, EnemyTile.transform.position, transform.rotation, this.transform);
            EnemyGold.name = $"EnemyGold_{i}";
            EnemyGold.transform.SetParent(EnemyTile.transform);
        }
    }
}
