using System.Collections;
using UnityEngine;

namespace _Scripts.Boss
{
    public class BossProjectileSpawner : MonoBehaviour
    {
        [Header("- References")]
        [SerializeField] private ProjectilePool projectilePool;
        
        [Header("- Projectile Settings")]
        [SerializeField] private float defaultProjectileSpeed = 10f;
        [SerializeField] private float defaultProjectileLifetime = 3f;
        [SerializeField] private float defaultProjectileDamage = 10f;

        private void Awake()
        {
            // Find the projectile pool if not assigned.
            if (projectilePool == null)
            {
                projectilePool = FindObjectOfType<ProjectilePool>();
                if (projectilePool == null)
                {
                    Debug.LogError("ProjectilePool not found! Boss attacks will not work properly.");
                }
            }
        }

        public GameObject SpawnProjectile(string projectileType, Vector3 position, Vector3 direction, float speed = 0f)
        {
            if (projectilePool == null) return null;

            // Get a projectile from the pool.
            GameObject projectile = projectilePool.GetProjectile(
                projectileType,
                position,
                Quaternion.LookRotation(direction)
            );

            if (projectile == null) return null;

            // Configure the projectile.
            PooledProjectile pooledProjectile = projectile.GetComponent<PooledProjectile>();
            if (pooledProjectile != null)
            {
                // Apply custom settings if provided.
                if (speed <= 0) speed = defaultProjectileSpeed;
                
                pooledProjectile.SetVelocity(direction.normalized * speed);
                pooledProjectile.SetLifetime(defaultProjectileLifetime);
                pooledProjectile.SetDamage(defaultProjectileDamage);
            }
            else
            {
                // Fallback for projectiles without the PooledProjectile component.
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = direction.normalized * (speed > 0 ? speed : defaultProjectileSpeed);
                }
            }

            return projectile;
        }

        public IEnumerator SpawnProjectilesInPattern(
            PatternType patternType, 
            string projectileType,
            Transform[] spawnPoints, 
            float duration, 
            float fireRate,
            Transform target = null)
        {
            float elapsedTime = 0f;
            int spawnPointIndex = 0;
            
            while (elapsedTime < duration)
            {
                switch (patternType)
                {
                    case PatternType.Circular:
                        SpawnCircularPattern(projectileType, spawnPoints, 12);
                        break;
                        
                    case PatternType.Targeted:
                        if (target != null)
                        {
                            SpawnTargetedPattern(projectileType, spawnPoints[spawnPointIndex], target, 5, 15f);
                        }
                        break;
                        
                    case PatternType.Random:
                        SpawnRandomPattern(projectileType, spawnPoints, 8);
                        break;
                }
                
                spawnPointIndex = (spawnPointIndex + 1) % spawnPoints.Length;
                yield return new WaitForSeconds(fireRate);
                elapsedTime += fireRate;
            }
        }

        private void SpawnCircularPattern(string projectileType, Transform[] spawnPoints, int projectileCount)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = (Time.time * 30f) + (i * (360f / projectileCount));
                Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                
                SpawnProjectile(projectileType, spawnPoint.position, direction, 10f);
            }
        }

        private void SpawnTargetedPattern(
            string projectileType, 
            Transform spawnPoint, 
            Transform target, 
            int projectileCount,
            float spreadAngle)
        {
            if (target == null) return;
            
            Vector3 directionToTarget = (target.position - spawnPoint.position).normalized;
            
            for (int i = 0; i < projectileCount; i++)
            {
                // Calculate spread direction.
                Vector3 spreadDirection = Quaternion.Euler(
                    Random.Range(-spreadAngle, spreadAngle),
                    Random.Range(-spreadAngle, spreadAngle),
                    0
                ) * directionToTarget;
                
                SpawnProjectile(projectileType, spawnPoint.position, spreadDirection, 15f);
            }
        }

        private void SpawnRandomPattern(string projectileType, Transform[] spawnPoints, int projectileCount)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Vector3 randomDirection = Random.insideUnitSphere;
                randomDirection.y = Mathf.Abs(randomDirection.y) * 0.2f; // This is to limit how vertical the bullets can be shot at (for the future).
                randomDirection.Normalize();
                
                SpawnProjectile(projectileType, spawnPoint.position, randomDirection, 8f);
            }
        }
    }

    public enum PatternType
    {
        Circular,
        Targeted,
        Random
    }
}