using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Vector2 gameAreaStart = new Vector2(-8, -4);
    public Vector2 gameAreaEnd = new Vector2(8, 4);

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
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Update()
    {
        if (player.position.x < gameAreaStart.x || player.position.x > gameAreaEnd.x ||
            player.position.y < gameAreaStart.y || player.position.y > gameAreaEnd.y)
        {
            Vector2 dir = Vector2.zero - (Vector2) player.position;
            
            //TODO A player movementbe a velocity-t állítja valami miatt, ezért nem működik a visszatolás
            
            Debug.DrawLine(player.position, dir/2, Color.red);
            player.GetComponent<Rigidbody2D>().AddForce(dir.normalized, ForceMode2D.Impulse);
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
