using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AugmenterBlueprint", menuName = "Blueprints/AugmenterBlueprint")]
public class AugmenterBlueprint : ScriptableObject
{
    [SerializeField] private List<AugmenterData> sliverAugmenter;
    [SerializeField] private List<AugmenterData> goldAugmenter;
    [SerializeField] private List<AugmenterData> platinumAugmenter;

    public List<AugmenterData> SliverAugmenter => sliverAugmenter;
    public List<AugmenterData> GoldAugmenter => goldAugmenter;
    public List<AugmenterData> PlatinumAugmenter => platinumAugmenter;
}


[System.Serializable]
public class AugmenterData
{
    [SerializeField] private AugmenterType augmenterType;
    [SerializeField] private Sprite augmenterIcon;
    [SerializeField] private string augmenterName;
    [SerializeField, TextArea] private string augmenterDescription;

    public AugmenterType AugmenterType => augmenterType;
    public Sprite AugmenterIcon => augmenterIcon;
    public string AugmenterName => augmenterName;
    public string AugmenterDescription => augmenterDescription;
}
