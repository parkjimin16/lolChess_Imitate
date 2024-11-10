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

    // 응당한 대가 전용
    public List<AugmenterData> GetAugmentsWithIndex(int index, List<AugmenterData> augmentList)
    {
        if (index < 0 || index >= augmentList.Count)
        {
            Debug.LogWarning("유효하지 않은 인덱스입니다.");
            return null;
        }

        var selectedAugment = augmentList[index];
        var resultList = new List<AugmenterData>();

        for (int i = 0; i < 6; i++)
        {
            resultList.Add(selectedAugment);
        }

        return resultList;
    }

    
    // 디버깅
    public AugmenterData GetAugmentByName(string name)
    {
        var allAugmenters = silverAugmenter.Concat(goldAugmenter).Concat(platinumAugmenter);
        return allAugmenters.FirstOrDefault(augment => augment.AugmenterName == name);
    }
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
