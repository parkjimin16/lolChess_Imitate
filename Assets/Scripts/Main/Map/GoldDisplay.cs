using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class GoldDisplay
{
    private int playerGold;
    private int enemyGold;
    private List<GameObject> PlayerGoldList;
    private List<GameObject> EnemyGoldList;
    private int maxGold;
    private UserData userData;

    public GoldDisplay(UserData userData)
    {
        this.userData = userData;
        this.PlayerGoldList = userData.MapInfo.PlayerGold;
        this.EnemyGoldList = userData.MapInfo.EnemyGold;
        this.maxGold = PlayerGoldList.Count;
    }

    public void UpdateGoldTiles()
    {
        int activePlayerGold = Mathf.Clamp(playerGold / 10, 0, maxGold);
        int activeEnemyGold = Mathf.Clamp(enemyGold / 10, 0, maxGold);

        // 플레이어 골드 타일 업데이트
        for (int i = 0; i < PlayerGoldList.Count; i++)
        {
            PlayerGoldList[i].SetActive(i < activePlayerGold);
        }

        // 적 골드 타일 업데이트
        for (int i = 0; i < EnemyGoldList.Count; i++)
        {
            int index = EnemyGoldList.Count - 1 - i;
            EnemyGoldList[index].SetActive(i < activeEnemyGold);
        }
    }

    public void SetPlayerGold(int gold)
    {
        playerGold = gold;
        UpdateGoldTiles();
    }

    public void SetEnemyGold(int gold)
    {
        enemyGold = gold;
        UpdateGoldTiles();
    }
}
