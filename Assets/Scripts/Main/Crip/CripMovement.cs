using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class CripMovement : MonoBehaviour
{
    public float moveInterval = 2f; // 이동 간격
    private float moveTimer;
    private MapInfo playerMapInfo;
    public HexTile currentTile;

    void Start()
    {
        moveTimer = moveInterval;

        // 크립이 속한 맵 정보를 가져옵니다.
        playerMapInfo = GetComponentInParent<HexTile>()?.transform.parent.GetComponentInParent<MapInfo>();

        currentTile = GetCurrentTile();
    }

    void Update()
    {
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            MoveRandomly();
            moveTimer = moveInterval;
        }
    }

    void MoveRandomly()
    {
        if (playerMapInfo == null)
            return;

        // 모든 비어있는 타일 가져오기
        List<HexTile> unoccupiedTiles = new List<HexTile>();
        foreach (var tileEntry in playerMapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            if (!tile.isOccupied)
            {
                unoccupiedTiles.Add(tile);
            }
        }

        if (unoccupiedTiles.Count > 0)
        {
            // 랜덤 타일 선택
            HexTile nextTile = unoccupiedTiles[Random.Range(0, unoccupiedTiles.Count)];

            // 현재 타일 상태 업데이트
            if (currentTile != null)
            {
                currentTile.isOccupied = false;
                currentTile.itemOnTile = null;
            }

            // 이동
            transform.position = nextTile.transform.position;

            // 새로운 타일 상태 업데이트
            nextTile.isOccupied = true;
            nextTile.itemOnTile = this.gameObject;
            currentTile = nextTile;

            // 부모를 새로운 타일로 설정
            transform.SetParent(nextTile.transform);
        }
    }

    HexTile GetCurrentTile()
    {
        if (playerMapInfo == null)
            return null;

        // 현재 위치에서 가장 가까운 타일 찾기
        float minDistance = float.MaxValue;
        HexTile nearestTile = null;
        foreach (var tileEntry in playerMapInfo.HexDictionary)
        {
            HexTile tile = tileEntry.Value;
            float dist = Vector3.Distance(transform.position, tile.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearestTile = tile;
            }
        }
        return nearestTile;
    }
}
