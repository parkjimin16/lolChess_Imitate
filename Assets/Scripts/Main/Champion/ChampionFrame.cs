using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChampionFrame : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider manaSlider;
    [SerializeField] private List<Image> equipItemImage;
    [SerializeField] private TextMeshProUGUI level;

    [Header("Camera")]
    [SerializeField] private Transform canvas;
    private Camera cam;
    private ChampionBase championBase;
    private ChampionBlueprint championBlueprint;
    public void Init(ChampionBase cBase, ChampionBlueprint cBlueprint)
    {
        championBase = cBase;
        championBlueprint = cBlueprint;

        SetChampionLevel(championBlueprint.ChampionLevel);
        SetHPSlider(championBlueprint.CurHP, championBlueprint.MaxHP);
        SetManaSlider(championBlueprint.CurMana, championBlueprint.MaxMana);
        SetEquipItemImage(cBase.EquipItem);
    }

    public void SetChampionLevel(int level)
    {
        this.level.text = level.ToString();
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
        for(int i=0;i < equipItem.Count; i++)
        {
            equipItemImage[i].sprite = equipItem[i].Icon;
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
