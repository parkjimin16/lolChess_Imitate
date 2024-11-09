using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AugmenterSlot : MonoBehaviour
{
    [SerializeField] private Image augmenter_Icon;
    [SerializeField] private TextMeshProUGUI augmenter_Name;
    [SerializeField] private TextMeshProUGUI augmenter_Desc;
    [SerializeField] private Button btn_Reroll;
    [SerializeField] private Button btn_AugmenterSlot;


    // 시너지 정보 필요함

    public void InitAugmenterSlot(Sprite sprite, string name, string desc)
    {
        augmenter_Icon.sprite = sprite;
        augmenter_Name.text = name;
        augmenter_Desc.text = desc;
    }
}
