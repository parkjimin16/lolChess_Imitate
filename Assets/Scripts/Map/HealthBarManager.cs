using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public StageManager stageManager;
    private List<HealthUI> healthBars = new List<HealthUI>();

    public static HealthBarManager Instance { get; private set; } // 싱글톤 인스턴스

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
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

        // 플레이어들의 체력을 기준으로 체력바 정렬
        healthBars.Sort((hb1, hb2) => hb2.PlayerData.health.CompareTo(hb1.PlayerData.health));

        // 정렬된 순서대로 체력바의 형제 순서(Sibling Index) 재배치
        for (int i = 0; i < healthBars.Count; i++)
        {
            healthBars[i].transform.SetSiblingIndex(i);
        }
    }

    public void UpdateHealthBars()
    {
        // 모든 체력바 업데이트
        foreach (HealthUI hbUI in healthBars)
        {
            hbUI.UpdateHealthBar();
            if (hbUI.PlayerData.health <= 0)
            {
                // 체력바를 회색으로 표시하거나 반투명하게 만들기
                hbUI.SetDead(); // SetDead() 메서드를 HealthBarUI에 구현
            }
        }

        // 플레이어들의 체력을 기준으로 체력바 정렬
        healthBars.Sort((hb1, hb2) =>
        {
            int healthComparison = hb2.PlayerData.health.CompareTo(hb1.PlayerData.health);
            if (healthComparison == 0)
            {
                return hb1.PlayerData.playerName.CompareTo(hb2.PlayerData.playerName);
            }
            return healthComparison;
        });

        // 정렬된 순서대로 체력바의 형제 순서(Sibling Index) 재배치
        for (int i = 0; i < healthBars.Count; i++)
        {
            healthBars[i].transform.SetSiblingIndex(i);
        }
    }

    public void ResetHealthBarSelection()
    {
        foreach (HealthUI hbUI in healthBars)
        {
            hbUI.HighlightHealthBar(false);
        }
    }

    // 체력바 클릭 시 호출되는 메서드
    public void OnHealthBarClicked(HealthUI clickedHealthBar)
    {
        // 카메라 이동 로직 호출
        CameraManager.Instance.MoveCameraToPlayer(clickedHealthBar.PlayerData);
    }
    public void HighlightHealthBar(PlayerData playerData)
    {
        foreach (HealthUI hbUI in healthBars)
        {
            if (hbUI.PlayerData == playerData)
            {
                hbUI.HighlightHealthBar(true);
                break;
            }
        }
    }
}
