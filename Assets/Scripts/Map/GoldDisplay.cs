using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDisplay : MonoBehaviour
{
    private int playerGold = 0;
    private int enemyGold = 0;

    [SerializeField]
    private List<GameObject> PlayerGoldList = new List<GameObject>();
    [SerializeField]
    private List<GameObject> EnemyGoldList = new List<GameObject>();

    public int currentGold = 0;

    private int maxGold = 5;

    // Start is called before the first frame update
    void Start()
    {
        GoldListAdd();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGoldTiles();
        //Debug.Log(activeGoldTiles);
    }

    private void GoldListAdd()
    {
        for (int i = 0; i < maxGold; i++)
        {
            GameObject leftTile = GameObject.Find($"PlayerGold_{i}");
            if (leftTile != null)
            {
                PlayerGoldList.Add(leftTile);
                leftTile.SetActive(false); // 초기에는 비활성화
            }

            GameObject rightTile = GameObject.Find($"EnemyGold_{i}");
            if (rightTile != null)
            {
                EnemyGoldList.Add(rightTile);
                rightTile.SetActive(false); // 초기에는 비활성화
            }
        }
    }

    private void UpdateGoldTiles()
    {
        // 활성화할 골드 타일의 수 계산
        int activePlayerGold = Mathf.Clamp(playerGold / 10, 0, maxGold);
        int activeEnemyGold = Mathf.Clamp(enemyGold / 10, 0, maxGold);

        // 왼쪽 골드 타일 업데이트
        for (int i = 0; i < PlayerGoldList.Count; i++)
        {
            if (i < activePlayerGold)
                PlayerGoldList[i].SetActive(true);
            else
                PlayerGoldList[i].SetActive(false);
        }

        // 오른쪽 골드 타일 업데이트
        for (int i = 0; i < EnemyGoldList.Count; i++)
        {
            int index = EnemyGoldList.Count - 1 - i; // 역순으로
            if (i < activeEnemyGold)
                EnemyGoldList[index].SetActive(true);
            else
                EnemyGoldList[index].SetActive(false);
        }
    }

    public void AddGold(int amount)
    {
        playerGold += amount;
        playerGold = Mathf.Clamp(playerGold, 0, maxGold);
        UpdateGoldTiles();
    }

    public void SpendGold(int amount)
    {
        playerGold -= amount;
        playerGold = Mathf.Clamp(playerGold, 0, maxGold);
        UpdateGoldTiles();
    }
}
