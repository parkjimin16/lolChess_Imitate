using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AugmenterCube : MonoBehaviour
{
    [SerializeField] private List<Image> image_Augmenter;

    public void Init(List<AugmenterData> augmenters)
    {
        for(int i=0;i < image_Augmenter.Count;i++)
        {
            image_Augmenter[i].color = new Color(image_Augmenter[i].color.r, image_Augmenter[i].color.g, image_Augmenter[i].color.b, i < augmenters.Count ? 1f : 0f);
            if (i < augmenters.Count)
            {
                image_Augmenter[i].sprite = augmenters[i].AugmenterIcon;
            }
        }
    }
}
