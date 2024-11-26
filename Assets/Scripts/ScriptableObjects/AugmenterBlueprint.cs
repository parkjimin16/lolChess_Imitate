using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "AugmenterBlueprint", menuName = "Blueprints/AugmenterBlueprint")]
public class AugmenterBlueprint : ScriptableObject
{
    [SerializeField] private List<AugmenterData> silverAugmenter;
    [SerializeField] private List<AugmenterData> goldAugmenter;
    [SerializeField] private List<AugmenterData> platinumAugmenter;

    public List<AugmenterData> SilverAugmenter => silverAugmenter;
    public List<AugmenterData> GoldAugmenter => goldAugmenter;
    public List<AugmenterData> PlatinumAugmenter => platinumAugmenter;

    #region 증강 선택
    public List<AugmenterData> GetRandomSilverAugments()
    {
        return GetRandomAugments(SilverAugmenter);
    }

    public List<AugmenterData> GetRandomGoldAugments()
    {
        return GetRandomAugments(GoldAugmenter);
    }

    public List<AugmenterData> GetRandomPlatinumAugments()
    {
        return GetRandomAugments(PlatinumAugmenter);
    }

    private List<AugmenterData> GetRandomAugments(List<AugmenterData> augmentList)
    {
        if (augmentList.Count < 6)
        {
            Debug.LogWarning("증강체 리스트가 충분하지 않습니다.");
            return augmentList;
        }

        return augmentList.OrderBy(x => Random.value).Take(6).ToList();
    }
    #endregion
}


[System.Serializable]
public class AugmenterData
{
    [SerializeField] private AugmenterType augmenterType;
    [SerializeField] private Sprite augmenterIcon;
    [SerializeField] private string augmenterName;
    [SerializeField, TextArea] private string augmenterDescription;
    [SerializeField] private BaseAugmenter baseAugmenter;

    public AugmenterType AugmenterType => augmenterType;
    public Sprite AugmenterIcon => augmenterIcon;
    public string AugmenterName => augmenterName;
    public string AugmenterDescription => augmenterDescription;
    public BaseAugmenter BaseAugmenter => baseAugmenter;
}
