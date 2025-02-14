using _Scripts.StatusSystem;
using UnityEngine;

namespace _Scripts.Enemy.Enemy_Types
{
    public class SlowingRangedEnemy : BaseEnemy
    {
        [Header("- Attack Settings")]
        [SerializeField] private GameObject slowProjectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float projectileSpeed = 15f;
        [SerializeField] private float attackCooldown = 3f;
        [SerializeField] private float projectileLifetime = 5f;
        [SerializeField] private int projectilesPerBurst = 3;
        [SerializeField] private float burstDelay = 0.2f;
        
        private float _nextAttackTime;
        private int _projectilesShot;
        private float _lastProjectileTime;

        protected override void Attack()
        {
            if (Time.time >= _nextAttackTime)
            {
                _projectilesShot = 0;
                _lastProjectileTime = Time.time;
            }
            
            // Handle burst firing.
            if (_projectilesShot < projectilesPerBurst && Time.time >= _lastProjectileTime + burstDelay)
            {
                FireProjectile();
                _projectilesShot++;
                _lastProjectileTime = Time.time;
                
                if (_projectilesShot >= projectilesPerBurst)
                {
                    _nextAttackTime = Time.time + attackCooldown;
                }
            }
        }

        private void FireProjectile()
        {
            // Face the player.
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            transform.forward = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);

            // Add some spread to the shots.
            Vector3 spreadDirection = Quaternion.Euler(0, Random.Range(-15f, 15f), 0) * directionToPlayer;

            // Create projectile.
            GameObject projectile = Instantiate(slowProjectilePrefab, 
                projectileSpawnPoint.position, 
                Quaternion.LookRotation(spreadDirection));

            // Set up projectile.
            SlowProjectile slowProjectile = projectile.GetComponent<SlowProjectile>();
            if (slowProjectile != null)
            {
                slowProjectile.Initialize(projectileSpeed, projectileLifetime);
            }
        }
    }

    // Projectile class.
    public class SlowProjectile : MonoBehaviour
    {
        private float speed;
        private float lifetime;
        private float damage = 5f;
        
        private void OnCollisionEnter(Collision collision)
        {
            // Check if the player was hit.
            if (collision.gameObject.CompareTag("Player"))
            {
                // Apply slow effect.
                StatusEffectHandler statusHandler = collision.gameObject.GetComponent<StatusEffectHandler>();
                if (statusHandler != null)
                {
                    statusHandler.ApplySlowdown();
                }
            }

            // ADD SLOW EFFECT PARTICLE SYSTEM OR VFX HERE.

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