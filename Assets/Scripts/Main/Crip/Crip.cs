using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crip : MonoBehaviour
{
    public CripObjectData cripData;
    private int currentHP;
    private MapGenerator mapGenerator;
    public HexTile currentTile;
    [SerializeField] private ItemTile itemtile;

    void Start()
    {
        currentHP = cripData.HP;
        mapGenerator = FindObjectOfType<MapGenerator>();
        
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
        itemtile.GenerateItem();
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
}
