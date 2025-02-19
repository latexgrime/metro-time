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
        [SerializeField] private GameObject impactEffectPrefab;
        [SerializeField] private AudioClip projectileSound;
        [SerializeField] private AudioClip impactSound;
        
        [Header("- Visual Settings")]
        [SerializeField] private bool instantlyDisappearOnImpact = true;
        [SerializeField] private float soundOnlyDelayTime = 0.1f;
        
        private ParticleSystem _mainParticleSystem;
        private ParticleSystem[] _particleSystems;
        private TrailRenderer[] _trailRenderers;
        private Renderer[] _renderers;
        private bool _hasCollided = false;
        private Collider _collider;
        private AudioSource _audioSource;
        private Rigidbody _rigidbody;
        private ProjectilePool _pool;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
            _particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            _trailRenderers = GetComponentsInChildren<TrailRenderer>(true);
            _renderers = GetComponentsInChildren<Renderer>(true);
            
            // Cache the main particle system if available.
            if (_particleSystems.Length > 0)
            {
                _mainParticleSystem = _particleSystems[0];
            }
        }
        
        private void Start()
        {
            // Ignore collisions between projectiles and enemies.
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyProjectile"), LayerMask.NameToLayer("Enemy"), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyProjectile"), LayerMask.NameToLayer("EnemyProjectile"), true);
            
            _pool = FindObjectOfType<ProjectilePool>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasCollided) return;
            _hasCollided = true;

            if (instantlyDisappearOnImpact)
            {
                // Completely hide all visual elements immediately.
                HideAllVisuals();
            }
            else
            {
                // Just stop emission but let existing particles fade out.
                StopAllParticleEmission();
            }
            
            // Disable physics but keep game object active.
            DisablePhysics();

            // Play impact sound.
            if (_audioSource != null && impactSound != null)
            {
                AudioSource.PlayClipAtPoint(impactSound, transform.position);
            }

            // Create impact effect.
            HandleImpactEffects(collision);

            // Apply status effect if hit player.
            if (collision.gameObject.CompareTag("Player"))
            {
                StatusEffectManager statusManager = collision.gameObject.GetComponent<StatusEffectManager>();
                if (statusManager != null)
                {
                    statusManager.AddStunBuildup(stunBuildupAmount);
                }
            }

            float delay;
            if (instantlyDisappearOnImpact)
            {
                // Use a small delay for the impact sound to play.
                delay = impactSound != null ? Mathf.Max(soundOnlyDelayTime, impactSound.length) : soundOnlyDelayTime;
            }
            else
            {
                // Calculate appropriate delay based on particle duration.
                float particleDuration = GetLongestParticleDuration();
                float soundDuration = impactSound != null ? impactSound.length : 0f;
                delay = Mathf.Max(particleDuration, soundDuration);
            }
            
            StartCoroutine(ReturnToPoolAfterDelay(delay));
        }

        public void Initialize(float projectileSpeed, float projectileLifetime)
        {
            _speed = projectileSpeed;
            _lifetime = projectileLifetime;
            _hasCollided = false;
            
            // Show all visual elements.
            ShowAllVisuals();
            
            // Reset physics.
            EnablePhysics();
            
            if (_rigidbody != null)
            {
                _rigidbody.linearVelocity = transform.forward * _speed;
            }

            // Set layer.
            gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
    
            // Return to pool after lifetime.
            StartCoroutine(ReturnToPoolAfterDelay(_lifetime));
        }
        
        private void HideAllVisuals()
        {
            // Disable all renderers.
            foreach (Renderer renderer in _renderers)
            {
                renderer.enabled = false;
            }
            
            // Stop and clear all particle systems.
            foreach (ParticleSystem ps in _particleSystems)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            
            // Disable trail renderers and clear them.
            foreach (TrailRenderer trail in _trailRenderers)
            {
                trail.emitting = false;
                trail.Clear();
            }
        }
        
        private void ShowAllVisuals()
        {
            // Enable all renderers.
            foreach (Renderer renderer in _renderers)
            {
                renderer.enabled = true;
            }
            
            // Restart particle systems.
            RestartAllParticles();
        }
        
        private void StopAllParticleEmission()
        {
            foreach (ParticleSystem ps in _particleSystems)
            {
                var emission = ps.emission;
                emission.enabled = false;
            }
            
            foreach (TrailRenderer trail in _trailRenderers)
            {
                trail.emitting = false;
                // Optional: make trail fade out faster.
                trail.time = Mathf.Min(0.5f, trail.time);
            }
        }
        
        private void RestartAllParticles()
        {
            foreach (ParticleSystem ps in _particleSystems)
            {
                ps.Clear();
                ps.Play();
                var emission = ps.emission;
                emission.enabled = true;
            }
            
            foreach (TrailRenderer trail in _trailRenderers)
            {
                trail.Clear();
                trail.emitting = true;
            }
        }
        
        private float GetLongestParticleDuration()
        {
            float longestDuration = 0f;
            
            foreach (ParticleSystem ps in _particleSystems)
            {
                // Get the main module for duration info.
                var main = ps.main;
                float duration = main.duration;
                
                // Add the maximum particle lifetime.
                if (main.startLifetime.mode == ParticleSystemCurveMode.Constant)
                {
                    duration += main.startLifetime.constant;
                }
                else if (main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
                {
                    duration += main.startLifetime.constantMax;
                }
                else
                {
                    // For other modes, use a reasonable default.
                    duration += 2.0f;
                }
                
                if (duration > longestDuration)
                {
                    longestDuration = duration;
                }
            }
            
            // Consider trail renderers too.
            foreach (TrailRenderer trail in _trailRenderers)
            {
                if (trail.time > longestDuration)
                {
                    longestDuration = trail.time;
                }
            }
            
            return longestDuration;
        }
        
        private void EnablePhysics()
        {
            if (_collider != null) _collider.enabled = true;
            
            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = false;
                _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
        }
        
        private void DisablePhysics()
        {
            if (_collider != null) _collider.enabled = false;
            
            if (_rigidbody != null)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.isKinematic = true;
            }
        }
        
        private void HandleImpactEffects(Collision collision)
        {
            if (impactEffectPrefab != null && collision.contactCount > 0)
            {
                ContactPoint contact = collision.contacts[0];
                GameObject effect = Instantiate(impactEffectPrefab, 
                    contact.point, 
                    Quaternion.LookRotation(contact.normal));
                
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
                Destroy(gameObject);
            }
        }
        
        public void PlayShotSound()
        {
            if (_audioSource != null && projectileSound != null)
            {
                _audioSource.PlayOneShot(projectileSound);
            }
        }

    }
}