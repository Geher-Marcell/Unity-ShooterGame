using Managers;
using UnityEngine;
// ReSharper disable PossibleLossOfFraction

[RequireComponent(typeof(PlayerManager))]
public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private int fireAmount = 1;

    private float _nextFire;
    
    private void Start()
    {
        PoolManager.Instance.CreatePool("BulletPool", bulletPrefab, 10, out _);
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > _nextFire)
        {
            _nextFire = Time.time + fireRate;
            Shoot();
        }        
    }
    
    private void Shoot()
    {
        float startAngle = -((fireAmount - 1) * 15) / fireAmount;
        float stepAngle = 30 / fireAmount;
        
        for (int i = 0; i < fireAmount; i++)
        {
            var bullet = PoolManager.Instance.GetObject(0);

            if (bullet == null)
            {
                PoolManager.Instance.AddObjectToPool(0);
                i--;
                continue;
            }

            var transform1 = transform;
            bullet.transform.position = transform1.position;
            bullet.transform.rotation = transform1.rotation;
            
            var angle = fireAmount > 1 ? startAngle + i * stepAngle : 0;
            
            bullet.transform.Rotate(0, 0, angle);
            
            bullet.SetActive(true);
        }
    }
}
