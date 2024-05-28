using System;
using Managers;
using Player;
using UnityEngine;
// ReSharper disable PossibleLossOfFraction

[RequireComponent(typeof(PlayerManager))]
public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private float _fireRate ;
    private int _fireAmount;

    private float _nextFire;
    
    private void Start()
    {
        PoolManager.Instance.CreatePool("BulletPool", bulletPrefab, 10, out _);
        
        var playerStats = GetComponent<PlayerStats>();
        _fireRate = playerStats.FireRate;
        _fireAmount = playerStats.FireAmount;
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > _nextFire)
        {
            _nextFire = Time.time + _fireRate;
            Shoot();
        }        
    }
    
    private void Shoot()
    {
        float startAngle = -((_fireAmount - 1) * 15) / _fireAmount;
        float stepAngle = 30 / _fireAmount;
        
        for (int i = 0; i < _fireAmount; i++)
        {
            var bullet = PoolManager.Instance.GetObject("BulletPool");

            if (bullet == null)
                bullet = PoolManager.Instance.AddObjectToPool("BulletPool");

            var transform1 = transform;
            bullet.transform.position = transform1.position;
            bullet.transform.rotation = transform1.rotation;
            
            var angle = _fireAmount > 1 ? startAngle + i * stepAngle : 0;
            
            bullet.transform.Rotate(0, 0, angle);
            
            SoundManager.Instance.PlaySFX("Shoot");
            
            bullet.SetActive(true);
        }
    }

    private void OnLevelUp()
    {
        var playerStats = GetComponent<PlayerStats>();
        _fireRate = playerStats.FireRate;
        _fireAmount = playerStats.FireAmount;
    }
    
    private void OnEnable()
    {
        GameManager.OnLevelUp += OnLevelUp;
    }
    
    private void OnDisable()
    {
        GameManager.OnLevelUp -= OnLevelUp;
    }
}
