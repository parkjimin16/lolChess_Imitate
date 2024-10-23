using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public StageManager stageManager;
    private List<HealthUI> healthBars = new List<HealthUI>();

    public static HealthBarManager Instance { get; private set; } // �̱��� �ν��Ͻ�

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

        // �÷��̾���� ü���� �������� ü�¹� ����
        healthBars.Sort((hb1, hb2) => hb2.PlayerData.health.CompareTo(hb1.PlayerData.health));

        // ���ĵ� ������� ü�¹��� ���� ����(Sibling Index) ���ġ
        for (int i = 0; i < healthBars.Count; i++)
        {
            healthBars[i].transform.SetSiblingIndex(i);
        }
    }

    public void UpdateHealthBars()
    {
        // ��� ü�¹� ������Ʈ
        foreach (HealthUI hbUI in healthBars)
        {
            hbUI.UpdateHealthBar();
            if (hbUI.PlayerData.health <= 0)
            {
                // ü�¹ٸ� ȸ������ ǥ���ϰų� �������ϰ� �����
                hbUI.SetDead(); // SetDead() �޼��带 HealthBarUI�� ����
            }
        }

        // �÷��̾���� ü���� �������� ü�¹� ����
        healthBars.Sort((hb1, hb2) =>
        {
            int healthComparison = hb2.PlayerData.health.CompareTo(hb1.PlayerData.health);
            if (healthComparison == 0)
            {
                return hb1.PlayerData.playerName.CompareTo(hb2.PlayerData.playerName);
            }
            return healthComparison;
        });

        // ���ĵ� ������� ü�¹��� ���� ����(Sibling Index) ���ġ
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

    // ü�¹� Ŭ�� �� ȣ��Ǵ� �޼���
    public void OnHealthBarClicked(HealthUI clickedHealthBar)
    {
        // ī�޶� �̵� ���� ȣ��
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
