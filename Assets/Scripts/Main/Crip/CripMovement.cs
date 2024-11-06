using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class CripMovement : MonoBehaviour
{
    public float moveInterval = 2f; // �̵� ����
    private float moveTimer;
    private MapInfo playerMapInfo;
    public HexTile currentTile;

    void Start()
    {
        moveTimer = moveInterval;

        // ũ���� ���� �� ������ �����ɴϴ�.
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

        // ��� ����ִ� Ÿ�� ��������
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
            // ���� Ÿ�� ����
            HexTile nextTile = unoccupiedTiles[Random.Range(0, unoccupiedTiles.Count)];

            // ���� Ÿ�� ���� ������Ʈ
            if (currentTile != null)
            {
                currentTile.isOccupied = false;
                currentTile.itemOnTile = null;
            }

            // �̵�
            transform.position = nextTile.transform.position;

            // ���ο� Ÿ�� ���� ������Ʈ
            nextTile.isOccupied = true;
            nextTile.itemOnTile = this.gameObject;
            currentTile = nextTile;

            // �θ� ���ο� Ÿ�Ϸ� ����
            transform.SetParent(nextTile.transform);
        }
    }

    HexTile GetCurrentTile()
    {
        if (playerMapInfo == null)
            return null;

        // ���� ��ġ���� ���� ����� Ÿ�� ã��
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
