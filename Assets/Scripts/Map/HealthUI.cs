using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;

public class HealthUI : MonoBehaviour
{
    public Image healthBarImage; // Inspector에서 할당
    public TextMeshProUGUI playerNameText; // Inspector에서 할당
    public TextMeshProUGUI currentHealth; // Inspector에서 할당
    private float healthPercent;
    private PlayerData playerData;

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
}
