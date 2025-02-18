using System.Collections;
using _Scripts.Boss;
using _Scripts.Status_System;
using UnityEngine;

namespace _Scripts.Enemy.Enemy_Types.Electric__Stun_
{
    public class ElectricProjectile : MonoBehaviour
    {
        [Header("- Projectile Settings")]
        private float _speed;
        private float _lifetime;

        [Header("- Status Effect")]
        [SerializeField] private float stunBuildupAmount = 35f;

        [Header("- Effects")]
        [SerializeField] private AudioClip projectileSound;
        [SerializeField] private AudioClip impactSound;
        [SerializeField] private GameObject impactEffectPrefab;
        
        private bool _hasCollided = false;
        private AudioSource _audioSource;
        private ProjectilePool _pool;

        private void Start()
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyProjectile"), LayerMask.NameToLayer("EnemyProjectile"), true);
            gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
            
            _audioSource = GetComponent<AudioSource>();
            _pool = FindObjectOfType<ProjectilePool>();
    
            if (_pool == null)
            {
                Debug.LogWarning("ProjectilePool not found! Projectile won't be returned to pool.");
            }

            if (_audioSource != null && projectileSound != null)
            {
                _audioSource.PlayOneShot(projectileSound);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasCollided) return;
            _hasCollided = true;

            // Play impact sound.
            if (_audioSource != null && impactSound != null)
            {
                AudioSource.PlayClipAtPoint(impactSound, transform.position);
            }

            // Handle visual effects.
            HandleImpactEffects(collision);

            // Apply stun effect if it hits the player.
            if (collision.gameObject.CompareTag("Player"))
            {
                StatusEffectManager statusManager = collision.gameObject.GetComponent<StatusEffectManager>();
                if (statusManager != null)
                {
                    statusManager.AddStunBuildup(stunBuildupAmount);
                }
            }

            // Disable and return to pool.
            StartCoroutine(ReturnToPoolAfterDelay(impactSound != null ? impactSound.length : 0f));
        }

        public void Initialize(float projectileSpeed, float projectileLifetime)
        {
            _speed = projectileSpeed;
            _lifetime = projectileLifetime;
            _hasCollided = false;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = transform.forward * _speed;
            }

            // Return to pool after lifetime expires.
            StartCoroutine(ReturnToPoolAfterDelay(_lifetime));
        }

        private void HandleImpactEffects(Collision collision)
        {
            if (impactEffectPrefab != null)
            {
                GameObject effect = Instantiate(impactEffectPrefab, collision.contacts[0].point, 
                    Quaternion.LookRotation(collision.contacts[0].normal));
                // Auto-destroy effect after 2 seconds.
                Destroy(effect, 2f);
            }
        }

        private IEnumerator ReturnToPoolAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (_pool != null)
            {
                _pool.ReturnProjectile(gameObject);
            }
            else
            {
                // Fallback if pool is unavailable.
                gameObject.SetActive(false);
            }
        }
    }
}
