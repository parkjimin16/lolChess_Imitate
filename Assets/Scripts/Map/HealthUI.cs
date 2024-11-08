using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HealthUI : MonoBehaviour, IPointerClickHandler
{
    public Image healthBarImage; // Inspector에서 할당
    public TextMeshProUGUI playerNameText; // Inspector에서 할당
    public TextMeshProUGUI currentHealth; // Inspector에서 할당
    private float healthPercent;
    private Player playerData;

    private Vector2 originalPosition; // 원래 위치 저장
    private bool isSelected = false; // 선택 여부

    public RectTransform contentRectTransform; // Content 오브젝트의 RectTransform 참조

    public Player PlayerData
    {
        get { return playerData; }
    }

    private void Start()
    {
        
        if (contentRectTransform != null)
        {
            originalPosition = contentRectTransform.anchoredPosition;
        }
    }

    public void SetPlayer(GameObject player)
    {
        playerData = player.GetComponent<Player>();

        if (playerNameText != null)
        {
            playerNameText.text = $"{playerData.UserData.UserName}";
            currentHealth.text = $"{playerData.UserData.UserHealth}";
            //Debug.Log(player.GetComponent<Player>().PlayerName);
        }
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (healthBarImage != null && playerData != null)
        {
            healthPercent = (float)playerData.UserData.UserHealth / 100f;
            healthBarImage.fillAmount = healthPercent;
        }
        if (currentHealth != null && playerData != null)
        {
            currentHealth.text = $"{playerData.UserData.UserHealth}";
        }
    }
    public void SetDead()
    {
        // 체력바 이미지를 회색으로 변경
        healthBarImage.color = Color.gray;

        // 플레이어 이름 텍스트에 취소선 추가 또는 색상 변경
        playerNameText.color = Color.gray;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 이미 선택된 상태라면 아무 것도 하지 않음
        if (isSelected) return;

        // 모든 체력바의 선택 상태 초기화
        Manager.UserHp.ResetHealthBarSelection();

        // 현재 체력바를 선택 상태로 설정하고 강조
        isSelected = true;
        HighlightHealthBar(true);

        // 카메라 이동 로직 호출
        Manager.UserHp.OnHealthBarClicked(this);
    }

    public void HighlightHealthBar(bool highlight)
    {
        if (contentRectTransform == null) return;

        if (highlight)
        {
            // Content 오브젝트를 왼쪽으로 이동
            contentRectTransform.anchoredPosition = originalPosition + new Vector2(-20f, 0);
        }
        else
        {
            // Content 오브젝트를 원래 위치로 복귀
            contentRectTransform.anchoredPosition = originalPosition;
            isSelected = false;
        }
    }
}