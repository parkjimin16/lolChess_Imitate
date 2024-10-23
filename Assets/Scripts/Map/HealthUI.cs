using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine.EventSystems;

public class HealthUI : MonoBehaviour, IPointerClickHandler
{
    public Image healthBarImage; // Inspector���� �Ҵ�
    public TextMeshProUGUI playerNameText; // Inspector���� �Ҵ�
    public TextMeshProUGUI currentHealth; // Inspector���� �Ҵ�
    private float healthPercent;
    private PlayerData playerData;

    private Vector2 originalPosition; // ���� ��ġ ����
    private bool isSelected = false; // ���� ����

    public RectTransform contentRectTransform; // Content ������Ʈ�� RectTransform ����

    public PlayerData PlayerData
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

    public void SetPlayer(PlayerData player)
    {
        playerData = player;
        if (playerNameText != null)
        {
            playerNameText.text = $"{player.playerName}";
            currentHealth.text = $"{player.health}";
        }
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (healthBarImage != null && playerData != null)
        {
            healthPercent = (float)playerData.health / 100f;
            healthBarImage.fillAmount = healthPercent;
        }
        if (currentHealth != null)
        {
            currentHealth.text = $"{playerData.health}";
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
        // �̹� ���õ� ���¶�� �ƹ� �͵� ���� ����
        if (isSelected) return;

        // ��� ü�¹��� ���� ���� �ʱ�ȭ
        HealthBarManager.Instance.ResetHealthBarSelection();

        // ���� ü�¹ٸ� ���� ���·� �����ϰ� ����
        isSelected = true;
        HighlightHealthBar(true);

        // ī�޶� �̵� ���� ȣ��
        HealthBarManager.Instance.OnHealthBarClicked(this);
    }

    public void HighlightHealthBar(bool highlight)
    {
        if (contentRectTransform == null) return;

        if (highlight)
        {
            // Content ������Ʈ�� �������� �̵�
            contentRectTransform.anchoredPosition = originalPosition + new Vector2(-20f, 0);
        }
        else
        {
            // Content ������Ʈ�� ���� ��ġ�� ����
            contentRectTransform.anchoredPosition = originalPosition;
            isSelected = false;
        }
    }
}