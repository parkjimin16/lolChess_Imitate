using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class CripMovement : MonoBehaviour
{
    public float moveInterval = 2f; // �̵� ����
    private float moveTimer;
    private MapGenerator.MapInfo playerMapInfo;
    public HexTile currentTile;

    private HexTile targetTile;
    private Vector3 targetPosition;
    public float moveSpeed = 2f; // �̵� �ӵ�

    private float fixedYPosition; // ������ y-��ǥ

    // ���� Ÿ���� �����ϱ� ���� ����
    private HexTile lastTile;

    public Crip crip;

    void Start()
    {
        crip = GetComponent<Crip>();

        moveTimer = moveInterval;

        // ũ���� ���� �� ������ �����ɴϴ�.
        playerMapInfo = gameObject.GetComponent<Crip>().playerMapInfo;
        currentTile = GetCurrentTile();

        targetTile = null;

        // ũ���� y-��ǥ�� �����ϱ� ���� ���� y-��ǥ�� +0.5f�� ���� ���� �����մϴ�.
        fixedYPosition = transform.position.y;

        // �ʱ� ��ġ�� y-��ǥ�� �����մϴ�.
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
            // ���� ��ġ���� Ÿ�� ��ġ�� �̵�
            Vector3 currentPosition = transform.position;
            Vector3 desiredPosition = Vector3.MoveTowards(currentPosition, targetPosition, moveSpeed * Time.deltaTime);

            // y-��ǥ�� �����մϴ�.
            desiredPosition.y = fixedYPosition;

            transform.position = desiredPosition;

            // �̵� �߿� ��ġ ������� Ÿ���� �����ϰ� ���¸� ������Ʈ
            UpdateTileUnderCrip();
        }
    }

    private void MoveRandomly()
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

            // ��ǥ Ÿ�ϰ� ��ġ ����
            targetTile = nextTile;

            // ��ǥ ��ġ�� y-��ǥ�� �����մϴ�.
            Vector3 tilePosition = nextTile.transform.position;
            tilePosition.y = fixedYPosition;
            targetPosition = tilePosition;

            // ���� ���� ���
            Vector3 direction = (targetPosition - transform.position).normalized;

            // ������Ʈ�� �������� ȸ��
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
            // ���� Ÿ�ϰ� �ٸ��ٸ�
            if (hitTile != currentTile)
            {
                // ���� Ÿ�Ͽ��� ũ�� ����
                if (currentTile != null)
                {
                    currentTile.championOnTile.Remove(this.gameObject);
                }

                // ���ο� Ÿ�Ͽ� ũ�� �߰�
                if (!hitTile.championOnTile.Contains(this.gameObject))
                {
                    hitTile.championOnTile.Add(this.gameObject);
                }

                // ���� Ÿ�� ������Ʈ
                currentTile = hitTile;
                crip.currentTile = currentTile;
                crip.transform.SetParent(currentTile.transform);
            }
            // ���� Ÿ�ϰ� ���ٸ� �ƹ� �۾��� ���� ����
        }
        else
        {
            // �Ʒ��� Ÿ���� �������� �ʴ� ���, ���� Ÿ�Ͽ��� ũ�� ����
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

    HexTile GetCurrentTile()
    {
        return GetTileUnderCrip();
    }

}
