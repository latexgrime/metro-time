using _Scripts.Status_System;
using UnityEngine;

namespace _Scripts.Enemy.Enemy_Types.Electric__Stun_
{
    public class ElectricProjectile : MonoBehaviour
    {
        [Header("Projectile Settings")] 
        private float _speed;
        private float _lifetime;
        private float _damage = 10f;

        [Header("- Status Effect")] 
        [SerializeField] private float stunBuildupAmount = 35f;

        [SerializeField] private AudioClip projectileSound;
        [SerializeField] private AudioClip impactSound;

        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource != null && projectileSound != null)
            {
                _audioSource.PlayOneShot(projectileSound);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Play impact sound.
            if (_audioSource != null && impactSound != null)
            {
                AudioSource.PlayClipAtPoint(impactSound, transform.position);
            }

            // Check if we hit the player.
            if (collision.gameObject.CompareTag("Player"))
            {
                StatusEffectManager statusManager = collision.gameObject.GetComponent<StatusEffectManager>();
                if (statusManager != null)
                {
                    Debug.Log($"Adding Stun Buildup: {stunBuildupAmount}");
                    statusManager.AddStunBuildup(stunBuildupAmount);
                }
            }

            // Destroy projectile on collision.
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