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
}
