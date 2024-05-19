using System;
using System.Collections.Generic;
using Interfaces;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Slider levelSlider;
    
    private ExperienceAttractor _experienceAttractor;
    [SerializeField] private PlayerStats _playerStats;
    
    void Start()
    {
        UpdateUI();
        
        _experienceAttractor = GetComponentInChildren<ExperienceAttractor>();
    }

    private void UpdateUI()
    {   
        #region ExpSlider

        levelText.text = $"Level {GameManager.Instance.CurrentLevel}";
        levelSlider.value = GameManager.Instance.CurrentExp;
        levelSlider.maxValue = GameManager.Instance.RequiredExp;

        #endregion
        
        healthText.text = $"Health: {_playerStats.Health}/{_playerStats.MaxHealth}";
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ExperienceOrb"))
        {
            _experienceAttractor.RemoveOrb(other.transform);
            other.gameObject.SetActive(false);
            
            GameManager.Instance.AddExp(1);
        }
    }
    
    private void OnEnable()
    {
        GameManager.OnExpCollected += UpdateUI;
        GameManager.OnLevelUp += UpdateUI;
        PlayerStats.OnDamageTaken += UpdateUI;
    }
    
    private void OnDisable()
    {
        GameManager.OnExpCollected -= UpdateUI;
        GameManager.OnLevelUp -= UpdateUI;
        PlayerStats.OnDamageTaken -= UpdateUI;
    }
}
