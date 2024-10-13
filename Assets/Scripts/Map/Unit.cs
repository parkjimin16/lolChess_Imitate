using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public HexTile currentTile;
    private Vector3 offset;
    //private bool isDragging = false;



    public void PlaceOnTile(HexTile tile)
    {
        if (currentTile != null)
        {
            // 기존 타일에서 유닛 제거
            currentTile.isOccupied = false;
        }

        currentTile = tile;
        currentTile.isOccupied = true;

        transform.position = tile.transform.position + new Vector3(0, 0.5f, 0); // 유닛을 타일 위로 올림
    }
}
