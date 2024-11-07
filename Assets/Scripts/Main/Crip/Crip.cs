using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class Crip : MonoBehaviour
{
    [SerializeField] private CripObjectData cripData;
    private int currentHP;
    public HexTile currentTile;
    public MapGenerator.MapInfo playerMapInfo;

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
        // 아이템 생성
        //GenerateItem();

        // 타일 상태 업데이트
        if (currentTile != null)
        {
            currentTile.isOccupied = false;
            currentTile.itemOnTile = null;
        }

        // 자신 파괴
        Destroy(this.gameObject);
    }

    public void Death()
    {
        GenerateItem();

        if (currentTile != null)
        {
            currentTile.isOccupied = false;
            currentTile.itemOnTile = null;
        }
        Destroy(this.gameObject);
    }
    void GenerateItem()
    {
        // 크립이 속한 맵의 ItemTile을 찾습니다.
        ItemTile itemTile = FindItemTileInMap();

        if (itemTile != null)
        {
            itemTile.GenerateItem();
        }
        else
        {
            Debug.LogWarning("ItemTile을 찾을 수 없습니다.");
        }
    }

    ItemTile FindItemTileInMap()
    {
        if (playerMapInfo != null)
        {
            foreach (var itemTile in playerMapInfo.ItemTile)
            {
                if(itemTile.TileType1 == ItemOwner.Player)
                {
                    return itemTile;
                }
            }
            
        }
        return null;
    }
}
