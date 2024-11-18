using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectTile : MonoBehaviour
{
    [SerializeField]private List<Transform> rectTile = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 9; i++)
        {
            Transform rTile = transform.Find($"Rect_{i}_-1");
            rectTile.Add(rTile);
            //Debug.Log(rectTile[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {

        }
    }
    public List<Transform> GetRectTileList()
    {
        return rectTile;
    }
}
