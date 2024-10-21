using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    private Camera mainCamera;
    private ItemFrame draggedItem;
    private ItemFrame hoveredItem; 
    private Vector3 offset;
    private bool isDragging = false;

    #region Unity Flow
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        if (draggedItem != null)
        {
            isDragging = true;
            HandleDragging();
            if (Input.GetMouseButtonUp(0))
            {
                HandleDrop();
                isDragging = false;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            //HandleRightClick();
        }

        if (!isDragging)
        {
            HandleMouseHover();
        }
    }


    #endregion

    #region Item Click Logic
    private void HandleLeftClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;

            if (clickedObject.CompareTag("Item"))
            {
                if (hoveredItem != null)
                {
                    hoveredItem.HideItemInfoUI();
                    hoveredItem = null; 
                }

                draggedItem = clickedObject.GetComponent<ItemFrame>();
                offset = draggedItem.transform.position - GetMouseWorldPosition();
            }
            else
            {
                if (draggedItem != null)
                {
                    HandleDrop();
                }

                Manager.UI.CloseAllPopupUI();
            }
        }
    }

    private void HandleDragging()
    {
        if (draggedItem != null)
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            mouseWorldPos.y = Mathf.Max(mouseWorldPos.y, 0f);
            draggedItem.transform.position = mouseWorldPos;
        }
    }

    private void HandleDrop()
    {
        if (draggedItem != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Collider[] hitColliders = Physics.OverlapSphere(hit.point, 1f);

                foreach (Collider collider in hitColliders)
                {
                    if (collider.CompareTag("Item") && collider.transform != draggedItem.transform)
                    {
                        ItemFrame targetItemFrame = collider.GetComponent<ItemFrame>();

                        if (targetItemFrame != null)
                        {
                            string combinedItemName = Manager.Item.ItemCombine(draggedItem.ItemBlueprint.ItemId, targetItemFrame.ItemBlueprint.ItemId);

                            if (combinedItemName == "error")
                            {
                                Debug.Log("아이템 조합 실패");
                                return;
                            }

                            Manager.Item.CreateItem(combinedItemName, new Vector3(0, 0, 0));

                            Destroy(targetItemFrame.gameObject);
                            Destroy(draggedItem.gameObject);
                            break;
                        }
                    }
                    else if (collider.CompareTag("Champion"))
                    {
                        ChampionBase cBase = collider.gameObject.GetComponent<ChampionBase>();

                        if (cBase == null)
                            return;

                        cBase.GetItem(draggedItem.ItemBlueprint);
                    }
                }
            }

            // 드래그가 끝나면 null로 설정
            draggedItem = null;
        }
    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.WorldToScreenPoint(Vector3.zero).z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }


    private void HandleRightClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;

            if (clickedObject.CompareTag("Item"))
            {
                ItemFrame itemFrame = clickedObject.GetComponent<ItemFrame>();
                itemFrame.ShowItemInfoUI();
            }
        }
    }

    private void HandleMouseHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit))
        {
            GameObject hoveredObject = hit.collider.gameObject;

            if (hoveredObject.CompareTag("Item"))
            {
                ItemFrame itemFrame = hoveredObject.GetComponent<ItemFrame>();
                if (hoveredItem != itemFrame)
                {
                    
                    if (hoveredItem != null)
                    {
                        hoveredItem.HideItemInfoUI();
                    }

                    hoveredItem = itemFrame; 
                    hoveredItem.ShowItemInfoUI();
                }
            }
            else
            {
                if (hoveredItem != null)
                {
                    hoveredItem.HideItemInfoUI();
                    hoveredItem = null; 
                }
            }
        }
        else
        {
            if (hoveredItem != null)
            {
                hoveredItem.HideItemInfoUI();
                hoveredItem = null;
            }
        }
    }
    #endregion
}
