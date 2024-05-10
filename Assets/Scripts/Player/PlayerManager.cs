using Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    }
    
    public void TakeDamage(int damage)
    {
        Health -= damage;
        UpdateUI();
        if (Health <= 0) Die();
    }

    public void Die()
    {
        SceneManager.LoadScene(0);
    }

    private void UpdateUI()
    {
        healthText.text = $"{Health}/{maxHealth}";
    }
}
