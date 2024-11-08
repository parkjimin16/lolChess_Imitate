using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HealthUI : MonoBehaviour, IPointerClickHandler
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
        Manager.UserHp.ResetHealthBarSelection();

        // ���� ü�¹ٸ� ���� ���·� �����ϰ� ����
        isSelected = true;
        HighlightHealthBar(true);

        // ī�޶� �̵� ���� ȣ��
        Manager.UserHp.OnHealthBarClicked(this);
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