using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AugmenterCube : MonoBehaviour
{
    [SerializeField] private List<Image> image_Augmenter;
    [SerializeField] private List<Button> btn_Augmenter;
    [SerializeField] private List<AugmenterData> augmenterDatas;

    public void Init(List<AugmenterData> augmenters)
    {
        augmenterDatas = augmenters;

        for (int i=0;i < image_Augmenter.Count;i++)
        {
            image_Augmenter[i].color = new Color(image_Augmenter[i].color.r, image_Augmenter[i].color.g, image_Augmenter[i].color.b, i < augmenters.Count ? 1f : 0f);
            if (i < augmenters.Count)
            {
                image_Augmenter[i].sprite = augmenters[i].AugmenterIcon;
                btn_Augmenter[i].interactable = true;
            }
            else
            {
                btn_Augmenter[i].interactable = false;
            }
        }

        for (int i = 0; i < btn_Augmenter.Count; i++)
        {
            int index = i;
            btn_Augmenter[i].onClick.AddListener(() => ShowAugmenterPopupUI(index));
        }
    }

    public void ShowAugmenterPopupUI(int index)
    {
        var augmenter = Manager.UI.ShowPopup<UIPopupAugmenterContainer>();

        Vector2 mousePosition = Input.mousePosition + new Vector3(0,-200,0);
        augmenter.InitContainer(augmenterDatas);
        augmenter.SetPosition(mousePosition);
    }
}
