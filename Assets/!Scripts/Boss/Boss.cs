using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemy;
using UnityEngine;

namespace _Scripts.Boss
{
    public class Boss : BaseEnemy
    {
        [Header("- Boss Attack Phases")]
        [SerializeField] private float attackDuration = 10f;
        [SerializeField] private float cooldownDuration = 5f;

        [Header("- Boss Specific Settings")]
        [SerializeField] private GameObject[] projectilePrefabs;
        [SerializeField] private Transform[] projectileSpawnPoints;
        [SerializeField] private float globalAttackCooldown = 2f;
        [SerializeField] private int maxSimultaneousProjectiles = 20;

        [Header("- Bullet Hell Settings")]
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private float bulletSpread = 15f;

        [Header("- Shield Visual Effect")]
        [SerializeField] private GameObject shieldEffect;
        [SerializeField] private GameObject shieldBreakEffect;

        // Boss Phases & Attack Control
        private BossPhase _currentPhase = BossPhase.Cooldown;
        private float _phaseStartTime;
        private float attackTimer;
        private bool isAttacking = false;
        private int _currentAttackPhase = 0;

        protected override void Start()
        {
            base.Start();
            ammoDropper = null;
            if (agent != null) agent.enabled = false;
            transform.position = startPosition;
            currentShield = maxShield;
            StartCooldownPhase();
        }

        protected override void Update()
        {
            if (isDeactivated) return;

            switch (_currentPhase)
            {
                case BossPhase.Attack:
                    HandleAttackPhase();
                    break;
                case BossPhase.Cooldown:
                    HandleCooldownPhase();
                    break;
            }

            // Debug Keys for Manual Attack Testing
            if (Input.GetKeyDown(KeyCode.I)) TriggerAttackPhase(0); // Bullet Hell. 
            if (Input.GetKeyDown(KeyCode.O)) TriggerAttackPhase(1); // Burst.
            if (Input.GetKeyDown(KeyCode.P)) TriggerAttackPhase(2); // Area Denial.
        }

        private void HandleAttackPhase()
        {
            if (!isAttacking)
            {
                StartAttackCycle();
            }

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                _currentAttackPhase++;
                if (_currentAttackPhase > 2) // After all attacks, go to cooldown.
                {
                    StartCooldownPhase();
                }
                else
                {
                    StartAttackCycle(); // Move to next attack.
                }
            }
        }

        private void HandleCooldownPhase()
        {
            if (Time.time - _phaseStartTime >= cooldownDuration)
            {
                StartAttackPhase();
            }
        }

        private void StartAttackPhase()
        {
            _currentPhase = BossPhase.Attack;
            _phaseStartTime = Time.time;
            isAttacking = false;
            _currentAttackPhase = 0; // Reset attack cycle.
            if (shieldEffect != null) shieldEffect.SetActive(true);
            EnableEnemySpawners(false);
        }

        private void StartCooldownPhase()
        {
            _currentPhase = BossPhase.Cooldown;
            _phaseStartTime = Time.time;
            isAttacking = false;
            if (shieldEffect != null) shieldEffect.SetActive(false);
            if (shieldBreakEffect != null) Instantiate(shieldBreakEffect, transform.position, Quaternion.identity);
            EnableEnemySpawners(true);
        }

        private void EnableEnemySpawners(bool enable)
        {
            Spawner[] spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
            foreach (var spawner in spawners) spawner.enabled = enable;
        }

        private void StartAttackCycle()
        {
            isAttacking = true;
            attackTimer = 5f; // Each attack lasts 5 seconds.

            switch (_currentAttackPhase)
            {
                case 0:
                    RotatingBulletHellPattern();
                    break;
                case 1:
                    ConcentratedBurstPattern();
                    break;
                case 2:
                    AreaDenialPattern();
                    break;
            }
        }

        protected override void Attack()
        {
            throw new System.NotImplementedException();
        }

