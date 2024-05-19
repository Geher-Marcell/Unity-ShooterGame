using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class ExperienceAttractor : MonoBehaviour
    {
        [Header("XP Systems")]
        public float attractForce = 10;
        private List<Transform> _xpOrbs = new List<Transform>();

        private void Update()
        {
            if(_xpOrbs.Count == 0) return;
            foreach (var orb in _xpOrbs)
            {
                var direction = (transform.position - orb.position).normalized;
                orb.GetComponent<Rigidbody2D>().AddForce(direction * attractForce);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("ExperienceOrb")) return;
            if (_xpOrbs.Contains(other.transform)) return;

            _xpOrbs.Add(other.transform);
        }

        public void RemoveOrb(Transform orb)
        {
            if (_xpOrbs.Contains(orb)) _xpOrbs.Remove(orb);
        }

    }
}