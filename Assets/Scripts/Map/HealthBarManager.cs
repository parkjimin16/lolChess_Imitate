using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public GameObject healthBarPrefab; // Inspector에서 HealthBar 프리팹을 할당
    public StageManager stageManager; // StageManager 스크립트 참조
    private List<HealthUI> healthBars = new List<HealthUI>();

    void Start()
    {
        InitializeHealthBars();
    }

    void InitializeHealthBars()
    {
        // 기존 체력바 삭제
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        healthBars.Clear();

        // 각 플레이어에 대한 체력바 생성
        foreach (PlayerData player in stageManager.allPlayers)
        {
            GameObject hbObj = Instantiate(healthBarPrefab, transform);
            HealthUI hbUI = hbObj.GetComponent<HealthUI>();
            hbUI.SetPlayer(player);
            healthBars.Add(hbUI);
        }
    }

    public void UpdateHealthBars()
    {
        foreach (HealthUI hbUI in healthBars)
        {
            hbUI.UpdateHealthBar();
        }
    }
}
