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

        // 각 플레이어에 대한 체력바 생성
        foreach (GameObject player in Manager.Game.PlayerListObject)
        {
            healthBars[idx].SetPlayer(player);
            idx++;
        }

        // 플레이어들의 체력을 기준으로 체력바 정렬
        healthBars.Sort((hb1, hb2) => hb2.PlayerData.UserData.UserHealth.CompareTo(hb1.PlayerData.UserData.UserHealth));

        // 정렬된 순서대로 체력바의 형제 순서(Sibling Index) 재배치
        for (int i = 0; i < healthBars.Count; i++)
        {
            healthBars[i].transform.SetSiblingIndex(i);
        }
    }

    public void UpdateHealthBars()
    {
        // 모든 체력바 업데이트
        foreach (HealthBarSlot hbUI in healthBars)
        {
            hbUI.UpdateHealthBar();
            if (hbUI.PlayerData.UserData.IsUserDie())
            {
                // 체력바를 회색으로 표시하거나 반투명하게 만들기
                hbUI.SetDead(); // SetDead() 메서드를 HealthBarUI에 구현
            }
        }

        // 플레이어들의 체력을 기준으로 체력바 정렬
        healthBars.Sort((hb1, hb2) =>
        {
            int healthComparison = hb2.PlayerData.UserData.UserHealth.CompareTo(hb1.PlayerData.UserData.UserHealth);
            if (healthComparison == 0)
            {
                return hb1.PlayerData.UserData.UserName.CompareTo(hb2.PlayerData.UserData.UserName);
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
        foreach (HealthBarSlot hbUI in healthBars)
        {
            hbUI.HighlightHealthBar(false);
        }
    }

    // 체력바 클릭 시 호출되는 메서드
    public void OnHealthBarClicked(HealthBarSlot clickedHealthBar)
    {
        // 카메라 이동 로직 호출
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
