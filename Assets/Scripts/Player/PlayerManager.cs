using Interfaces;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IDamagable
{
    [SerializeField] private int maxHealth = 100;
    public int Health { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TMP_Text healthText;
    
    void Start()
    {
        Health = maxHealth;
        UpdateUI();
        Application.targetFrameRate = 60;
    }
    
    public void TakeDamage(int damage)
    {
        Health -= damage;
        UpdateUI();
        if (Health <= 0) Die();
    }

    public void Die()
    {
        Debug.Log("Player died");
        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        healthText.text = $"{Health}/{maxHealth}";
    }
}