        public override void TakeShieldDamage(float damage)
        {
            if (_currentPhase == BossPhase.Cooldown)
            {
                base.TakeShieldDamage(damage);
            }
        }

        private void RotatingBulletHellPattern()
        {
            Debug.Log("Executing Rotating Bullet Hell Pattern.");
    
            float attackDuration = 5f; // Duration of Bullet Hell.
            float fireRate = 0.2f; // Time between shots.

            StartCoroutine(ContinuousBulletHell(attackDuration, fireRate));
        }

        private IEnumerator ContinuousBulletHell(float duration, float fireRate)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                int projectileCount = 12;

                for (int i = 0; i < projectileCount; i++)
                {
                    Transform spawnPoint = projectileSpawnPoints[i % projectileSpawnPoints.Length];
                    GameObject projectilePrefab = projectilePrefabs[i % projectilePrefabs.Length];
                    string projectileType = projectilePrefab.name;

                    float angle = (Time.time * rotationSpeed) + (i * (360f / projectileCount));
                    Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            
                    // Create the projectile with the correct initial rotation.
                    GameObject projectile = FindObjectOfType<ProjectilePool>()?.GetProjectile(
                        projectileType, 
                        spawnPoint.position, 
                        Quaternion.LookRotation(direction)
                    );
            
                    if (projectile == null) continue;

                    // Apply velocity in the same direction.
                    Rigidbody rb = projectile.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = direction * 10f;
                    }
                }

                yield return new WaitForSeconds(fireRate);
                elapsedTime += fireRate;
            }
        }

        private void ConcentratedBurstPattern()
        {
            Debug.Log("Executing Concentrated Burst Pattern.");
            for (int i = 0; i < 5; i++)
            {
                Transform spawnPoint = projectileSpawnPoints[0];
                GameObject projectilePrefab = projectilePrefabs[0];
                string projectileType = projectilePrefab.name;

                Vector3 directionToPlayer = (player.position - spawnPoint.position).normalized;
                Vector3 spreadDirection = Quaternion.Euler(
                    Random.Range(-bulletSpread, bulletSpread), 
                    Random.Range(-bulletSpread, bulletSpread), 
                    0
                ) * directionToPlayer;

                // Create projectile with the correct rotation first.
                GameObject projectile = FindObjectOfType<ProjectilePool>()?.GetProjectile(
                    projectileType, 
                    spawnPoint.position, 
                    Quaternion.LookRotation(spreadDirection)
                );
        
                if (projectile == null) continue;

                // Apply velocity in the same direction.
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = spreadDirection * 15f;
                }
            }
        }

        private void AreaDenialPattern()
        {
            Debug.Log("Executing Area Denial Pattern.");
            for (int i = 0; i < 10; i++)
            {
                Transform spawnPoint = projectileSpawnPoints[Random.Range(0, projectileSpawnPoints.Length)];
                GameObject projectilePrefab = projectilePrefabs[Random.Range(0, projectilePrefabs.Length)];
                string projectileType = projectilePrefab.name;

                Vector3 randomDirection = Random.insideUnitSphere;
                randomDirection.y = 0;
                randomDirection.Normalize();

                // Create projectile with the correct rotation first.
                GameObject projectile = FindObjectOfType<ProjectilePool>()?.GetProjectile(
                    projectileType, 
                    spawnPoint.position, 
                    Quaternion.LookRotation(randomDirection)
                );
        
                if (projectile == null) continue;

                // Apply velocity in the same direction.
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = randomDirection * 8f;
                }
            }
        }

        // DEBUG
        public void TriggerAttackPhase(int attackPhase)
        {
            if (_currentPhase != BossPhase.Cooldown) return; // Prevent interrupting active attacks.

            _currentPhase = BossPhase.Attack;
            isAttacking = false;
            _currentAttackPhase = attackPhase; // Set specific attack phase.

            StartAttackCycle();
        }

        
        private enum BossPhase
        {
            Attack,
            Cooldown
        }
    }
}
