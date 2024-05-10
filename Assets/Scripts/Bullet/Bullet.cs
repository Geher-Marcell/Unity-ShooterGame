using System;
using System.Collections;
using System.Collections.Generic;
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
    
    private void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
        
        Rb.AddForce(transform.up * speed, ForceMode2D.Impulse);
    }

    [Obsolete("Obsolete")]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out IDamagable damagable)) return;
        if (other.CompareTag("Player")) return;

        var t = transform;
        var pos = t.position;
        ParticleManager.Instance.PlayParticle(0, pos, t.rotation);
        ParticleManager.Instance.PlayParticle(1, pos);
            
        damagable.TakeDamage(damage);
        Destroy(gameObject);
    }
}
