using _Scripts.Status_System;
using UnityEngine;

namespace _Scripts.Enemy.Enemy_Types.Electric__Stun_
{
    public class ElectricProjectile : MonoBehaviour
    {
        [Header("- Projectile Settings")] 
        private float _speed;
        private float _lifetime;
        private float _damage = 10f;

        [Header("- Status Effect")] 
        [SerializeField] private float stunBuildupAmount = 35f;

        [SerializeField] private AudioClip projectileSound;
        [SerializeField] private AudioClip impactSound;
        [SerializeField] private GameObject impactEffectPrefab;
        
        private bool _hasCollided = false;
        private MeshRenderer _meshRenderer;
        private Collider _collider;

        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
    
            // Ignore collisions between enemy projectiles and enemies.
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyProjectile"), LayerMask.NameToLayer("Enemy"), true);
    
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

            // Check if we hit the player.
            if (collision.gameObject.CompareTag("Player"))
            {
                StatusEffectManager statusManager = collision.gameObject.GetComponent<StatusEffectManager>();
                if (statusManager != null)
                {
                    statusManager.AddStunBuildup(stunBuildupAmount);
                }
            }

            // Disable Particle System emission.    
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Stop();
            }

            // Destroy after sound finishes.
            float soundDuration = impactSound != null ? impactSound.length : 0f;
            Destroy(gameObject, soundDuration);
        }

        public void Initialize(float projectileSpeed, float projectileLifetime)
        {
            _speed = projectileSpeed;
            _lifetime = projectileLifetime;
    
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = transform.forward * _speed;
            }

            // Automatically set the layer for dummy-proofing.
            gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
    
            Destroy(gameObject, _lifetime);
        }
        
        private void HandleImpactEffects(Collision collision)
        {
            if (impactEffectPrefab != null)
            {
                ContactPoint contact = collision.contacts[0];
                GameObject effect = Instantiate(impactEffectPrefab, 
                    contact.point, 
                    Quaternion.LookRotation(contact.normal));
            
                // Auto-destroy the effect after a few seconds [Workaround for the impact sfx not being played because the projectile gets destroyed instantly].
                Destroy(effect, 2f);
            }
        }
    }
}