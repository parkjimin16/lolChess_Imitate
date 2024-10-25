using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopPanel : UIBase
{
    [SerializeField] private List<GameObject> championSlotList;
    [SerializeField] private GameDataBlueprint gameDataBlueprint;
    [SerializeField] private List<ItemBlueprint> itemBlueprint;
    [SerializeField] private List<string> shopChampionList;
    [SerializeField] private GameObject targetObject;

    [SerializeField] private List<Transform> championPos;
    private int currentChampionIndex = 0;

    private bool isLoadComplete = false;
    public int Level = 1;

    [SerializeField] private RectTile rtile;

    public void InitShopBtn(GameDataBlueprint gameData)
    {
        gameDataBlueprint = gameData;

        SetUI<Button>();

        // ���� ����ġ ��ư
        SetButtonEvent("Btn_Exp", UIEventType.Click, UpdateExpBtn);
        SetButtonEvent("Btn_Reroll", UIEventType.Click, UpdateChampionSlot);

        foreach (GameObject slot in championSlotList)
        {
            Button button = slot.GetComponent<Button>();
            ChampionSlot cSlot = slot.GetComponent<ChampionSlot>();
            button.interactable = true;

            if (button != null)
            {
                button.onClick.AddListener(() => InstantiateChampion(cSlot.ChampionBlueprint, button));
            }
        }
        
    }

    private void InstantiateChampion(ChampionBlueprint cBlueprint, Button button)
    {
        if (currentChampionIndex < championPos.Count)
        {
            GameObject newChampionObject = Manager.Asset.InstantiatePrefab(cBlueprint.ChampionInstantiateName);
            newChampionObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            GameObject frame = Manager.Asset.InstantiatePrefab("ChampionFrame");

            frame.transform.SetParent(newChampionObject.transform, false);
            newChampionObject.transform.position = championPos[currentChampionIndex].position + new Vector3(0, 0.5f, 0);

            ChampionBase cBase = newChampionObject.GetComponent<ChampionBase>();
            ChampionFrame cFrame = frame.GetComponentInChildren<ChampionFrame>();

            cBase.SetChampion(cBlueprint);
            cBase.InitChampion(cFrame);
            currentChampionIndex++;

            button.interactable = false;
        }
        else
        {
            Debug.LogWarning("��� è�Ǿ� ��ġ�� ���Ǿ����ϴ�.");
        }
    }

    private void UpdateChampionSlot(PointerEventData enterEvent)
    {
        shopChampionList = GetRandomChampions(Level);

        int idx = 0;

        foreach (var championName in shopChampionList)
        {
            ChampionBlueprint championBlueprint = Manager.Asset.GetBlueprint(championName) as ChampionBlueprint;
            ChampionSlot championSlotSciprt = championSlotList[idx].GetComponent<ChampionSlot>();
            Button btn = championSlotList[idx].GetComponent<Button>();

            btn.interactable = true;
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

        return probabilities.Length - 1; // ������ �ε��� ��ȯ (�̷������δ� ���⿡ �������� �ʾƾ� ��)
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

    private void UpdateExpBtn(PointerEventData enterEvent)
    {
        Debug.Log("����ġ ����");
        Level++;
        if(Level >= 10)
        {
            Level = 10;
        }
    }
    
    public void SetChampionPos(RectTile tile)
    {
        rtile = tile;
        championPos = rtile.GetRectTileList();
    }
}
