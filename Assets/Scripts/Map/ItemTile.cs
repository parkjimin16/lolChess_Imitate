using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTile : MonoBehaviour
{
    [SerializeField] private GameObject itemTile;
    [SerializeField] private List<GameObject> _items = new List<GameObject>(10);

    public PlayerType TileType;
    public ItemOwner TileType1;
    // Start is called before the first frame update
    void Start()
    {
        if (itemTile != null)
        {
            // �θ� ������Ʈ�� ��� �ڽ� ������Ʈ ��������
            for (int i = 0; i < itemTile.transform.childCount; i++)
            {
                GameObject child = itemTile.transform.GetChild(i).gameObject;
                // ����Ʈ ũ�Ⱑ 8���� ���� ���� �߰�
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
            GenerateItem();
        }
    }
    public void RemoveItem(GameObject item)
    {
        // �������� ��ġ�� Ÿ���� ã���ϴ�.
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
            // �� Ÿ�� ����Ʈ ����
            List<GameObject> emptyTiles = new List<GameObject>();

            // ��� Ÿ���� �˻��Ͽ� �� Ÿ���� ����
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
                // �� Ÿ�� �߿��� �������� ����
                GameObject selectedTile = emptyTiles[Random.Range(0, emptyTiles.Count)];

                // ������ ����
                string itemId = Manager.Item.NormalItem[Random.Range(0, Manager.Item.NormalItem.Count)].ItemId;
                GameObject newItem = Manager.Item.CreateItem(itemId, selectedTile.transform.position);

                // �������� Ÿ���� �ڽ����� �����ϰ� ��ġ ����
                newItem.transform.SetParent(selectedTile.transform);
                newItem.transform.position = selectedTile.transform.position + new Vector3(0, 0.3f, 0);

                // Ÿ���� ���� ������Ʈ
                HexTile tile = selectedTile.GetComponent<HexTile>();
                tile.isItemTile = true;
                tile.itemOnTile = newItem;
            }
            else
            {
                // ��� Ÿ���� ���� á�� ���
                Debug.Log("�����Ұ�");
            }
        }
    }
}
