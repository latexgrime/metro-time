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
}