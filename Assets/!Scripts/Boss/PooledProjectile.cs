using System.Collections;
using UnityEngine;

namespace _Scripts.Boss
{
    [RequireComponent(typeof(Rigidbody))]
    public class PooledProjectile : MonoBehaviour
    {
        [Header("- Projectile Settings")]
        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private GameObject impactEffect;

        private ProjectilePool _pool;
        private Rigidbody _rigidbody;
        private Coroutine _lifetimeCoroutine;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _pool = FindObjectOfType<ProjectilePool>();
        }

        private void OnEnable()
        {
            // Start lifetime counter when projectile is activated.
            _lifetimeCoroutine = StartCoroutine(LifetimeCountdown());
        }

        private void OnDisable()
        {
            // Ensure coroutine is stopped when projectile is deactivated.
            if (_lifetimeCoroutine != null)
            {
                StopCoroutine(_lifetimeCoroutine);
                _lifetimeCoroutine = null;
            }
        }

        private IEnumerator LifetimeCountdown()
        {
            yield return new WaitForSeconds(lifetime);
            ReturnToPool();
        }

        public void SetVelocity(Vector3 velocity)
        {
            if (_rigidbody != null)
            {
                _rigidbody.linearVelocity = velocity;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Handle impact effects.
            if (impactEffect != null)
            {
                Instantiate(impactEffect, collision.contacts[0].point, 
                    Quaternion.LookRotation(collision.contacts[0].normal));
            }

            // Apply damage if applicable.
            var damageable = collision.gameObject.GetComponent<_Scripts.Enemy.Interfaces.IShieldable>();
            if (damageable != null)
            {
                damageable.TakeShieldDamage(damage);
            }

            // Return to pool.
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            // Reset the projectile state before returning to pool.
            if (_rigidbody != null)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
        
            if (_pool != null)
            {
                _pool.ReturnProjectile(gameObject);
            }
            else
            {
                // Fallback if pool not found.
                gameObject.SetActive(false);
            }
        }

        // For external configuration.
        public void SetDamage(float newDamage)
        {
            damage = newDamage;
        }

        public void SetLifetime(float newLifetime)
        {
            lifetime = newLifetime;
        }
    }
}