using System;
using System.Linq;
using System.Threading.Tasks;
using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("PauseMenu")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    
    [Header("Experience System")] 
    [SerializeField] private GameObject xpOrb;
    public int CurrentLevel { get; private set; } = 1;
    public int CurrentExp { get; private set; } = 0;
    [SerializeField] private AnimationCurve xpPerLevel;
    public int RequiredExp => Mathf.RoundToInt(xpPerLevel.Evaluate(CurrentLevel));
    
    [Header("Upgrades")]
    [SerializeField] private GameObject upgradePanel;
    private UpgradeOption[] upgradeOptions;
    
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
        
        pauseMenu.SetActive(false);
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        PoolManager.Instance.CreatePool(xpOrb.name, xpOrb, 10, out _);
    }

    public void Update()
    {
        CheckOutOfBounds();
        if(Input.GetKeyDown(pauseKey)) Pause(true);
    }
    
#region BoundChecks
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
#endregion

#region Pause Menu

private void Pause(bool pause = true)
{
    if(upgradePanel.activeSelf) return;
    if(pauseMenu.activeSelf == pause) return;   
    Time.timeScale = pause ? 0 : 1;
    pauseMenu.SetActive(pause);
}

public void Resume() => Pause(false);
public void Quit() => Application.Quit();

#endregion

#region XP System
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
        SoundManager.Instance.PlaySFX("Pickup");
        CheckLevelUp();
    }
    
    private void CheckLevelUp()
    {
        if (CurrentExp < RequiredExp) return;
        
        CurrentExp -= RequiredExp;
        CurrentLevel++;
        
        SoundManager.Instance.PlaySFX("LevelUp");
        
        ShowUpgradeOptions(); //The level up event is handled in the buttons of the upgrade panel
    }
#endregion

#region Upgrades
    private void ShowUpgradeOptions()
    {
        upgradePanel.SetActive(true);
        Time.timeScale = 0;
        
        var children = upgradePanel.GetComponentsInChildren<Transform>().Where(t => t.parent == upgradePanel.transform).ToList();

        foreach (var c in children)
        {
            var option = upgradeOptions[Random.Range(0, upgradeOptions.Length)]; //Select a random upgrade
            
            c.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Upgrades/" + option.upgradeName);
            c.GetChild(1).GetComponent<TMP_Text>().text = option.upgradeName;
            c.GetChild(2).GetComponent<TMP_Text>().text = option.description;
            
            c.GetComponent<Button>().onClick.RemoveAllListeners();
            c.GetComponent<Button>().onClick.AddListener(() =>
            {
                option.onUpgrade?.Invoke();
                Time.timeScale = 1;
                upgradePanel.SetActive(false);
                OnLevelUp?.Invoke();
            });
        }
    }

    private void PopulateUpgrades()
    {
        var playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        
        upgradeOptions = new UpgradeOption[]
        {
            new()
            {
                upgradeName = "Health",
                description = "Increases the player's health by 10",
                onUpgrade = () =>
                {
                    playerStats.MaxHealth += 10;
                    playerStats.Health += 10;
                }
            },
            new()
            {
                upgradeName = "Fire Rate",
                description = "Increases the player's fire rate by 0.1",
                onUpgrade = () => playerStats.FireRate -= 0.1f
            },
            new()
            {
                upgradeName = "+1 Bullet",
                description = "Increases the player's bullet count by 1",
                onUpgrade = () => playerStats.FireAmount++
            },
            new()
            {
                upgradeName = "Damage",
                description = "Increases the player's damage by 5",
                onUpgrade = () => playerStats.Damage += 5
            }
        };
    }
#endregion

    private async Task WaitForBoolResultAsync(Func<Task> action) => await action();
}

public class UpgradeOption
{
    public string upgradeName; // Name displayed to the player
    public string description; // Description of the upgrade's effect
    public Action onUpgrade; // Delegate to handle the upgrade effect 
}
