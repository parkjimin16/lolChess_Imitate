using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public int q; // ���� ť�� ��ǥ���� q
    public int r; // ���� ť�� ��ǥ���� r
    public int s; // ���� ť�� ��ǥ���� s (q + r + s = 0)

    public int x; // Rect Tile ��ǥ�� x
    public int y; // Rect Tile ��ǥ�� y

    public bool isRectangularTile; // ���簢�� ���� ����
    //public bool isOccupied = false;
    public bool isItemTile;
    public GameObject itemOnTile;

    public List<GameObject> championOnTile;
    public bool isOccupied
    {
        get { return championOnTile.Count > 0; }
    }
    public bool HasChampion
    {
        get
        {
            foreach (var unit in championOnTile)
            {
                if (unit != null && unit.CompareTag("Champion"))
                {
                    return true;
                }
            }
            return false;
        }
    }

    private void Start()
    {
        isRectangularTile = false;
        isItemTile = false;
    }
}
