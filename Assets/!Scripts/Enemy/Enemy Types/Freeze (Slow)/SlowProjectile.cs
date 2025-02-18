using System.Collections;
using _Scripts.Status_System;
using UnityEngine;

namespace _Scripts.Enemy.Enemy_Types.Freeze__Slow_
{
    public class SlowProjectile : MonoBehaviour
    {
        [Header("- Projectile Settings")]
        private float _speed;
        private float _lifetime;

        [Header("- Status Effect")]
        [SerializeField] private float slowBuildupAmount = 15f;

        [Header("- Effects")]
        [SerializeField] private GameObject impactEffectPrefab;
        [SerializeField] private AudioClip projectileSound;
        [SerializeField] private AudioClip impactSound;
        
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

            if (_audioSource != null && impactSound != null)
            {
                AudioSource.PlayClipAtPoint(impactSound, transform.position);
            }

            HandleImpactEffects(collision);

            if (collision.gameObject.CompareTag("Player"))
            {
                StatusEffectManager statusManager = collision.gameObject.GetComponent<StatusEffectManager>();
                if (statusManager != null)
                {
                    statusManager.AddSlowBuildup(slowBuildupAmount);
                }
            }

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
