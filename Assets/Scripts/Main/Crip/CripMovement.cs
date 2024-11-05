using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CripMovement : MonoBehaviour
{
    public float moveInterval = 2f; // 이동 간격
    private float moveTimer;
    private MapGenerator mapGenerator;
    public HexTile currentTile;

    void Start()
    {
        moveTimer = moveInterval;
        mapGenerator = FindObjectOfType<MapGenerator>();
        //currentTile = GetCurrentTile();
    }

    void Update()
    {
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            //MoveRandomly();
            moveTimer = moveInterval;
        }
    }

    /*void MoveRandomly()
    {
        // 모든 타일 가져오기
        //List<HexTile> allTiles = mapGenerator.GetAllTiles();

        // 비어있는 타일 목록 생성
        List<HexTile> unoccupiedTiles = new List<HexTile>();
        foreach (HexTile tile in allTiles)
        {
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
        }
    }*/

    /*HexTile GetCurrentTile()
    {
        // 현재 위치에서 가장 가까운 타일 찾기
        float minDistance = float.MaxValue;
        HexTile nearestTile = null;
        foreach (HexTile tile in mapGenerator.GetAllTiles())
        {
            float dist = Vector3.Distance(transform.position, tile.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearestTile = tile;
            }
        }
        return nearestTile;
    }*/
}
