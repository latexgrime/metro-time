using UnityEngine;

namespace _Scripts.Weapon_Systems
{
    public class Bullet : MonoBehaviour
    {
        [Header("Bullet Properties")]
        public float damage;
        public float speed;
        public float lifetime = 3f;
        
        [Header("Effects")]
        public TrailRenderer trailRenderer;
        public GameObject impactEffect;
        
        private void Start()
        {
            // Destroy bullet after lifetime.
            Destroy(gameObject, lifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Handle impact effect.
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, collision.contacts[0].point, 
                    Quaternion.LookRotation(collision.contacts[0].normal));
                Destroy(impact, 2f); // Cleanup impact effect.
            }

            // Handle damage.
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            // Debug hit information.
            Debug.Log($"Bullet hit: {collision.gameObject.name} at {collision.contacts[0].point}");

            // Destroy the bullet.
            Destroy(gameObject);
        }
    }
}