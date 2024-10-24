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
        // �ʿ信 ���� �ʱ�ȭ
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
            // ���� �±׷� �õ�
            _movableObj = OnClickObjUsingTag("Moveable");
            if (_movableObj != null)
            {
                SetMovableObjectType(MovableObjectType.Unit);
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
                else if (_movableObjectType == MovableObjectType.Unit)
                {
                    tile.isOccupied = false;
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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;
            int layerMask = 1 << LayerMask.NameToLayer(_tileLayerName);

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
            {
                GameObject hitTileObj = hitInfo.collider.gameObject;
                HexTile hitTile = hitTileObj.GetComponent<HexTile>();

                if (_movableObjectType == MovableObjectType.Item)
                {
                    HandleItemDrop(hitTile, hitTileObj);
                }
                else if (_movableObjectType == MovableObjectType.Unit)
                {
                    HandleUnitDrop(hitTile, hitTileObj);
                }
            }
            else
            {
                // Ÿ���� �ƴ� ���� ������� ���, ������Ʈ�� ���� ��ġ�� ��ȯ
                _isReturning = true;
            }
        }
    }

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

            // ���� Ÿ���� ���� ������Ʈ
            if (currentTile != null)
            {
                HexTile previousTile = currentTile.GetComponent<HexTile>();
                previousTile.isOccupied = false;
            }
        }
        else
        {
            // Ÿ�Ͽ� �̹� ������ ���� ���, ������ ���� ��ġ�� ��ȯ�մϴ�.
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

                // ������Ʈ�� ���� Ÿ���� �ڽ����� ����
                if (currentTile != null)
                {
                    _movableObj.transform.SetParent(currentTile.transform);
                    HexTile previousTile = currentTile.GetComponent<HexTile>();

                    if (_movableObjectType == MovableObjectType.Item)
                    {
                        previousTile.isItemTile = true;
                        previousTile.itemOnTile = _movableObj;
                    }
                    else if (_movableObjectType == MovableObjectType.Unit)
                    {
                        previousTile.isOccupied = true;
                    }
                }
            }
        }
    }
    // ���� �޼��� �߰�
    public void SetMovableObjectType(MovableObjectType type)
    {
        _movableObjectType = type;

        // Ÿ�Կ� ���� �±�, ���̾�, ������ ����
        if (type == MovableObjectType.Item)
        {
            _movableTag = "Item";
            _tileLayerName = "ItemPush";
            _offset = new Vector3(0.0f, 0.3f, 0.0f);
        }
        else if (type == MovableObjectType.Unit)
        {
            _movableTag = "Moveable";
            _tileLayerName = "PlayerTile";
            _offset = new Vector3(0.0f, 1.0f, 0.0f);
        }
    }
    private GameObject FindcurrentTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("PlayerTile")))
        {
            currentTile = hit.collider.gameObject;
            Debug.Log(hit.collider.gameObject);
        }
        return null;
    }
}