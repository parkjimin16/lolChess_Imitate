using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChampionManager
{
    public void AddBattleChampion(UserData userData, GameObject champion)
    {
        ChampionBase addChampionBase = champion.GetComponent<ChampionBase>();

        if (addChampionBase == null || userData.BattleChampionObject == null)
            return;


        // 임시
        userData.BattleChampionObject.Add(champion);
        //battleChampion.Add(cBlueprint);

        Manager.Synerge.AddSynergyLine(userData, addChampionBase.ChampionName, Utilities.GetLineName(addChampionBase.ChampionLine_First));
        Manager.Synerge.AddSynergyLine(userData, addChampionBase.ChampionName, Utilities.GetLineName(addChampionBase.ChampionLine_Second));
        Manager.Synerge.AddSynergyLine(userData, addChampionBase.ChampionName, Utilities.GetJobName(addChampionBase.ChampionJob_First));
        Manager.Synerge.AddSynergyLine(userData, addChampionBase.ChampionName, Utilities.GetJobName(addChampionBase.ChampionJob_Second));

        ChampionBase cBase = champion.GetComponent<ChampionBase>();

        // 2성
        int sameChampionCount = userData.BattleChampionObject.Count(obj =>
        {
                ChampionBase championBase = obj.GetComponent<ChampionBase>();
                return championBase != null && cBase != null && championBase.ChampionName == cBase.ChampionName &&championBase.ChampionLevel == cBase.ChampionLevel;
        });
      

        GameObject championToEnhance = null;


        if (sameChampionCount >= 3)
        {
            championToEnhance = MergeChampion(userData, champion);
        }


        if (championToEnhance == null)
            return;

        // 3성
        ChampionBase tempChampionBase = championToEnhance.GetComponent<ChampionBase>();
        string championName = tempChampionBase.ChampionName;
        int sameChampionAfterMergeCount = userData.BattleChampionObject.Count(obj =>
        {
            ChampionBase championBase = obj.GetComponent<ChampionBase>();
            return championBase != null && championBase.ChampionName == championName && championBase.ChampionLevel == 2;
        });

        if (sameChampionAfterMergeCount >= 3)
        {
            championToEnhance = MergeChampion(userData, championToEnhance);
        }
    }

    private GameObject MergeChampion(UserData userData, GameObject champion)
    {
        List<ItemBlueprint> itemList = new List<ItemBlueprint> ();


        int countToRemove = 2;
        GameObject championToEnhance = champion;

        // 리스트를 뒤에서부터 순회하며 중복 챔피언 제거, 인덱스 에러남
        for (int i = userData.BattleChampionObject.Count - 1; i >= 0 && countToRemove > 0; i--)
        {
            ChampionBase championBase = userData.BattleChampionObject[i].GetComponent<ChampionBase>();
            if (championBase != null && championBase.ChampionName == championToEnhance.GetComponent<ChampionBase>().ChampionName)
            {
                if (championBase == championToEnhance.GetComponent<ChampionBase>())
                {
                    continue;
                }


                itemList.AddRange(championBase.EquipItem);

                Utilities.Destroy(userData.BattleChampionObject[i]);
                userData.BattleChampionObject.RemoveAt(i);
                countToRemove--;
            }
        }

        if (itemList.Count > 0)
        {
            Manager.Item.StartCreatingItems(itemList, new Vector3(0, 10, 0));
        }

        itemList.Clear();
        EnhanceChampion(championToEnhance);
        return championToEnhance;
    }

    private void EnhanceChampion(GameObject champion)
    {
        var cBase = champion.GetComponent<ChampionBase>();
        if (cBase != null)
        {
            cBase.ChampionLevelUp();
        }
    }

    public void RemoveBattleChampion(UserData userData, GameObject champion)
    {

    }
}

[System.Serializable]
public class ChampionLevelData
{
    public int Level;        // 등급
    public int Hp;
    public int Power;
}

[System.Serializable]
public class ChampionData
{
    public int Cost;
    public string[] Names;
}

[System.Serializable]
public class ChampionRandomData
{
    public int Level;
    public float[] Probability;
}

[System.Serializable]
public class ChampionMaxCount
{
    public int Cost;
    public int MaxCount;
}