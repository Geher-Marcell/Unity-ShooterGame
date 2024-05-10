using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 1f;

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
       Instantiate(bulletPrefab, transform.position, transform.rotation);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.up * 2);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, 0, 30) * transform.up * 2);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, 0, -30) * transform.up * 2);
    }
}
