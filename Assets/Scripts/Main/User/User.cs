using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    private Vector2 _lastTouchPos = Vector2.zero;
    private Vector2 _currentTouchPos = Vector2.zero;
    private Vector3 _prePos = Vector3.zero;

    private Vector3 _beforePosition;
    private Vector3 _offset;

    [SerializeField] private GameObject _movableObj;
    [SerializeField] private GameObject currentTile;

    private MovableObjectType _movableObjectType;

    private string _movableTag;
    private string _tileLayerName;

    private bool _isReturning = false;
    private bool _objectMoved = false;

    private ItemFrame _hoveredItem;
    [SerializeField]private MapGenerator _mapGenerator;
    private class ReturningObjectData
    {
        public GameObject obj;
        public Vector3 beforePosition;
        public GameObject currentTile;
        public MovableObjectType movableObjectType;
    }

    // ���� ����
    private ReturningObjectData _returningObjData;

    #region Unity Flow
    private void Start()
    {
        
        // �ʿ信 ���� �ʱ�ȭ
    }

    private void Update()
    {
        _lastTouchPos = _currentTouchPos;
        _currentTouchPos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {

            if (Manager.UI.CheckPopupStack())
            {
                Manager.UI.CloseAllPopupUI();
            }
;
            TouchBeganEvent();
            FindcurrentTile();
            _prePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {

            if (Input.mousePosition != _prePos)
            {
                TouchMovedEvent();
            }
            else
            {
                TouchStayEvent();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            TouchEndedEvent();
        }

 

        ObjectReturn();

        HandleItemHover();
        // Debugging rays (optional)
    }
    #endregion

    #region Mouse Click Logic
    private void TouchBeganEvent()
    {
        if (_isReturning)
        {
            return;
        }

        _objectMoved = false; // �߰�

        _movableObj = OnClickObjUsingTag("Item");
        if (_movableObj != null)
        {
            SetMovableObjectType(MovableObjectType.Item);
        }
        else
        {
            // "Champion" �±׷� �õ�
            _movableObj = OnClickObjUsingTag("Champion");
            if (_movableObj != null)
            {
                SetMovableObjectType(MovableObjectType.Champion);
            }
        }

        if (_movableObj != null)
        {
            // ������Ʈ Ÿ�Կ� ���� ���� Ÿ���� �����ɴϴ�.
            if (_movableObj.transform.parent != null)
            {
                currentTile = _movableObj.transform.parent.gameObject;
            }

            _beforePosition = _movableObj.transform.position;

            // Ÿ���� ���¸� ������Ʈ�մϴ�.
            if (currentTile != null)
            {
                HexTile tile = currentTile.GetComponent<HexTile>();

                if (_movableObjectType == MovableObjectType.Item)
                {
                    tile.isItemTile = false;
                    tile.itemOnTile = null;
                }
                else if (_movableObjectType == MovableObjectType.Champion)
                {
                    tile.isOccupied = false;
                    tile.championOnTile = null;
                }
            }

            // ������Ʈ�� ���� ��Ʈ�� �̵�
            _movableObj.transform.SetParent(null);
        }
    }

    private void TouchMovedEvent()
    {
        if (_movableObj != null)
        {
            _objectMoved = true; // �߰�

            Vector3 touchPos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, 10));

            _movableObj.transform.position = worldPos;
        }
    }

    private void TouchStayEvent()
    {
        // �ʿ� �� ����
    }

    private void TouchEndedEvent()
    {
        if (_movableObj != null)
        {
            if (_objectMoved)
            {
                // Ÿ���� ���¸� ������Ʈ�մϴ�.
                if (currentTile != null)
                {
                    HexTile tile = currentTile.GetComponent<HexTile>();

                    if (_movableObjectType == MovableObjectType.Item)
                    {
                        tile.isItemTile = false;
                        tile.itemOnTile = null;
                    }
                    else if (_movableObjectType == MovableObjectType.Champion)
                    {
                        tile.isOccupied = false;
                        tile.championOnTile = null;
                    }
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            // ��Ʈ�� ������Ʈ���� ��ȸ�մϴ�.
            foreach (RaycastHit hit in hits)
            {
                GameObject hitObj = hit.collider.gameObject;

                // �巡�� ���� ������Ʈ�� �����մϴ�.
                if (hitObj == _movableObj)
                {
                    continue;
                }

                // ����ϴ� Ÿ�� �±׸� Ȯ���մϴ�.
                string expectedTileTag = (_movableObjectType == MovableObjectType.Item) ? "ItemTile" : "PlayerTile";

                // �θ� ������Ʈ�� �±׵� Ȯ���մϴ�.
                GameObject hitParentObj = hitObj.transform.root.gameObject;

                if (hitObj.CompareTag(expectedTileTag) || hitParentObj.CompareTag(expectedTileTag))
                {
                    HexTile hitTile = hitObj.GetComponent<HexTile>() ?? hitParentObj.GetComponent<HexTile>();

                    if (hitTile != null)
                    {
                        if (_movableObjectType == MovableObjectType.Item)
                        {
                            HandleItemDrop(hitTile, hitTile.gameObject);
                        }
                        else if (_movableObjectType == MovableObjectType.Champion)
                        {
                            HandleUnitDrop(hitTile, hitTile.gameObject);
                            //var nearbyChampions = GetChampionsWithinOneTile(_movableObj);
                            //DebugChampionList(nearbyChampions); // ����� �ڵ� ȣ��
                        }

                        return; // Ÿ���� ã�����Ƿ� �޼��带 �����մϴ�.
                    }
                }

                // �߰��� �κ�: ������ ���� �� è�Ǿ𿡰� ������ ���� ó��
                if (_movableObjectType == MovableObjectType.Item)
                {
                    if (hitObj.CompareTag("Item") && hitObj.transform != _movableObj.transform)
                    {
                        // ������ ���� ó��
                        HandleItemCombination(hitObj);
                        return;
                    }
                    else if (hitObj.CompareTag("Champion"))
                    {
                        // è�Ǿ𿡰� ������ ���� ó��
                        HandleGiveItemToChampion(hitObj);
                        return;
                    }
                }
            }

            // Ÿ���̳� ������, è�Ǿ��� ã�� ���� ���
            _isReturning = true;
            _returningObjData = new ReturningObjectData
            {
                obj = _movableObj,
                beforePosition = _beforePosition,
                currentTile = currentTile,
                movableObjectType = _movableObjectType
            };

            // ���� ��ȣ�ۿ� ������Ʈ �ʱ�ȭ
            _movableObj = null;
            currentTile = null;
        }
    }
    #endregion

    #region Item Champion Move, Combine Logic
    private void HandleItemDrop(HexTile hitTile, GameObject hitTileObj)
    {
        if (hitTile.isItemTile == false)
        {
            // Ÿ���� ��� ���� ��� �������� �����ϴ�.
            _movableObj.transform.position = hitTileObj.transform.position + _offset;
            _movableObj.transform.SetParent(hitTileObj.transform);

            // Ÿ�� ���� ������Ʈ
            hitTile.isItemTile = true;
            hitTile.itemOnTile = _movableObj;


        }
        else
        {
            // Ÿ�Ͽ� �̹� �������� ���� ���, �������� ��ȯ�մϴ�.
            GameObject otherItem = hitTile.itemOnTile;

            // ���� ������Ʈ�� ���� Ÿ���� �ִ��� Ȯ���մϴ�.
            if (currentTile != null)
            {
                // �ٸ� �������� ���� Ÿ�Ϸ� �̵�
                otherItem.transform.position = currentTile.transform.position + _offset;
                otherItem.transform.SetParent(currentTile.transform);

                // Ÿ�� ���� ������Ʈ
                HexTile previousTile = currentTile.GetComponent<HexTile>();
                previousTile.isItemTile = true;
                previousTile.itemOnTile = otherItem;
            }
            else
            {
                // ���� Ÿ���� ���� ���, �ٸ� �������� ���ϴ� ��ġ�� �̵��ϰų� ������ �����մϴ�.
                otherItem.transform.position = _beforePosition;
                otherItem.transform.SetParent(null);
            }

            // �巡���� �������� ��� Ÿ�Ϸ� �̵�
            _movableObj.transform.position = hitTileObj.transform.position + _offset;
            _movableObj.transform.SetParent(hitTileObj.transform);

            // Ÿ�� ���� ������Ʈ
            hitTile.isItemTile = true;
            hitTile.itemOnTile = _movableObj;
        }
    }
    private void HandleUnitDrop(HexTile hitTile, GameObject hitTileObj)
    {
        if (hitTile.isOccupied == false)
        {
            // Ÿ���� ��� ���� ��� ������ �����ϴ�.
            _movableObj.transform.position = hitTileObj.transform.position + _offset;
            _movableObj.transform.SetParent(hitTileObj.transform);

            // Ÿ�� ���� ������Ʈ
            hitTile.isOccupied = true;
            hitTile.championOnTile = _movableObj;
            //GetChampionsWithinOneTile(_movableObj);

            // ���� Ÿ���� ���� ������Ʈ (���� Ÿ�ϰ� �ٸ� ����)
            if (currentTile != null && currentTile != hitTileObj)
            {
                HexTile previousTile = currentTile.GetComponent<HexTile>();
                previousTile.isOccupied = false;
                previousTile.championOnTile = null;
            }
        }
        else
        {
            // Ÿ�Ͽ� �̹� ������ ���� ���, ������ ���� ��ġ�� ��ȯ�մϴ�.
            _isReturning = true;
        }
    }
    private void HandleItemCombination(GameObject targetItemObj)
    {
        ItemFrame draggedItemFrame = _movableObj.GetComponent<ItemFrame>();
        ItemFrame targetItemFrame = targetItemObj.GetComponent<ItemFrame>();

        if (draggedItemFrame != null && targetItemFrame != null)
        {
            string combinedItemName = Manager.Item.ItemCombine(draggedItemFrame.ItemBlueprint.ItemId, targetItemFrame.ItemBlueprint.ItemId);

            if (combinedItemName == "error")
            {
                Debug.Log("������ ���� ����");
                _isReturning = true;
                return;
            }

            // ���ο� ������ ����
            GameObject newItem = Manager.Item.CreateItem(combinedItemName, targetItemObj.transform.position);

            // ���� �����۵� ����
            Destroy(targetItemFrame.gameObject);
            Destroy(draggedItemFrame.gameObject);

            // Ÿ�� ���� ������Ʈ
            HexTile tile = null;

            if (currentTile != null)
            {
                tile = currentTile.GetComponent<HexTile>();
            }
            else
            {
                tile = targetItemObj.transform.parent.GetComponent<HexTile>();
            }

            if (tile != null)
            {
                tile.itemOnTile = newItem;
                newItem.transform.SetParent(tile.transform);
            }
        }
        else
        {
            Debug.Log("������ �������� ã�� �� �����ϴ�.");
            _isReturning = true;
        }
    }
    private void HandleGiveItemToChampion(GameObject championObj)
    {
        ChampionBase cBase = championObj.GetComponent<ChampionBase>();

        if (cBase == null)
        {
            Debug.Log("è�Ǿ� ���̽��� ã�� �� �����ϴ�.");
            _isReturning = true;
            return;
        }

        ItemFrame draggedItemFrame = _movableObj.GetComponent<ItemFrame>();

        if (draggedItemFrame != null)
        {
            cBase.GetItem(draggedItemFrame.ItemBlueprint);

            // ������ ������Ʈ ����
            Destroy(draggedItemFrame.gameObject);

            // ���� Ÿ���� ������ ������ ������Ʈ
            if (currentTile != null)
            {
                HexTile tile = currentTile.GetComponent<HexTile>();
                tile.isItemTile = false;
                tile.championOnTile = null;
            }
        }
        else
        {
            Debug.Log("�巡�׵� ������ �������� ã�� �� �����ϴ�.");
            _isReturning = true;
        }
    }
    private void HandleItemHover()
    {
        // �巡�� ���̸� ȣ�� ó���� ���� �ʽ��ϴ�.
        if (_movableObj != null)
        {
            if (_hoveredItem != null)
            {
                _hoveredItem.HideItemInfoUI();
                _hoveredItem = null;
            }
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hoveredObject = hit.collider.gameObject;

            if (hoveredObject.CompareTag("Item"))
            {
                ItemFrame itemFrame = hoveredObject.GetComponent<ItemFrame>();
                if (_hoveredItem != itemFrame)
                {
                    if (_hoveredItem != null)
                    {
                        _hoveredItem.HideItemInfoUI();
                    }

                    _hoveredItem = itemFrame;
                    _hoveredItem.ShowItemInfoUI();
                }
            }
            else
            {
                if (_hoveredItem != null)
                {
                    _hoveredItem.HideItemInfoUI();
                    _hoveredItem = null;
                }
            }
        }
        else
        {
            if (_hoveredItem != null)
            {
                _hoveredItem.HideItemInfoUI();
                _hoveredItem = null;
            }
        }
    }
    private GameObject OnClickObjUsingTag(string tag)
    {
        Vector3 touchPos = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        RaycastHit hitInfo;
        Physics.Raycast(ray, out hitInfo);

        if (hitInfo.collider != null)
        {
            GameObject hitObject = hitInfo.collider.gameObject;
            if (hitObject != null && hitObject.gameObject.tag.Equals(tag))
            {
                return hitObject;
            }
        }
        return null;
    }
    private void ObjectReturn()
    {
        if (_isReturning && _returningObjData != null && _returningObjData.obj != null)
        {
            _returningObjData.obj.transform.position = Vector3.MoveTowards(
                _returningObjData.obj.transform.position,
                _returningObjData.beforePosition,
                Time.deltaTime * 30f);

            if (Vector3.Distance(_returningObjData.obj.transform.position, _returningObjData.beforePosition) < 0.01f)
            {
                _isReturning = false;

                // ������Ʈ�� ���� Ÿ���� �ڽ����� ����
                if (_returningObjData.currentTile != null)
                {
                    _returningObjData.obj.transform.SetParent(_returningObjData.currentTile.transform);
                    HexTile previousTile = _returningObjData.currentTile.GetComponent<HexTile>();

                    if (_returningObjData.movableObjectType == MovableObjectType.Item)
                    {
                        previousTile.isItemTile = true;
                        previousTile.itemOnTile = _returningObjData.obj;
                    }
                    else if (_returningObjData.movableObjectType == MovableObjectType.Champion)
                    {
                        previousTile.isOccupied = true;
                        previousTile.championOnTile = _returningObjData.obj;
                    }
                }

                // ��ȯ ������Ʈ ������ �ʱ�ȭ
                _returningObjData = null;
            }
        }
    }
    // ���� �޼��� �߰�
    public void SetMovableObjectType(MovableObjectType type)
    {
        _movableObjectType = type;

        if (type == MovableObjectType.Item)
        {
            _movableTag = "Item";
            _offset = new Vector3(0.0f, 0.3f, 0.0f);
        }
        else if (type == MovableObjectType.Champion)
        {
            _movableTag = "Champion";
            _offset = new Vector3(0.0f, 0.5f, 0.0f);
        }
    }
    private GameObject FindcurrentTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            GameObject hitObj = hit.collider.gameObject;

            // Ÿ���� �±׸� Ȯ���Ͽ� ���� Ÿ���� ����
            if (hitObj.CompareTag("PlayerTile") || hitObj.CompareTag("ItemTile"))
            {
                currentTile = hitObj;
            }
        }
        return null;
    }
    public List<GameObject> GetChampionsWithinOneTile(GameObject champion)
    {
        List<GameObject> champions = new List<GameObject>();

        // Champion�� �θ𿡼� HexTile�� �����ɴϴ�.
        HexTile currentTile = champion.transform.parent?.GetComponent<HexTile>();
        if (currentTile == null)
        {
            Debug.LogWarning("è�Ǿ��� � Ÿ�Ͽ��� �������� �ʽ��ϴ�.");
            return champions;
        }

        int q = currentTile.q;
        int r = currentTile.r;

        // ¦�� ������ ����
        bool isEvenRow = (r % 2) == 0;
        (int dq, int dr)[] directions;

        if (isEvenRow)
        {
            // Directions for even rows
            directions = new (int, int)[]
            {
            (1, 0),    // East
            (1, -1),   // Southeast
            (0, -1),   // Southwest
            (-1, 0),   // West
            (0, 1),    // Northwest
            (1, 1)     // Northeast
            };
        }
        else
        {
            // Directions for odd rows
            directions = new (int, int)[]
            {
            (1, 0),    // East
            (0, -1),   // Southeast
            (-1, -1),  // Southwest
            (-1, 0),   // West
            (-1, 1),   // Northwest
            (0, 1)     // Northeast
            };
        }

        foreach (var dir in directions)
        {
            int neighborQ = q + dir.dq;
            int neighborR = r + dir.dr;

            // ����� �α� �߰�
            //Debug.Log($"Checking neighbor at q: {neighborQ}, r: {neighborR}");

            // ���� Ÿ���� �����ϴ��� Ȯ��
            if (_mapGenerator.mapInfos[0].HexDictionary.TryGetValue((neighborQ, neighborR), out HexTile neighborTile))
            {
                if (neighborTile.itemOnTile != null && neighborTile.itemOnTile != champion)
                {
                    champions.Add(neighborTile.itemOnTile);
                }
            }
        }

        return champions;
    }
    public void DebugChampionList(List<GameObject> champions)
    {
        if (champions == null || champions.Count == 0)
        {
            Debug.Log("�ֺ��� è�Ǿ��� �����ϴ�.");
            return;
        }

        Debug.Log($"�ֺ��� �ִ� è�Ǿ� ��: {champions.Count}");
        foreach (var champion in champions)
        {
            if (champion != null)
            {
                Debug.Log($"è�Ǿ� �̸�: {champion.name}, ��ġ: {champion.transform.position}");
            }
            else
            {
                Debug.Log("è�Ǿ��� null�Դϴ�.");
            }
        }
    }
    #endregion
}
