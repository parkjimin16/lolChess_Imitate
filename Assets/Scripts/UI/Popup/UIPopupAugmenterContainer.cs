using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupAugmenterContainer : UIPopup
{
    [SerializeField] private GameObject uiObject;
    [SerializeField] private List<GameObject> gameObjects;
    [SerializeField] private List<AugmenterContainerSlot> slot_Augmenter;
    [SerializeField] private List<AugmenterData> datas;

    public void InitContainer(List<AugmenterData> data)
    {
        datas = data;

        for(int i=0;i < slot_Augmenter.Count;i++)
        {
            if (i < data.Count)
            {
                slot_Augmenter[i].InitAugmenterContainerSlot(datas[i]);
                gameObjects[i].SetActive(true);
            }
            else
            {
                gameObjects[i].SetActive(false);
            }
        }
    }

    public void SetPosition(Vector2 position)
    {
        uiObject.transform.position = position;
    }
}
