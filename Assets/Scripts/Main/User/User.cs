using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    // 변수 & 프로퍼티
    [SerializeField] private UISceneMain uiMain;
    

    // 클릭 로직
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

    // 변수 선언
    private ReturningObjectData _returningObjData;

    #region Unity Flow
    private void Update()
    {
        if(!_previousIsBattleOngoing && Manager.Stage.IsBattleOngoing)
        {
            // 전투가 방금 시작됨
            if (_isDragging && _movableObj != null)
            {
                if (!_isReturning)
                {
                    // 드래그 중인 오브젝트 반환 프로세스 초기화
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
                    // 반환 과정을 초기화합니다.
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
                ObjectReturn(); // 드래그 중인 오브젝트 반환
            }
        }

        _lastTouchPos = _currentTouchPos;
        _currentTouchPos = Input.mousePosition;

        if (Input.GetMouseButtonDown(1))
        {
            GameObject clickedObject = OnClickObjUsingTag("Champion");

            if (clickedObject != null)
            {
                Debug.Log("챔피언 클릭");
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
            _isDragging = true; // 드래그 중임을 표시

            Vector3 touchPos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, 10));

            _movableObj.transform.position = worldPos;
        }
    }

    private void TouchStayEvent()
    {
        // 필요 시 구현
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
                // 이전 타일이 없을 경우, 다른 아이템을 원하는 위치로 이동하거나 동작을 정의합니다.
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
            Debug.LogWarning("hitTile이 null입니다.");
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
                    Debug.LogWarning("이전 타일에 HexTile 컴포넌트가 없습니다.");
                }
            }

            currentTile = hitTileObj;
        }
        // 타일에 유닛 있으면 원래 자리로
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
                Debug.Log("아이템 조합 실패");
                _isReturning = true;
                return;
            }

            // 새로운 아이템 생성
            GameObject newItem = Manager.Item.CreateItem(combinedItemName, targetItemObj.transform.position);

            // 기존 아이템들 삭제
            Destroy(targetItemFrame.gameObject);
            Destroy(draggedItemFrame.gameObject);

            // 타일 정보 업데이트
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
            Debug.Log("아이템 프레임을 찾을 수 없습니다.");
            _isReturning = true;
        }
    }
    private void HandleGiveItemToChampion(GameObject championObj)
    {
        ChampionBase cBase = championObj.GetComponent<ChampionBase>();

        if (cBase == null)
        {
            Debug.Log("챔피언 베이스를 찾을 수 없습니다.");
            _isReturning = true;
            return;
        }

        ItemFrame draggedItemFrame = _movableObj.GetComponent<ItemFrame>();

        if (draggedItemFrame != null)
        {
            cBase.GetItem(draggedItemFrame.ItemBlueprint);

            // 아이템 오브젝트 삭제
            Destroy(draggedItemFrame.gameObject);

            // 현재 타일의 아이템 정보를 업데이트
            if (currentTile != null)
            {
                HexTile tile = currentTile.GetComponent<HexTile>();
                tile.isItemTile = false;
                tile.championOnTile = null;
            }
        }
        else
        {
            Debug.Log("드래그된 아이템 프레임을 찾을 수 없습니다.");
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
                        // 소유자가 아니면 선택 불가
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
                        // 소유자가 아니면 선택 불가
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
                        Debug.LogWarning("이전 타일에 HexTile 컴포넌트가 없습니다.");
                    }
                }

                _returningObjData = null;
            }
        }
    }

    // 설정 메서드 추가
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
