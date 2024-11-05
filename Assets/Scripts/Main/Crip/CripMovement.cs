using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CripMovement : MonoBehaviour
{
    public float moveInterval = 2f; // �̵� ����
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
        // ��� Ÿ�� ��������
        //List<HexTile> allTiles = mapGenerator.GetAllTiles();

        // ����ִ� Ÿ�� ��� ����
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
        }
    }*/

    /*HexTile GetCurrentTile()
    {
        // ���� ��ġ���� ���� ����� Ÿ�� ã��
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
