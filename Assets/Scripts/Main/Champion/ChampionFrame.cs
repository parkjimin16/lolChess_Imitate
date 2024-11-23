using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChampionFrame : ObjectPoolable
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider manaSlider;
    [SerializeField] private List<Image> equipItemImage;
    [SerializeField] private Image image_level;

    [Header("Camera")]
    [SerializeField] private Transform canvas;
    private Camera cam;
    private ChampionBase championBase;
    private ChampionBlueprint championBlueprint;

    public void Init(ChampionBase cBase, ChampionBlueprint cBlueprint)
    {
        championBase = cBase;
        championBlueprint = cBlueprint;

        SetChampionLevel();
        SetHPSlider(championBlueprint.CurHP, championBlueprint.MaxHP);
        SetManaSlider(championBlueprint.CurMana, championBlueprint.MaxMana);
        SetEquipItemImage(championBase.EquipItem);

        gameObject.transform.localPosition = Vector3.zero;
    }

    public void SetChampionLevel()
    {
        int level = championBase.ChampionLevel;

        Color color = Color.white;
        switch (level)
        {
            case 1: 
                color = Color.gray; break;
            case 2:
                color = Color.blue; break;
            case 3:
                color = Color.red; break;
        }

        image_level.color = color;
    }
    public void SetHPSlider(float curHP, float maxHP)
    {
        hpSlider.maxValue = maxHP;
        hpSlider.value = curHP;
    }

    public void SetManaSlider(float curMana, float maxMana)
    {
        manaSlider.maxValue = maxMana;
        manaSlider.value = curMana;
    }

    public void SetEquipItemImage(List<ItemBlueprint> equipItem)
    {
        for(int i=0;i < equipItemImage.Count; i++)
        {
            if (i < equipItem.Count)
            {
                equipItemImage[i].color = new Color(equipItemImage[i].color.r, equipItemImage[i].color.g, equipItemImage[i].color.b, 1);
                equipItemImage[i].sprite = equipItem[i].Icon;
            }
 
            else
            {
                equipItemImage[i].color = new Color(equipItemImage[i].color.r, equipItemImage[i].color.g, equipItemImage[i].color.b, 0);
                equipItemImage[i].sprite = null;
            }

        }
    }

    private void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
