using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f;

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
        var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.up * 2);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, 0, 30) * transform.up * 2);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, 0, -30) * transform.up * 2);
    }
}
