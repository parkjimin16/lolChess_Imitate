using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    // ���� & ������Ƽ
    [SerializeField] private UISceneMain uiMain;
    

    // Ŭ�� ����
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
    private bool _isDragging = false;
    private bool _previousIsBattleOngoing = false;

    private ItemFrame _hoveredItem;
    [SerializeField]private MapGenerator _mapGenerator;

    [SerializeField] private GameObject test;
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
    private void Update()
    {
        if(!_previousIsBattleOngoing && Manager.Stage.IsBattleOngoing)
        {
            // ������ ��� ���۵�
            if (_isDragging && _movableObj != null)
            {
                if (!_isReturning)
                {
                    // �巡�� ���� ������Ʈ ��ȯ ���μ��� �ʱ�ȭ
                    _isReturning = true;
                    _returningObjData = new ReturningObjectData
                    {
                        obj = _movableObj,
                        beforePosition = _beforePosition,
                        currentTile = currentTile,
                        movableObjectType = _movableObjectType
                    };
                    _movableObj = null;
                    currentTile = null;
                }
            }
        }
        _previousIsBattleOngoing = Manager.Stage.IsBattleOngoing;

        if (Manager.Stage.IsBattleOngoing)
        {
            if (_isDragging && _movableObj != null && _movableObjectType == MovableObjectType.Champion)
            {
                if (!_isReturning)
                {
                    // ��ȯ ������ �ʱ�ȭ�մϴ�.
                    _isReturning = true;
                    _returningObjData = new ReturningObjectData
                    {
                        obj = _movableObj,
                        beforePosition = _beforePosition,
                        currentTile = currentTile,
                        movableObjectType = _movableObjectType
                    };
                    _movableObj = null;
                    currentTile = null;
                }
                ObjectReturn(); // �巡�� ���� ������Ʈ ��ȯ
            }
        }

        _lastTouchPos = _currentTouchPos;
        _currentTouchPos = Input.mousePosition;

        if (Input.GetMouseButtonDown(1))
        {
            GameObject clickedObject = OnClickObjUsingTag("Champion");

            if (clickedObject != null)
            {
                Debug.Log("è�Ǿ� Ŭ��");
                HandleChampionRightClick(clickedObject);
                return;
            }
        }


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
    }
    #endregion

    #region Mouse Click Logic
    private void TouchBeganEvent()
    {
        if (_isReturning)
        {
            return;
        }

        _objectMoved = false;

        _movableObj = OnClickObjUsingTag("Item");
        if (_movableObj != null)
        {
            SetMovableObjectType(MovableObjectType.Item);
        }
        else
        {
            _movableObj = OnClickObjUsingTag("Champion");
            if (_movableObj != null)
            {
                SetMovableObjectType(MovableObjectType.Champion);
            }
        }

        if (_movableObj != null)
        {
            if (_movableObj.transform.parent != null)
            {
                currentTile = _movableObj.transform.parent.gameObject;
            }

            _beforePosition = _movableObj.transform.position;

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
                    //tile.championOnTile.Remove(_movableObj);
                }
            }
            //_movableObj.transform.SetParent(null);
        }
    }

    private void TouchMovedEvent()
    {
        if (_movableObj != null)
        {
            _objectMoved = true;
            _isDragging = true; // �巡�� ������ ǥ��

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
            _isDragging = false;
            if (_objectMoved)
            {
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
                        //tile.championOnTile.Remove(_movableObj);
                    }
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                GameObject hitObj = hit.collider.gameObject;

                if (hitObj == _movableObj)
                {
                    continue;
                }

                string expectedTileTag = (_movableObjectType == MovableObjectType.Item) ? "ItemTile" : "PlayerTile";

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
                        }

                        return; 
                    }
                }

                if (_movableObjectType == MovableObjectType.Item)
                {
                    if (hitObj.CompareTag("Item") && hitObj.transform != _movableObj.transform)
                    {
                        HandleItemCombination(hitObj);
                        return;
                    }
                    else if (hitObj.CompareTag("Champion"))
                    {
                        HandleGiveItemToChampion(hitObj);
                        return;
                    }
                }
            }

            _isReturning = true;
            _returningObjData = new ReturningObjectData
            {
                obj = _movableObj,
                beforePosition = _beforePosition,
                currentTile = currentTile,
                movableObjectType = _movableObjectType
            };

            _movableObj = null;
            currentTile = null;
        }
    }
    #endregion

    #region Item Champion Move, Combine Logic
    private void HandleChampionRightClick(GameObject championObj)
    {
        ChampionBase cBase = championObj.GetComponent<ChampionBase>();
        if (cBase != null)
        {
            uiMain.UIChampionExplainPanel.UpdateChampionExplainPanel(cBase);
        }
    }

    private void HandleItemDrop(HexTile hitTile, GameObject hitTileObj)
    {
        if (hitTile.isItemTile == false)
        {
            _movableObj.transform.position = hitTileObj.transform.position + _offset;
            _movableObj.transform.SetParent(hitTileObj.transform);

            hitTile.isItemTile = true;
            hitTile.itemOnTile = _movableObj;
            
        }
        else
        {
            GameObject otherItem = hitTile.itemOnTile;

            if (currentTile != null)
            {
                otherItem.transform.position = currentTile.transform.position + _offset;
                otherItem.transform.SetParent(currentTile.transform);

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

            _movableObj.transform.position = hitTileObj.transform.position + _offset;
            _movableObj.transform.SetParent(hitTileObj.transform);

            hitTile.isItemTile = true;
            hitTile.itemOnTile = _movableObj;
        }
    }
    private void HandleUnitDrop(HexTile hitTile, GameObject hitTileObj)
    {
        if (hitTile == null)
        {
            Debug.LogWarning("hitTile�� null�Դϴ�.");
            _isReturning = true;
            return;
        }

        if (!hitTile.HasChampion)
        {
            _movableObj.transform.position = hitTileObj.transform.position + _offset;
            _movableObj.transform.SetParent(hitTileObj.transform);

            hitTile.championOnTile.Add(_movableObj);

            if (currentTile != null && currentTile != hitTileObj)
            {
                
                HexTile previousTile = currentTile.GetComponent<HexTile>();
                Debug.Log(previousTile.name);
                if (previousTile != null)
                {
                    previousTile.championOnTile.Remove(_movableObj);
                }
                else
                {
                    Debug.LogWarning("���� Ÿ�Ͽ� HexTile ������Ʈ�� �����ϴ�.");
                }
            }

            currentTile = hitTileObj;
        }
        // Ÿ�Ͽ� ���� ������ ���� �ڸ���
        else
        {
            _isReturning = true;
        }

        Manager.User.ClearSynergy(Manager.User.GetHumanUserData());
        Manager.Champion.SettingNonBattleChampion(Manager.User.GetHumanUserData());
        Manager.Champion.SettingBattleChampion(Manager.User.GetHumanUserData());

        if (uiMain == null)
            return;

        uiMain.UISynergyPanel.UpdateSynergy(Manager.User.GetHumanUserData());
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
                if (tag == "Champion" && !Manager.Stage.IsBattleOngoing)
                {
                    UserData user1 = Manager.User.GetHumanUserData();
                    if (user1.TotalChampionObject.Contains(hitObject))
                    {
                        return hitObject;
                    }
                    else
                    {
                        // �����ڰ� �ƴϸ� ���� �Ұ�
                        return null;
                    }
                }
                else if (tag == "Item")
                {
                    UserData user1 = Manager.User.GetHumanUserData();
                    if (user1.UserItemObject.Contains(hitObject))
                    {
                        return hitObject;
                    }
                    else
                    {
                        // �����ڰ� �ƴϸ� ���� �Ұ�
                        return null;
                    }
                }
                //return hitObject;
            }
        }
        return null;
    }
    private void ObjectReturn()
    {
        if (_isReturning && _returningObjData != null && _returningObjData.obj != null)
        {
            if(_returningObjData.movableObjectType == MovableObjectType.Champion)
            {
                _returningObjData.obj.transform.position = _returningObjData.beforePosition;
            }
            if(_returningObjData.movableObjectType == MovableObjectType.Item)
            {
                _returningObjData.obj.transform.position = Vector3.MoveTowards(
                _returningObjData.obj.transform.position,
                _returningObjData.beforePosition,
                Time.deltaTime * 30f);
            }

            if (Vector3.Distance(_returningObjData.obj.transform.position, _returningObjData.beforePosition) < 0.01f)
            {
                _isReturning = false;

                if (_returningObjData.currentTile != null)
                {
                    _returningObjData.obj.transform.SetParent(_returningObjData.currentTile.transform);
                    HexTile previousTile = _returningObjData.currentTile.GetComponent<HexTile>();
                    if (previousTile != null)
                    {
                        if (_returningObjData.movableObjectType == MovableObjectType.Item)
                        {
                            previousTile.isItemTile = true;
                            previousTile.itemOnTile = _returningObjData.obj;
                        }
                        else if (_returningObjData.movableObjectType == MovableObjectType.Champion)
                        {
                            if (!previousTile.championOnTile.Contains(_returningObjData.obj))
                            {
                                previousTile.championOnTile.Add(_returningObjData.obj);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("���� Ÿ�Ͽ� HexTile ������Ʈ�� �����ϴ�.");
                    }
                }

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
            _offset = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    private GameObject FindcurrentTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj.CompareTag("PlayerTile") || hitObj.CompareTag("ItemTile"))
            {
                currentTile = hitObj;
                return currentTile;
            }
        }
        return null;
    }
     #endregion
}
