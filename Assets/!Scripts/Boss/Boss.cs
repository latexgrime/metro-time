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
        public event System.Action OnAttackPhaseStart;
        public event System.Action OnCooldownPhaseStart;


        [Header("- Boss Specific Settings")]
        [SerializeField] private GameObject[] projectilePrefabs;
        [SerializeField] private Transform[] projectileSpawnPoints;
        [SerializeField] private float globalAttackCooldown = 2f;
        [SerializeField] private int maxSimultaneousProjectiles = 20;

        [Header("- Bullet Hell Settings")]
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private float bulletSpread = 15f;

        private BossPhase _currentPhase = BossPhase.Cooldown;
        private float _phaseStartTime;
        private float _nextAttackTime;
        private int _currentAttackPattern;

        [Header("- Shield Visual Effect")]
        [SerializeField] private GameObject shieldEffect;
        [SerializeField] private GameObject shieldBreakEffect;

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

            if (Input.GetKeyDown(KeyCode.I)) DebugTriggerAttackPattern(0);
            if (Input.GetKeyDown(KeyCode.O)) DebugTriggerAttackPattern(1);
            if (Input.GetKeyDown(KeyCode.P)) DebugTriggerAttackPattern(2);
        }

        private void HandleAttackPhase()
        {
            if (Time.time - _phaseStartTime >= attackDuration)
            {
                StartCooldownPhase();
                return;
            }

            if (Time.time >= _nextAttackTime)
            {
                //ExecuteAttackPattern();
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
            if (shieldEffect != null) shieldEffect.SetActive(true);
            EnableEnemySpawners(false);
            OnAttackPhaseStart?.Invoke();
        }

        private void StartCooldownPhase()
        {
            _currentPhase = BossPhase.Cooldown;
            _phaseStartTime = Time.time;
            if (shieldEffect != null) shieldEffect.SetActive(false);
            if (shieldBreakEffect != null) Instantiate(shieldBreakEffect, transform.position, Quaternion.identity);
            EnableEnemySpawners(true);
            OnCooldownPhaseStart?.Invoke();
        }

        private void EnableEnemySpawners(bool enable)
        {
            Spawner[] spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
            foreach (var spawner in spawners) spawner.enabled = enable;
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

        private IEnumerator AttackSequence()
        {
            while (true) 
            {
                yield return StartCoroutine(RotatingBulletHellPattern()); // Bullet Hell (5 secs).
                //yield return StartCoroutine(ExecuteAttackMultipleTimes(ConcentratedBurstPattern, 2, 5f)); // Burst (twice for 5 secs).
                //yield return StartCoroutine(ExecuteAttackMultipleTimes(AreaDenialPattern, Random.Range(1, 4), 5f)); // Area Denial (random times for 5 secs).
        
                StartCooldownPhase(); // Enter idle phase.
                yield return new WaitForSeconds(cooldownDuration); // Wait before restarting.
        
                StartAttackPhase(); // Restart cycle.
            }
        }


        // **Rotating Bullet Hell**
        private IEnumerator RotatingBulletHellPattern()
        {
            Debug.Log("Executing Rotating Bullet Hell Pattern.");
    
            float attackDuration = 5f; 
            float fireRate = 0.2f;
            float elapsedTime = 0f;

            while (elapsedTime < attackDuration)
            {
                int projectileCount = 12;
                for (int i = 0; i < projectileCount; i++)
                {
                    Transform spawnPoint = projectileSpawnPoints[i % projectileSpawnPoints.Length];
                    GameObject projectilePrefab = projectilePrefabs[i % projectilePrefabs.Length];
                    string projectileType = projectilePrefab.name;

                    GameObject projectile = FindObjectOfType<ProjectilePool>()?.GetProjectile(projectileType, spawnPoint.position, Quaternion.identity);
                    if (projectile == null) continue;

                    float angle = (Time.time * rotationSpeed) + (i * (360f / projectileCount));
                    Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;

                    projectile.transform.rotation = Quaternion.LookRotation(direction);
                    Rigidbody rb = projectile.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = projectile.transform.forward * 10f;
                    }
                }

                yield return new WaitForSeconds(fireRate);
                elapsedTime += fireRate;
            }
        }

        // **Concentrated Burst**
        private void ConcentratedBurstPattern()
        {
            Debug.Log("Executing Concentrated Burst Pattern.");

            int burstCount = 5;
            for (int i = 0; i < burstCount; i++)
            {
                Transform spawnPoint = projectileSpawnPoints[0];
                GameObject projectilePrefab = projectilePrefabs[0];
                string projectileType = projectilePrefab.name;

                GameObject projectile = FindObjectOfType<ProjectilePool>()?.GetProjectile(projectileType, spawnPoint.position, Quaternion.identity);
                if (projectile == null) continue;

                Vector3 directionToPlayer = (player.position - spawnPoint.position).normalized;
                Vector3 spreadDirection = Quaternion.Euler(Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread), 0) * directionToPlayer;

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = spreadDirection * 15f;
                }
            }
        }

        // **Area Denial**
        private void AreaDenialPattern()
        {
            Debug.Log("Executing Area Denial Pattern.");

            int projectileCount = 10;
            for (int i = 0; i < projectileCount; i++)
            {
                Transform spawnPoint = projectileSpawnPoints[Random.Range(0, projectileSpawnPoints.Length)];
                GameObject projectilePrefab = projectilePrefabs[Random.Range(0, projectilePrefabs.Length)];
                string projectileType = projectilePrefab.name;

                GameObject projectile = FindObjectOfType<ProjectilePool>()?.GetProjectile(projectileType, spawnPoint.position, Quaternion.identity);
                if (projectile == null) continue;

                Vector3 randomDirection = Random.insideUnitSphere;
                randomDirection.y = 0;
                randomDirection.Normalize();

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = randomDirection * 8f;
                }
            }
        }

        // DEBUG
        public void DebugTriggerAttackPattern(int patternIndex)
        {
            if (patternIndex < 0 || patternIndex > 2)
            {
                Debug.LogWarning("Invalid attack pattern index. Choose between 0 and 2.");
                return;
            }

            _currentAttackPattern = patternIndex;
            //ExecuteAttackPattern();
        }

        
        private enum BossPhase
        {
            Attack,
            Cooldown
        }
    }
}
