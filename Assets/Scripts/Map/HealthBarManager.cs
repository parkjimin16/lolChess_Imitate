using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public GameObject healthBarPrefab; // Inspector���� HealthBar �������� �Ҵ�
    public StageManager stageManager; // StageManager ��ũ��Ʈ ����
    private List<HealthUI> healthBars = new List<HealthUI>();

    void Start()
    {
        InitializeHealthBars();
    }

    void InitializeHealthBars()
    {
        // ���� ü�¹� ����
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        healthBars.Clear();

        // �� �÷��̾ ���� ü�¹� ����
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
