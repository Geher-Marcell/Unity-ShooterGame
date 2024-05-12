using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private GameObject enemyPrefab;
    
    private float spawnRate = 1;
    private float nextSpawn = 0;

    private void Start()
    {
        PoolManager.Instance.CreatePool(enemyPrefab.name, enemyPrefab, 10, out _);
    }

    private void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnRate;
            SpawnEnemy();
        }
    }
    
    private void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
        
        var enemy = PoolManager.Instance.GetObject(enemyPrefab.name);

        if (enemy == null)
            enemy = PoolManager.Instance.AddObjectToPool(enemyPrefab.name);
        
        enemy.transform.position = spawnPoint.position;
        enemy.SetActive(true);
    }
}
