using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private int fireAmount = 1;

    private float _nextFire;
    
    private void Start()
    {
        Destroy(bulletPrefab, 2f);
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
            var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            var angle = fireAmount > 1 ? startAngle + i * stepAngle : 0;
            
            bullet.transform.Rotate(0, 0, angle);
        }
    }
}
