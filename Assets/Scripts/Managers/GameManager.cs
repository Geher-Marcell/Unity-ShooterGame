using System;
using Managers;
using Player;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private Transform player;

    [Header("Game Area")]
    public Vector2 gameAreaStart = new Vector2(-8, -4);
    public Vector2 gameAreaEnd = new Vector2(8, 4);
    
    [SerializeField] private GameObject outOfBoundsUI;
    [SerializeField] private TMP_Text outOfBoundsText;
    [SerializeField] private Transform outOfBoundsVisualPivot;
    
    public float outOfBoundsTime = 3;
    private float remainingOutOfBoundsTime = 0;

    [Header("Experience System")] 
    [SerializeField] private GameObject xpOrb;
    public int CurrentLevel { get; private set; } = 1;
    public int CurrentExp { get; private set; } = 0;
    [SerializeField] private AnimationCurve xpPerLevel;
    public int RequiredExp => Mathf.RoundToInt(xpPerLevel.Evaluate(CurrentLevel));
    
    [Header("Upgrades")]
    public UpgradeOption[] upgradeOptions;
    
    //Events
    public delegate void ExpCollected();
    public static event ExpCollected OnExpCollected;
    
    public delegate void LevelUp();
    public static event LevelUp OnLevelUp;
    
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        remainingOutOfBoundsTime = outOfBoundsTime;
        UpdateOutOfBoundsUI(false);
        PopulateUpgrades();
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        PoolManager.Instance.CreatePool(xpOrb.name, xpOrb, 10, out _);
    }

    public void Update() => CheckOutOfBounds();

    private void CheckOutOfBounds()
    {
        if (IsOutOfBounds(player))
        {
            if (remainingOutOfBoundsTime > 0)
            {
                remainingOutOfBoundsTime -= Time.deltaTime;
                UpdateOutOfBoundsUI(true);
            }
            else
                player.GetComponent<PlayerStats>().Die();
        }
        else
        {
            remainingOutOfBoundsTime = outOfBoundsTime;
            UpdateOutOfBoundsUI(false);
        }
    }
    
    private void UpdateOutOfBoundsUI(bool show = true)
    {
        if(outOfBoundsUI.activeSelf == !show) outOfBoundsUI.SetActive(show);

        if (show)
        {
            outOfBoundsText.text = Math.Round(remainingOutOfBoundsTime, 1) + "";

            Vector2 lookDir = player.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            outOfBoundsVisualPivot.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    
    public bool IsOutOfBounds(Transform t)
    {
        var pos = t.position; 
        return (pos.x < gameAreaStart.x ||
                pos.x > gameAreaEnd.x ||
                pos.y < gameAreaStart.y ||
                pos.y > gameAreaEnd.y);
    }
    
    //XP System
    public void DropXp(Vector2 position)
    {
        var xp = PoolManager.Instance.GetObject(xpOrb.name);

        if (xp == null)
            xp = PoolManager.Instance.AddObjectToPool(xpOrb.name);
        
        xp.transform.position = position;
        xp.SetActive(true);
    }

    public void AddExp(int expAmount)
    {
        CurrentExp += expAmount;
        OnExpCollected?.Invoke();
        CheckLevelUp();
    }
    
    private void CheckLevelUp()
    {
        if (CurrentExp < RequiredExp) return;
        
        CurrentExp -= RequiredExp;
        CurrentLevel++;
        OnLevelUp?.Invoke();
        
        UpgradeOption chosenUpgrade = upgradeOptions[Random.Range(0, upgradeOptions.Length)];
        chosenUpgrade.onUpgrade?.Invoke(); // Apply the upgrade effect
    }

    private void PopulateUpgrades()
    {
        upgradeOptions = new UpgradeOption[]
        {
            new()
            {
                upgradeName = "Health Upgrade",
                description = "Increases the player's health by 10",
                onUpgrade = () => Debug.Log("Health upgraded by 10")
            },
            new()
            {
                upgradeName = "Damage Upgrade",
                description = "Increases the player's damage by 5",
                onUpgrade = () => Debug.Log("Damage upgraded by 5")
            },
            new()
            {
                upgradeName = "Fire Rate Upgrade",
                description = "Increases the player's fire rate by 0.1",
                onUpgrade = () => Debug.Log("Fire rate upgraded by 0.1")
            }
        };
    }
}

public class UpgradeOption
{
    public string upgradeName; // Name displayed to the player
    public string description; // Description of the upgrade's effect
    public Action onUpgrade; // Delegate to handle the upgrade effect 
}
