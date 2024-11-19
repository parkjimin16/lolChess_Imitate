using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HealthBarSlot : MonoBehaviour, IPointerClickHandler
{
    public Image healthBarImage; // Inspector���� �Ҵ�
    public TextMeshProUGUI playerNameText; // Inspector���� �Ҵ�
    public TextMeshProUGUI currentHealth; // Inspector���� �Ҵ�
    private float healthPercent;
    private Player playerData;

    private Vector2 originalPosition; // ���� ��ġ ����
    private bool isSelected = false; // ���� ����

    public RectTransform contentRectTransform; // Content ������Ʈ�� RectTransform ����

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
        // ü�¹� �̹����� ȸ������ ����
        healthBarImage.color = Color.gray;

        // �÷��̾� �̸� �ؽ�Ʈ�� ��Ҽ� �߰� �Ǵ� ���� ����
        playerNameText.color = Color.gray;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected) return;

        Manager.UserHp.ResetHealthBarSelection();

        isSelected = true;
        HighlightHealthBar(true);

        Manager.UserHp.OnHealthBarClicked(this);
    }

    public void HighlightHealthBar(bool highlight)
    {
        if (contentRectTransform == null) return;

        if (highlight)
        {
            contentRectTransform.anchoredPosition = originalPosition + new Vector2(-20f, 0);
        }
        else
        {
            contentRectTransform.anchoredPosition = originalPosition;
            isSelected = false;
        }
    }
}