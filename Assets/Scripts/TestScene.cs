using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestScene : UIBase
{
    [SerializeField] private List<GameObject> championSlotList;
    [SerializeField] private GameDataBlueprint gameDataBlueprint;
    [SerializeField] private List<ItemBlueprint> itemBlueprint;
    [SerializeField] private List<string> shopChampionList;
    [SerializeField] private GameObject championObject;
    [SerializeField] private GameObject targetObject;

    private bool isLoadComplete = false;
    public int Level = 1;

    private void Start()
    {
        Manager.Asset.LoadAllAsync((count, totalCount) =>
        {
            if (count >= totalCount)
            {
                isLoadComplete = true;
                championObject = null;
                Debug.Log("Complete");
            }
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            championObject = Manager.Asset.InstantiatePrefab("ChampionFrame");
            championObject.transform.position = new Vector3(0,0,0);
            

            gameDataBlueprint = Manager.Asset.GetBlueprint("GameDataBlueprint") as GameDataBlueprint;


        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            InitBtn();

            ChampionBlueprint cBlueprint = Manager.Asset.GetBlueprint("ChampionBlueprint_Ahri") as ChampionBlueprint;
            ChampionBase cBase = championObject.GetComponent<ChampionBase>();

            cBase.SetChampion(cBlueprint);
            cBase.ChampionInit();

            //BaseSkill bSkill = championObject.GetComponent<BaseSkill>();
            //bSkill.UseSkill();
            //bSkill.UseSkillTarget(targetObject);

            Manager.Item.Init();
        }
    }

    public void InitBtn()
    {
        SetUI<Button>();

        SetButtonEvent("Btn_Reroll", UIEventType.Click, UpdateChampionSlot);
    }

    private void UpdateChampionSlot(PointerEventData enterEvent)
    {
        shopChampionList = GetRandomChampions(Level);

        int idx = 0;

        foreach (var championName in shopChampionList)
        {
            ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint(championName) as ChampionBlueprint;
            ChampionSlot championSlotSciprt = championSlotList[idx].GetComponent<ChampionSlot>();

            championSlotSciprt.ChampionSlotInit(championBlueprint, Utilities.SetSlotColor(championBlueprint.ChampionCost));
            idx++;
        }
    }

    private List<string> GetRandomChampions(int level)
    {
        List<string> selectedChampions = new List<string>();

        ChampionRandomData currentData = gameDataBlueprint.ChampionRandomDataList[level - 1];

        for (int i = 0; i < 5; i++)
        {
            int costIndex = GetCostIndex(currentData.Probability);
            ChampionData costChampionData = GetChampionDataByCost(costIndex + 1);
            if (costChampionData != null && costChampionData.Names.Length > 0)
            {
                string selectedChampion = costChampionData.Names[Random.Range(0, costChampionData.Names.Length)];
                selectedChampions.Add(selectedChampion);
            }
        }

        return selectedChampions;
    }

    private int GetCostIndex(float[] probabilities)
    {
        float[] cumulativeProbabilities = new float[probabilities.Length];
        cumulativeProbabilities[0] = probabilities[0];

        for (int i = 1; i < probabilities.Length; i++)
        {
            cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + probabilities[i];
        }

        float randomValue = Random.Range(0f, 1f);

        for (int i = 0; i < cumulativeProbabilities.Length; i++)
        {
            if (randomValue < cumulativeProbabilities[i])
            {
                return i;
            }
        }

        return probabilities.Length - 1; // 마지막 인덱스 반환 (이론적으로는 여기에 도달하지 않아야 함)
    }


    private ChampionData GetChampionDataByCost(int cost)
    {
        foreach (ChampionData data in gameDataBlueprint.ChampionDataList)
        {
            if (data.Cost == cost)
            {
                return data;
            }
        }
        return null;
    }
}
