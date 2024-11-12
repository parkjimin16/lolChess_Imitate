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

    public bool isRectangularTile = false; // ���簢�� ���� ����
    //public bool isOccupied = false;
    public bool isItemTile = false;
    public GameObject itemOnTile = null;

    public List<GameObject> championOnTile = new List<GameObject>();
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
