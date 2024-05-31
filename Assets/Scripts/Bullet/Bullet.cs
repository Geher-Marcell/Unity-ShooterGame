using System;
using Interfaces;
using Player;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    private int damage = 10;
    private float pushForce = 5;
    [SerializeField] private float lifeTime = 5;

    private Rigidbody2D Rb;
    
    private PlayerStats _playerStats = null;
    
    private void OnEnable()
    {
        Rb = GetComponent<Rigidbody2D>();
        
        Invoke(nameof(Deactivate), lifeTime);
        
        if(_playerStats == null)
            _playerStats = PlayerStats.Instance;
        
        damage = _playerStats.Damage;
        pushForce = _playerStats.PushForce;
        Rb.AddForce(transform.up * speed, ForceMode2D.Impulse);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Deactivate));
    }
    
    [Obsolete("Obsolete")]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out IDamagable damagable)) return;
        if (other.CompareTag("Player")) return;

        var t = transform;
        var pos = t.position;
        ParticleManager.Instance.PlayParticle((int) ParticleManager.ParticleType.FlyOff, pos, t.rotation);
        ParticleManager.Instance.PlayParticle((int) ParticleManager.ParticleType.Impact, pos);
        
        damagable.TakeDamage(damage);
        if (other.TryGetComponent(out Rigidbody2D rb))
            rb.AddForce(transform.up * pushForce, ForceMode2D.Impulse);

        Deactivate();
    }
    
    void Deactivate() => gameObject.SetActive(false);
}
