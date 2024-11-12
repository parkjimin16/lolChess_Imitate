using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public int q; // Çí»ç°ï Å¥ºê ÁÂÇ¥°èÀÇ q
    public int r; // Çí»ç°ï Å¥ºê ÁÂÇ¥°èÀÇ r
    public int s; // Çí»ç°ï Å¥ºê ÁÂÇ¥°èÀÇ s (q + r + s = 0)

    public int x; // Rect Tile ÁÂÇ¥°è x
    public int y; // Rect Tile ÁÂÇ¥°è y

    public bool isRectangularTile = false; // Á÷»ç°¢Çü ¿µ¿ª ¿©ºÎ
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
