using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemTile : MonoBehaviour
{
    [SerializeField] private GameObject itemTile;
    [SerializeField] private List<GameObject> _items = new List<GameObject>(10);

    public ItemTileType TileType;
    public ItemTileType1 TileType1;
    // Start is called before the first frame update
    void Start()
    {
        //itemTile.layer = LayerMask.NameToLayer("Test");
        if (itemTile != null)
        {
            // 부모 오브젝트의 모든 자식 오브젝트 가져오기
            for (int i = 0; i < itemTile.transform.childCount; i++)
            {
                GameObject child = itemTile.transform.GetChild(i).gameObject;
                //child.layer = LayerMask.NameToLayer("Water");
                // 리스트 크기가 8보다 작을 때만 추가
                if (_items.Count < 10)
                {
                    _items.Add(child);
                }
            }
        }
        else
        {
            Debug.Log("Parent object 'ItemPush' not found!");
            return;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(TileType1 == ItemTileType1.Player)
            {
                string itemId = Manager.Item.NormalItem[Random.Range(0, Manager.Item.NormalItem.Count)].ItemId;
                Manager.Item.CreateItem(itemId, _items[1].transform.position);
            }

        }
    }
}
