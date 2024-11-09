using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    /*private Vector2 _lastTouchPos = Vector2.zero;
    private Vector2 _currentTouchPos = Vector2.zero;
    private Vector3 _prePos = Vector3.zero;

    private Vector3 _beforePosition;
    private Vector3 _offset = new Vector3(0.0f, 1.0f, 0.0f);

    [SerializeField] private GameObject _movableObj;
    [SerializeField] private GameObject currentTile;

    private const string _moveableTAG = "Moveable";

    private void Start()
    {

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

        // 캐릭터 Ray 확인용 
        if (_movableObj != null)
        {
            Ray ray = new Ray(_movableObj.transform.position, Camera.main.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
        }

        // TEST Code
        Vector3 touchPos = Input.mousePosition;
        Ray test_ray = Camera.main.ScreenPointToRay(touchPos);
        Debug.DrawRay(test_ray.origin, test_ray.direction * 1000, Color.blue);
    }

    private void TouchBeganEvent()
    {
        _movableObj = OnClickObjUsingTag(_moveableTAG);

        if (_movableObj != null)
        {
            _beforePosition = _movableObj.transform.position;
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
        // 마우스가 움직이지 않을 때의 동작이 필요하면 여기에 작성
    }

    private void TouchEndedEvent()
    {
        if (_movableObj != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("PlayerTile"));

            if (hitInfo.collider != null && hitInfo.collider.gameObject.GetComponent<HexTile>().isOccupied == false)
            {
                Debug.Log("hit info : " + hitInfo.collider.gameObject.name);
                _movableObj.transform.position = hitInfo.collider.gameObject.transform.position;
                _movableObj.transform.position += _offset;
                hitInfo.collider.gameObject.GetComponent<HexTile>().isOccupied = true;
                currentTile.GetComponent<HexTile>().isOccupied = false;
            }
            else
            {
                _movableObj.transform.position = _beforePosition;
                //currentTile.GetComponent<HexTile>().isOccupied = false;
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
    private GameObject FindcurrentTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("PlayerTile")))
        {
            currentTile = hit.collider.gameObject;
            Debug.Log(hit.collider.gameObject);
        }
        return null;
    }*/
}
