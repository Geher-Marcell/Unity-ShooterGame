using System;
using Managers;
using TMPro;
using UnityEngine;

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
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Update()
    {
        CheckOutOfBounds();
    }

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
                player.GetComponent<PlayerManager>().Die();
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
    public void DropXp(Vector2 position, int amount)
    {
        Instantiate(xpOrb, position, Quaternion.identity);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(gameAreaStart.x, gameAreaStart.y), new Vector2(gameAreaEnd.x, gameAreaStart.y));
        Gizmos.DrawLine(new Vector2(gameAreaEnd.x, gameAreaStart.y), new Vector2(gameAreaEnd.x, gameAreaEnd.y));
        Gizmos.DrawLine(new Vector2(gameAreaEnd.x, gameAreaEnd.y), new Vector2(gameAreaStart.x, gameAreaEnd.y));
        Gizmos.DrawLine(new Vector2(gameAreaStart.x, gameAreaEnd.y), new Vector2(gameAreaStart.x, gameAreaStart.y));
    }
}
