using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class CripMovement : MonoBehaviour
{
    public float moveInterval = 2f; // 이동 간격
    private float moveTimer;
    private MapGenerator.MapInfo playerMapInfo;
    public HexTile currentTile;

    private HexTile targetTile;
    private Vector3 targetPosition;
    public float moveSpeed = 2f; // 이동 속도

    private float fixedYPosition; // 고정된 y-좌표

    // 이전 타일을 추적하기 위한 변수
    private HexTile lastTile;

    public Crip crip;

    void Start()
    {
        crip = GetComponent<Crip>();

        moveTimer = moveInterval;

        // 크립이 속한 맵 정보를 가져옵니다.
        playerMapInfo = gameObject.GetComponent<Crip>().playerMapInfo;
        currentTile = GetCurrentTile();

        targetTile = null;

        // 크립의 y-좌표를 고정하기 위해 현재 y-좌표에 +0.5f를 더한 값을 저장합니다.
        fixedYPosition = transform.position.y;

        // 초기 위치의 y-좌표를 고정합니다.
        Vector3 startPosition = transform.position;
        startPosition.y = fixedYPosition;
        transform.position = startPosition;

        lastTile = currentTile;

        transform.rotation = Quaternion.Euler(0,180,0);

    }

    void Update()
    {
        if (targetTile == null)
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0f)
            {
                crip.PlayAnimation("Walk");
                MoveRandomly();
                moveTimer = moveInterval;
            }
        }
        else
        {
            // 현재 위치에서 타겟 위치로 이동
            Vector3 currentPosition = transform.position;
            Vector3 desiredPosition = Vector3.MoveTowards(currentPosition, targetPosition, moveSpeed * Time.deltaTime);

            // y-좌표를 고정합니다.
            desiredPosition.y = fixedYPosition;

            transform.position = desiredPosition;

            // 이동 중에 위치 기반으로 타일을 감지하고 상태를 업데이트
            UpdateTileUnderCrip();
        }
    }

    private void MoveRandomly()
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

            // 목표 타일과 위치 설정
            targetTile = nextTile;

            // 목표 위치의 y-좌표를 고정합니다.
            Vector3 tilePosition = nextTile.transform.position;
            tilePosition.y = fixedYPosition;
            targetPosition = tilePosition;

            // 진행 방향 계산
            Vector3 direction = (targetPosition - transform.position).normalized;

            // 오브젝트를 방향으로 회전
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
        }
    }


    void UpdateTileUnderCrip()
    {
        HexTile hitTile = GetTileUnderCrip();

        if (hitTile != null)
        {
            // 현재 타일과 다르다면
            if (hitTile != currentTile)
            {
                // 이전 타일에서 크립 제거
                if (currentTile != null)
                {
                    currentTile.championOnTile.Remove(this.gameObject);
                }

                // 새로운 타일에 크립 추가
                if (!hitTile.championOnTile.Contains(this.gameObject))
                {
                    hitTile.championOnTile.Add(this.gameObject);
                }

                // 현재 타일 업데이트
                currentTile = hitTile;
                crip.currentTile = currentTile;
                crip.transform.SetParent(currentTile.transform);
            }
            // 현재 타일과 같다면 아무 작업도 하지 않음
        }
        else
        {
            // 아래에 타일이 감지되지 않는 경우, 현재 타일에서 크립 제거
            if (currentTile != null)
            {
                currentTile.championOnTile.Remove(this.gameObject);
                currentTile = null;
            }
        }
    }

    HexTile GetTileUnderCrip()
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

    HexTile GetCurrentTile()
    {
        return GetTileUnderCrip();
    }

}
