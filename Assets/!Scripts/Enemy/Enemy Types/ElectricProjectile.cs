using _Scripts.StatusSystem;
using UnityEngine;

namespace _Scripts.Enemy.Enemy_Types
{
    public class ElectricProjectile : MonoBehaviour
    {
        private float speed;
        private float lifetime;
        private float damage = 10f; 
        
        private void OnCollisionEnter(Collision collision)
        {
            // Check if we hit the player.
            if (collision.gameObject.CompareTag("Player"))
            {
                // Apply stun effect.
                StatusEffectHandler statusHandler = collision.gameObject.GetComponent<StatusEffectHandler>();
                if (statusHandler != null)
                {
                    statusHandler.ApplyStun();
                }
            }

            // Destroy projectile on any collision.
            Destroy(gameObject);
        }

        public void Initialize(float projectileSpeed, float projectileLifetime)
        {
            speed = projectileSpeed;
            lifetime = projectileLifetime;
            
            // Set up rigidbody movement.
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = transform.forward * speed;
            }

            // Destroy after lifetime.
            Destroy(gameObject, lifetime);
        }
    }
}