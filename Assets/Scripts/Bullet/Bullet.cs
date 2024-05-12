using System;
using Interfaces;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 5;
    
    public Rigidbody2D Rb { get; private set; }
    
    private void OnEnable()
    {
        Rb = GetComponent<Rigidbody2D>();
        
        Invoke(nameof(Deactivate), lifeTime);
        
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

        Deactivate();
    }
    
    void Deactivate() => gameObject.SetActive(false);
}
