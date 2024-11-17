using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemTile : MonoBehaviour
{
    [SerializeField] private GameObject itemTile;
    [SerializeField] private List<GameObject> _items = new List<GameObject>(10);

    public ItemOwner TileType1;
    public MapGenerator.MapInfo MapInfo;


    void Start()
    {
        if (itemTile != null)
        {
            for (int i = 0; i < itemTile.transform.childCount; i++)
            {
                GameObject child = itemTile.transform.GetChild(i).gameObject;

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

    public List<GameObject> _Items { get { return _items; } }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            GenerateItem();
        }
    }


    public void RemoveItem(GameObject item)
    {
        // 아이템이 위치한 타일을 찾습니다.
        Transform parentTileTransform = item.transform.parent;
        if (parentTileTransform != null)
        {
            HexTile tile = parentTileTransform.GetComponent<HexTile>();
            if (tile != null)
            {
                tile.isItemTile = false;
                tile.itemOnTile = null;
            }
        }

        Destroy(item);
    }

    public void GenerateItem()
    {
        if (TileType1 == ItemOwner.Player)
        {
            // 빈 타일 리스트 생성
            List<GameObject> emptyTiles = new List<GameObject>();

            // 모든 타일을 검사하여 빈 타일을 수집
            foreach (GameObject tileObj in _items)
            {
                HexTile tile = tileObj.GetComponent<HexTile>();
                if (tile != null && tile.isItemTile == false)
                {
                    emptyTiles.Add(tileObj);
                }
            }

            if (emptyTiles.Count > 0)
            {
                // 빈 타일 중에서 무작위로 선택
                GameObject selectedTile = emptyTiles[Random.Range(0, emptyTiles.Count)];

                // 아이템 생성
                string itemId = Manager.Item.NormalItem[Random.Range(0, Manager.Item.NormalItem.Count)].ItemId;
                GameObject newItem = Manager.Item.CreateItem(itemId, selectedTile.transform.position);

                // 아이템을 타일의 자식으로 설정하고 위치 조정
                newItem.transform.SetParent(selectedTile.transform);
                newItem.transform.position = selectedTile.transform.position + new Vector3(0, 0.3f, 0);

                // 타일의 상태 업데이트
                HexTile tile = selectedTile.GetComponent<HexTile>();
                tile.isItemTile = true;
                tile.itemOnTile = newItem;

                // MapInfo를 통해 UserData에 접근하여 아이템 추가
                if (MapInfo != null && MapInfo.playerData != null && MapInfo.playerData.UserData != null)
                {
                    UserData userData = MapInfo.playerData.UserData;
                    userData.UserItemObject.Add(newItem);
                }
            }
            else
            {
                // 모든 타일이 가득 찼을 경우
                Debug.Log("생성불가");
            }
        }
    }

    public void GenerateItem(string itemId)
    {
        if (TileType1 == ItemOwner.Player)
        {
            List<GameObject> emptyTiles = new List<GameObject>();

            foreach (GameObject tileObj in _items)
            {
                HexTile tile = tileObj.GetComponent<HexTile>();
                if (tile != null && tile.isItemTile == false)
                    emptyTiles.Add(tileObj);
            }

            if (emptyTiles.Count > 0)
            {
                GameObject selectedTile = emptyTiles[Random.Range(0, emptyTiles.Count)];
                GameObject newItem = Manager.Item.CreateItem(itemId, selectedTile.transform.position);

                newItem.transform.SetParent(selectedTile.transform);
                newItem.transform.position = selectedTile.transform.position + new Vector3(0, 0.3f, 0);

                HexTile tile = selectedTile.GetComponent<HexTile>();
                tile.isItemTile = true;
                tile.itemOnTile = newItem;
            }
            else
            {
                Debug.Log("생성불가");
            }
        }
    }

    public int EmptyTileCount()
    {
        int count = 0;

        foreach (GameObject tileObj in _items)
        {
            HexTile tile = tileObj.GetComponent<HexTile>();
            if (tile != null && tile.isItemTile == false)
                count++;
        }

        return count;
        ;
    }
}
