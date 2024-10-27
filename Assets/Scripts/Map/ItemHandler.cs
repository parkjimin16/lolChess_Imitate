using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
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

    private void Start()
    {
        // 필요에 따라 초기화
    }

    private void Update()
    {
        _lastTouchPos = _currentTouchPos;
        _currentTouchPos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
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

        // Debugging rays (optional)
    }

    private void TouchBeganEvent()
    {
        _movableObj = OnClickObjUsingTag("Item");
        if (_movableObj != null)
        {
            SetMovableObjectType(MovableObjectType.Item);
        }
        else
        {
            // 유닛 태그로 시도
            _movableObj = OnClickObjUsingTag("Champion");
            if (_movableObj != null)
            {
                SetMovableObjectType(MovableObjectType.Champion);
            }
        }

        if (_movableObj != null)
        {
            // 오브젝트 타입에 따라 현재 타일을 가져옵니다.
            if (_movableObj.transform.parent != null)
            {
                currentTile = _movableObj.transform.parent.gameObject;
            }

            _beforePosition = _movableObj.transform.position;

            // 타일의 상태를 업데이트합니다.
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
                }
            }

            // 오브젝트를 월드 루트로 이동
            _movableObj.transform.SetParent(null);
        }
    }

    private void TouchMovedEvent()
    {
        if (_movableObj != null)
        {
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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            // 히트한 오브젝트들을 순회합니다.
            foreach (RaycastHit hit in hits)
            {
                GameObject hitObj = hit.collider.gameObject;

                // 드래그 중인 오브젝트는 무시합니다.
                if (hitObj == _movableObj)
                {
                    continue;
                }

                // 기대하는 타일 태그를 확인합니다.
                string expectedTileTag = (_movableObjectType == MovableObjectType.Item) ? "ItemTile" : "PlayerTile";

                // 부모 오브젝트의 태그도 확인합니다.
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

                        return; // 타일을 찾았으므로 메서드를 종료합니다.
                    }
                }

                // 추가된 부분: 아이템 결합 및 챔피언에게 아이템 전달 처리
                if (_movableObjectType == MovableObjectType.Item)
                {
                    if (hitObj.CompareTag("Item") && hitObj.transform != _movableObj.transform)
                    {
                        // 아이템 결합 처리
                        HandleItemCombination(hitObj);
                        return;
                    }
                    else if (hitObj.CompareTag("Champion"))
                    {
                        // 챔피언에게 아이템 전달 처리
                        HandleGiveItemToChampion(hitObj);
                        return;
                    }
                }
            }

            // 타일이나 아이템, 챔피언을 찾지 못한 경우
            _isReturning = true;
        }
    }

    private void HandleItemDrop(HexTile hitTile, GameObject hitTileObj)
    {
        if (hitTile.isItemTile == false)
        {
            // 타일이 비어 있을 경우 아이템을 놓습니다.
            _movableObj.transform.position = hitTileObj.transform.position + _offset;
            _movableObj.transform.SetParent(hitTileObj.transform);

            // 타일 상태 업데이트
            hitTile.isItemTile = true;
            hitTile.itemOnTile = _movableObj;


        }
        else
        {
            // 타일에 이미 아이템이 있을 경우, 아이템을 교환합니다.
            GameObject otherItem = hitTile.itemOnTile;

            // 현재 오브젝트의 이전 타일이 있는지 확인합니다.
            if (currentTile != null)
            {
                // 다른 아이템을 이전 타일로 이동
                otherItem.transform.position = currentTile.transform.position + _offset;
                otherItem.transform.SetParent(currentTile.transform);

                // 타일 상태 업데이트
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

            // 드래그한 아이템을 대상 타일로 이동
            _movableObj.transform.position = hitTileObj.transform.position + _offset;
            _movableObj.transform.SetParent(hitTileObj.transform);

            // 타일 상태 업데이트
            hitTile.isItemTile = true;
            hitTile.itemOnTile = _movableObj;
        }
    }

    private void HandleUnitDrop(HexTile hitTile, GameObject hitTileObj)
    {
        if (hitTile.isOccupied == false)
        {
            // 타일이 비어 있을 경우 유닛을 놓습니다.
            _movableObj.transform.position = hitTileObj.transform.position + _offset;
            _movableObj.transform.SetParent(hitTileObj.transform);

            // 타일 상태 업데이트
            hitTile.isOccupied = true;

            // 이전 타일의 상태 업데이트
            if (currentTile != null)
            {
                HexTile previousTile = currentTile.GetComponent<HexTile>();
                previousTile.isOccupied = false;
            }
        }
        else
        {
            // 타일에 이미 유닛이 있을 경우, 유닛을 원래 위치로 반환합니다.
            _isReturning = true;
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
        if (_isReturning && _movableObj != null)
        {
            _movableObj.transform.position = Vector3.MoveTowards(_movableObj.transform.position, _beforePosition, Time.deltaTime * 30f);

            if (Vector3.Distance(_movableObj.transform.position, _beforePosition) < 0.01f)
            {
                _isReturning = false;

                // 오브젝트를 이전 타일의 자식으로 설정
                if (currentTile != null)
                {
                    _movableObj.transform.SetParent(currentTile.transform);
                    HexTile previousTile = currentTile.GetComponent<HexTile>();

                    if (_movableObjectType == MovableObjectType.Item)
                    {
                        previousTile.isItemTile = true;
                        previousTile.itemOnTile = _movableObj;
                    }
                    else if (_movableObjectType == MovableObjectType.Champion)
                    {
                        previousTile.isOccupied = true;
                    }
                }
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
            _offset = new Vector3(0.0f, 0.5f, 0.0f);
        }
    }
    private GameObject FindcurrentTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            GameObject hitObj = hit.collider.gameObject;

            // 타일의 태그를 확인하여 현재 타일을 설정
            if (hitObj.CompareTag("PlayerTile") || hitObj.CompareTag("ItemTile"))
            {
                currentTile = hitObj;
            }
        }
        return null;
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
                tile.itemOnTile = null;
            }
        }
        else
        {
            Debug.Log("드래그된 아이템 프레임을 찾을 수 없습니다.");
            _isReturning = true;
        }
    }
}