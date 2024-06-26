using System;
using Interfaces;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour, IDamagable
    {
        [Header("Enemy Variables")]
        [SerializeField] private float health = 100;
        [SerializeField] private float maxSpeed = 5;
        [SerializeField] private float acceleration = 2f;
        [SerializeField] private float airDrag = 95f;
        
        [Header("Attack Variables")]
        [SerializeField] private float damage = 10;
        [SerializeField] private float pushForce = 5;
        [SerializeField] private float attackRate = 1;
        
        private float currentHealth = 0;
        private float nextAttackTime = 0;
        
        [Space(10)]
        [SerializeField] private ParticleSystem deathParticles;
        [SerializeField] private ParticleSystem hitParticles;
        
        protected Transform Player { get; private set; }
        protected Rigidbody2D Rb { get; private set; }
        protected float Acceleration => acceleration;

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            SoundManager.Instance.PlaySFX("Hit");
            if (currentHealth <= 0)
                Die();
        }

        public void Die()
        {
            ParticleManager.Instance.PlayParticle((int)ParticleManager.ParticleType.EnemyDeath, transform.position, transform.rotation);
            GameManager.Instance.DropXp(transform.position);
            gameObject.SetActive(false);
        }

        public virtual void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            Rb = GetComponent<Rigidbody2D>();
        }

        public virtual void Update()
        {
            LimitSpeed();
            ApplyAirDrag();
        }

        public void OnEnable()
        {
            currentHealth = health;
        }

        private void LimitSpeed()
        {
            if (Rb.velocity.magnitude > maxSpeed) Rb.velocity = Rb.velocity.normalized * maxSpeed;
        }

        private void ApplyAirDrag()
        {
            var velocity = Rb.velocity;
            velocity = new Vector2(velocity.x * (airDrag/100), velocity.y * (airDrag/100));
            Rb.velocity = velocity;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (Time.time < nextAttackTime) return;
                nextAttackTime = Time.time + attackRate;
                other.gameObject.GetComponent<IDamagable>().TakeDamage((int)damage);
                other.rigidbody.AddForce((other.transform.position - transform.position).normalized * pushForce, ForceMode2D.Impulse);
            }
        }
    }
}