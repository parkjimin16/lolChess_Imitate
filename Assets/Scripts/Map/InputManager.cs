using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Unit selectedUnit;
    void Start()
    {

    }

    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 유닛 선택
                Unit unit = hit.collider.GetComponent<Unit>();
                if (unit != null)
                {
                    selectedUnit = unit;
                    Debug.Log("유닛 선택됨: " + unit.name);
                }
                else if (selectedUnit != null)
                {
                    // 타일 선택 및 유닛 이동
                    HexTile tile = hit.collider.GetComponent<HexTile>();
                    if (tile != null)
                    {
                        Debug.Log("타일 선택됨: " + tile.name + ", isOccupied: " + tile.isOccupied);
                        if (!tile.isOccupied)
                        {
                            selectedUnit.PlaceOnTile(tile);
                            Debug.Log("유닛 이동: " + selectedUnit.name + " -> " + tile.name);
                            selectedUnit = null;
                        }
                        else
                        {
                            Debug.Log("타일이 이미 점유되어 있습니다.");
                        }
                    }
                }
            }
        }*/
    }
}
