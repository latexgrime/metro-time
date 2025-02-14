using _Scripts.StatusSystem;
using UnityEngine;

namespace _Scripts.Enemy.Enemy_Types
{
    public class ElectricRangedEnemy : BaseEnemy
    {
        [Header("- Attack Settings")]
        [SerializeField] private GameObject electricProjectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float projectileSpeed = 20f;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private float projectileLifetime = 5f;
        
        private float _nextAttackTime;

        protected override void Attack()
        {
            if (Time.time >= _nextAttackTime)
            {
                // Face the player.
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                transform.forward = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);

                // Create projectile.
                GameObject projectile = Instantiate(electricProjectilePrefab, 
                    projectileSpawnPoint.position, 
                    Quaternion.LookRotation(directionToPlayer));

                // Set up projectile.
                ElectricProjectile electricProjectile = projectile.GetComponent<ElectricProjectile>();
                if (electricProjectile != null)
                {
                    electricProjectile.Initialize(projectileSpeed, projectileLifetime);
                }

                // Set next attack time
                _nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    // Projectile class.
    public class ElectricProjectile : MonoBehaviour
    {
        private float speed;
        private float lifetime;
        // Shield damage to player if they have shield.
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