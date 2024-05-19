using Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerStats : MonoBehaviour, IDamagable
    {
        public int MaxHealth;
        public int Health;
        public float FireRate;
        public int FireAmount;
        
        public delegate void DamageTaken();
        public static event DamageTaken OnDamageTaken;
        
        private void Awake() => Health = MaxHealth;
        
        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0) Die();
        }

        public void Die() => SceneManager.LoadScene(0);
    }
}