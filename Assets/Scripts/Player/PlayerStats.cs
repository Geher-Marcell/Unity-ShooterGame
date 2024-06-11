using Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerStats : MonoBehaviour, IDamagable
    {
        public static PlayerStats Instance;
        
        public int MaxHealth;
        public int Health;
        public float FireRate;
        public int FireAmount;
        public int Damage;
        public float PushForce;
        
        public delegate void DamageTaken();
        public static event DamageTaken OnDamageTaken;

        private void Awake()
        {
            Health = MaxHealth;
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        public void TakeDamage(int damage)
        {
            Health -= damage;
            OnDamageTaken?.Invoke();
            if (Health <= 0) Die();
        }

        public void Die() => SceneManager.LoadScene(0);
    }
}