using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserHpManager
{
    private GameObject healthBarContianer;
    private GameObject healthBarPrefab;
    private List<HealthBarSlot> healthBars;

    public void InitUserHp(List<HealthBarSlot> hpList)
    {
        healthBars = hpList;

        InitializeHealthBars();
    }

    public void InitializeHealthBars()
    {
        int idx = 0;

        if(healthBars.Count != Manager.Game.PlayerListObject.Length)
        {
            Debug.Log("Error");
            return;
        }

        // �� �÷��̾ ���� ü�¹� ����
        foreach (GameObject player in Manager.Game.PlayerListObject)
        {
            healthBars[idx].SetPlayer(player);
            idx++;
        }

        // �÷��̾���� ü���� �������� ü�¹� ����
        healthBars.Sort((hb1, hb2) => hb2.PlayerData.UserData.UserHealth.CompareTo(hb1.PlayerData.UserData.UserHealth));

        // ���ĵ� ������� ü�¹��� ���� ����(Sibling Index) ���ġ
        for (int i = 0; i < healthBars.Count; i++)
        {
            healthBars[i].transform.SetSiblingIndex(i);
        }
    }

    public void UpdateHealthBars()
    {
        // ��� ü�¹� ������Ʈ
        foreach (HealthBarSlot hbUI in healthBars)
        {
            hbUI.UpdateHealthBar();
            if (hbUI.PlayerData.UserData.IsUserDie())
            {
                // ü�¹ٸ� ȸ������ ǥ���ϰų� �������ϰ� �����
                hbUI.SetDead(); // SetDead() �޼��带 HealthBarUI�� ����
            }
        }

        // �÷��̾���� ü���� �������� ü�¹� ����
        healthBars.Sort((hb1, hb2) =>
        {
            int healthComparison = hb2.PlayerData.UserData.UserHealth.CompareTo(hb1.PlayerData.UserData.UserHealth);
            if (healthComparison == 0)
            {
                return hb1.PlayerData.UserData.UserName.CompareTo(hb2.PlayerData.UserData.UserName);
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
        foreach (HealthBarSlot hbUI in healthBars)
        {
            hbUI.HighlightHealthBar(false);
        }
    }

    // ü�¹� Ŭ�� �� ȣ��Ǵ� �޼���
    public void OnHealthBarClicked(HealthBarSlot clickedHealthBar)
    {
        // ī�޶� �̵� ���� ȣ��
        Manager.Cam.MoveCameraToPlayer(clickedHealthBar.PlayerData);
    }
    public void HighlightHealthBar(Player playerData)
    {
        foreach (HealthBarSlot hbUI in healthBars)
        {
            if (hbUI.PlayerData == playerData)
            {
                hbUI.HighlightHealthBar(true);
                break;
            }
        }
    }
}
