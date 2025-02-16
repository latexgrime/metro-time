using _Scripts.Status_System;
using UnityEngine;

namespace _Scripts.Enemy.Enemy_Types.Freeze__Slow_
{
    public class SlowProjectile : MonoBehaviour
    {
        private AudioSource _audioSource;
        
        private float _speed;
        private float _lifetime;
        private float _damage = 5f;
        
        [Header("- Status Effect")] 
        [SerializeField] private float slowBuildupAmount = 15f;

        [SerializeField] private AudioClip projectileSound;
        [SerializeField] private AudioClip impactSound;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Play impact sound.
            if (_audioSource != null && impactSound != null)
            {
                AudioSource.PlayClipAtPoint(impactSound, transform.position);
            }
            
            // Check if the player was hit.
            if (collision.gameObject.CompareTag("Player"))
            {
                // Apply slow effect.
                StatusEffectManager statusHandler = collision.gameObject.GetComponent<StatusEffectManager>();
                if (statusHandler != null)
                {
                    statusHandler.AddSlowBuildup(slowBuildupAmount);
                }
            }

            // Destroy projectile on any collision.
            Destroy(gameObject);
        }

        public void Initialize(float projectileSpeed, float projectileLifetime)
        {
            _speed = projectileSpeed;
            _lifetime = projectileLifetime;
            
            // Set up rigidbody movement.
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = transform.forward * _speed;
            }

            // Destroy after lifetime. 
            Destroy(gameObject, _lifetime);
        }
    }
}
