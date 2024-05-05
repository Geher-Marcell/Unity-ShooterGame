using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Area")]
    public Vector2 gameAreaStart = new Vector2(-8, -4);
    public Vector2 gameAreaEnd = new Vector2(8, 4);
    public float outOfBoundsTime = 3;
    private float remainingOutOfBoundsTime = 0;
    
    private Transform player;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        remainingOutOfBoundsTime = outOfBoundsTime;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Update()
    {
        if (player.position.x < gameAreaStart.x || player.position.x > gameAreaEnd.x || player.position.y < gameAreaStart.y || player.position.y > gameAreaEnd.y)
        {
            if (remainingOutOfBoundsTime > 0)
            {
                remainingOutOfBoundsTime -= Time.deltaTime;
                Debug.Log(Math.Ceiling(remainingOutOfBoundsTime));
            }
            else
            {
                // Kill player here
                Debug.Log("Out of bounds for too long!");
                SceneManager.LoadScene(0);
            }
        }
        else
        {
            remainingOutOfBoundsTime = outOfBoundsTime;
        }
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
