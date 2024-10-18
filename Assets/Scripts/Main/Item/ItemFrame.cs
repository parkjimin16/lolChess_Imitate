using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ItemFrame : MonoBehaviour
{
    [SerializeField] private Image itemImge;
    [SerializeField] private TextMeshPro txt_ItemName;
    [SerializeField] private ItemBlueprint itemBlueprint;

    // 드래그
    private Vector3 offset;
    private RectTransform rectTransform;
    private Camera mainCamera;
    private bool isDragging = false;


    #region Property
    public Image ItemImage => itemImge;
    public TextMeshPro Txt_ItemName => txt_ItemName;
    public ItemBlueprint ItemBlueprint => itemBlueprint;
    #endregion


    public void Init(ItemBlueprint item)
    {
        if(item == null)
        {
            Debug.Log("item Null");
        }

        itemImge = item.Icon;
        txt_ItemName.text = item.ItemName;
        itemBlueprint = item;
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }


    #region Drag & Combine Check
    private void OnMouseDown()
    {
        if (gameObject.CompareTag("Item")) 
        {
            offset = gameObject.transform.position - GetMouseWorldPosition();
            isDragging = true; 
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging && gameObject.CompareTag("Item"))
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition() + offset;
            gameObject.transform.position = new Vector3(mouseWorldPos.x, transform.position.y, mouseWorldPos.z);
        }
    }

    private void OnMouseUp()
    {
        if (isDragging && gameObject.CompareTag("Item"))
        {
            isDragging = false;

            CheckForOverlapWithItem();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void CheckForOverlapWithItem()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit))
        {
            Collider[] hitColliders = Physics.OverlapSphere(hit.point, 1f); 

            foreach (Collider collider in hitColliders)
            {
                if (collider.CompareTag("Item") && collider.transform != transform)
                {
                    ItemFrame iFrame = collider.gameObject.GetComponent<ItemFrame>();

                    if (iFrame != null)
                    {
                        string name = Manager.Item.ItemCombine(itemBlueprint.ItemId, iFrame.ItemBlueprint.ItemId);

                        if(name == "error")
                        {
                            Debug.Log("조합 잘못함");
                            return;
                        }

                        Manager.Item.CreateItem(name, new Vector3(0, 0, 0));

                        Destroy(hit.transform.gameObject);
                        Destroy(gameObject);
                        break; 
                    }
                }
            }
        }
    }
    #endregion
}
