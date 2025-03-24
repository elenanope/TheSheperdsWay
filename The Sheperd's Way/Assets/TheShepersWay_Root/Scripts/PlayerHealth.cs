using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health System Configuration")]
    [SerializeField] Image playerHealthBar;
    public float maxHealth = 1f;
    public float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        PlayerStatusUpdater();
    }

    void PlayerStatusUpdater()
    {
        playerHealthBar.fillAmount = currentHealth;
    }

}
