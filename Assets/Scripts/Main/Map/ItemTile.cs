using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemTile : MonoBehaviour
{
    [SerializeField] private GameObject itemTile;
    [SerializeField] private List<GameObject> items = new List<GameObject>(10);

    public ItemOwner TileType1;
    public MapGenerator.MapInfo MapInfo;


    void Start()
    {
        if (itemTile != null)
        {
            for (int i = 0; i < itemTile.transform.childCount; i++)
            {
                GameObject child = itemTile.transform.GetChild(i).gameObject;

                if (items.Count < 10)
                {
                    items.Add(child);
                }
            }
        }
        else
        {
            Debug.Log("Parent object 'ItemPush' not found!");
            return;
        }

    }

    public List<GameObject> Items { get { return items; } }

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
            foreach (GameObject tileObj in items)
            {
                HexTile tile = tileObj.GetComponent<HexTile>();
                if (tile != null && tile.isItemTile == false)
                {
                    emptyTiles.Add(tileObj);
                }
            }

            if (emptyTiles.Count > 0)
            {
                if (MapInfo == null && MapInfo.playerData == null && MapInfo.playerData.UserData == null)
                    return;

                GameObject selectedTile = emptyTiles[Random.Range(0, emptyTiles.Count)];

                string itemId = Manager.Item.NormalItem[Random.Range(0, Manager.Item.NormalItem.Count)].ItemId;
                GameObject newItem = Manager.Item.CreateItem(itemId, selectedTile.transform.position);
                newItem.transform.SetParent(selectedTile.transform);
                newItem.transform.position = selectedTile.transform.position + new Vector3(0, 0.3f, 0);

                HexTile tile = selectedTile.GetComponent<HexTile>();
                tile.isItemTile = true;
                tile.itemOnTile = newItem;


                UserData userData = MapInfo.playerData.UserData;
                ItemFrame bItem = newItem.GetComponent<ItemFrame>();
                bItem.ItemBlueprint.BaseItem.FirstItem(userData);
                userData.TotalItemBlueprint.Add(bItem.ItemBlueprint);
                userData.UserItemObject.Add(newItem);

            }
            else
            {

            }
        }
    }

    public void GenerateItem(string itemId)
    {
        if (TileType1 == ItemOwner.Player)
        {
            List<GameObject> emptyTiles = new List<GameObject>();

            foreach (GameObject tileObj in items)
            {
                HexTile tile = tileObj.GetComponent<HexTile>();
                if (tile != null && tile.isItemTile == false)
                    emptyTiles.Add(tileObj);
            }

            if (emptyTiles.Count > 0)
            {
                if (MapInfo == null && MapInfo.playerData == null && MapInfo.playerData.UserData == null)
                    return;


                GameObject selectedTile = emptyTiles[Random.Range(0, emptyTiles.Count)];
                GameObject newItem = Manager.Item.CreateItem(itemId, selectedTile.transform.position);

                newItem.transform.SetParent(selectedTile.transform);
                newItem.transform.position = selectedTile.transform.position + new Vector3(0, 0.3f, 0);

                HexTile tile = selectedTile.GetComponent<HexTile>();
                tile.isItemTile = true;
                tile.itemOnTile = newItem;

                UserData userData = MapInfo.playerData.UserData;
                ItemFrame bItem = newItem.GetComponent<ItemFrame>();
                bItem.ItemBlueprint.BaseItem.FirstItem(userData);
                userData.TotalItemBlueprint.Add(bItem.ItemBlueprint);
                userData.UserItemObject.Add(newItem);
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

        foreach (GameObject tileObj in items)
        {
            HexTile tile = tileObj.GetComponent<HexTile>();
            if (tile != null && tile.isItemTile == false)
                count++;
        }

        return count;
        ;
    }
}
