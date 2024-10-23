using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserHpManager
{
    private GameObject healthBarContianer;
    private GameObject healthBarPrefab;
    private List<HealthUI> healthBars = new List<HealthUI>();

    public void InitUserHp(GameObject container, GameObject hpPrefab, List<HealthUI> hpList)
    {
        healthBarContianer = container;
        healthBarPrefab = hpPrefab;
        healthBars = hpList;

        InitializeHealthBars();
    }

    private void InitializeHealthBars()
    {
        /*
        // ���� ü�¹� ����
        foreach (Transform child in healthBarContianer.transform)
        {
            child.gameObject.SetActive(false);  
            Destroy(child.gameObject);
        }
        healthBars.Clear();
        */


        int idx = 0;

        if(healthBars.Count == Manager.Stage.AllPlayers.Length)
        {
            Debug.Log("Same");
        }
        else
        {
            Debug.Log("Error");
            return;
        }

        // �� �÷��̾ ���� ü�¹� ����
        foreach (PlayerData player in Manager.Stage.AllPlayers)
        {
            //GameObject hbObj = Instantiate(healthBarPrefab, transform);
            //HealthUI hbUI = hbObj.GetComponent<HealthUI>();
            //hbUI.SetPlayer(player);
            //healthBars.Add(hbUI);

            healthBars[idx].SetPlayer(player);
            idx++;
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
