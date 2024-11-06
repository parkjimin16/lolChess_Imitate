using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class Crip : MonoBehaviour
{
    public CripObjectData cripData;
    private int currentHP;
    public HexTile currentTile;
    //[SerializeField] private ItemTile itemtile;

    void Start()
    {
        currentHP = cripData.HP;
        
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            OnDeath();
        }
    }

    void OnDeath()
    {
        //itemtile.GenerateItem();
        // ������ ����
        //GenerateItem();

        // Ÿ�� ���� ������Ʈ
        if (currentTile != null)
        {
            currentTile.isOccupied = false;
            currentTile.itemOnTile = null;
        }

        // �ڽ� �ı�
        Destroy(this.gameObject);
    }

    public void Death()
    {
        //GenerateItem();

        if (currentTile != null)
        {
            currentTile.isOccupied = false;
            currentTile.itemOnTile = null;
        }
        Destroy(this.gameObject);
    }
    void GenerateItem()
    {
        // ũ���� ���� ���� ItemTile�� ã���ϴ�.
        ItemTile itemTile = FindItemTileInMap();

        if (itemTile != null)
        {
            itemTile.GenerateItem();
        }
        else
        {
            Debug.LogWarning("ItemTile�� ã�� �� �����ϴ�.");
        }
    }

    ItemTile FindItemTileInMap()
    {
        // ���� Ÿ���� �θ���� Ž���Ͽ� MapInfo�� ã���ϴ�.
        MapInfo playerMapInfo = currentTile.transform.GetComponentInParent<MapInfo>();
        if (playerMapInfo != null)
        {
            // MapInfo ������ ItemTile ������Ʈ�� ã���ϴ�.
            ItemTile itemTile = playerMapInfo.mapTransform.GetComponentInChildren<ItemTile>();
            return itemTile;
        }
        return null;
    }
}
