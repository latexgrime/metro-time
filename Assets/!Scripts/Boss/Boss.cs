using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemy;
using UnityEngine;

namespace _Scripts.Boss
{
    [RequireComponent(typeof(BossProjectileSpawner))]
    public class Boss : BaseEnemy
    {
        [Header("- Boss Attack Phases")]
        [SerializeField] private float attackDuration = 10f;
        [SerializeField] private float cooldownDuration = 5f;

        [Header("- Boss Specific Settings")]
        [SerializeField] private GameObject[] projectilePrefabs;
        [SerializeField] private Transform[] projectileSpawnPoints;
        [SerializeField] private float globalAttackCooldown = 2f;

        [Header("- Shield Visual Effect")]
        [SerializeField] private GameObject shieldEffect;
        [SerializeField] private GameObject shieldBreakEffect;

        // Boss Phases & Attack Control.
        private BossPhase _currentPhase = BossPhase.Cooldown;
        private float _phaseStartTime;
        private float attackTimer;
        private bool isAttacking = false;
        private int _currentAttackPhase = 0;
        
        // Components.
        private BossProjectileSpawner _projectileSpawner;
        private Coroutine _currentAttackCoroutine;

        protected override void Start()
        {
            base.Start();
            ammoDropper = null;
            if (agent != null) agent.enabled = false;
            transform.position = startPosition;
            currentShield = maxShield;
            
            _projectileSpawner = GetComponent<BossProjectileSpawner>();
            if (_projectileSpawner == null)
            {
                _projectileSpawner = gameObject.AddComponent<BossProjectileSpawner>();
            }
            
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

            // Debug Keys for Manual Attack Testing.
            HandleDebugKeys();
        }

        private void HandleDebugKeys()
        {
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
            
            // Stop any ongoing attack coroutines.
            if (_currentAttackCoroutine != null)
            {
                StopCoroutine(_currentAttackCoroutine);
                _currentAttackCoroutine = null;
            }
            
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

            // Stop any previous attack coroutine
            if (_currentAttackCoroutine != null)
            {
                StopCoroutine(_currentAttackCoroutine);
            }

            // Start the appropriate attack pattern.
            switch (_currentAttackPhase)
            {
                case 0:
                    _currentAttackCoroutine = StartCoroutine(RotatingBulletHellPattern());
                    break;
                case 1:
                    _currentAttackCoroutine = StartCoroutine(ConcentratedBurstPattern());
                    break;
                case 2:
                    _currentAttackCoroutine = StartCoroutine(AreaDenialPattern());
                    break;
            }
        }

        protected override void Attack()
        {
            // Not used in the boss - we use specialized attack patterns instead.!!1
        }

        public override void TakeShieldDamage(float damage)
        {
            if (_currentPhase == BossPhase.Cooldown)
            {
                base.TakeShieldDamage(damage);
            }
        }

        private IEnumerator RotatingBulletHellPattern()
        {
            Debug.Log("Executing Rotating Bullet Hell Pattern.");
            
            if (_projectileSpawner != null && projectilePrefabs.Length > 0)
            {
                string projectileType = projectilePrefabs[0].name;
                yield return _projectileSpawner.SpawnProjectilesInPattern(
                    PatternType.Circular,
                    projectileType,
                    projectileSpawnPoints,
                    5f, // duration
                    0.2f, // fire rate
                    player
                );
            }
            else
            {
                Debug.LogError("ProjectileSpawner or projectile prefabs not found!");
                yield break;
            }
        }

        private IEnumerator ConcentratedBurstPattern()
        {
            Debug.Log("Executing Concentrated Burst Pattern.");
            
            if (_projectileSpawner != null && projectilePrefabs.Length > 0)
            {
                for (int i = 0; i < 3; i++) // Three waves of bursts.
                {
                    string projectileType = projectilePrefabs[Random.Range(0, projectilePrefabs.Length)].name;
                    _projectileSpawner.SpawnProjectilesInPattern(
                        PatternType.Targeted,
                        projectileType,
                        projectileSpawnPoints,
                        1f, // Short duration for each burst.
                        0.2f,
                        player
                    );
                    
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                Debug.LogError("ProjectileSpawner or projectile prefabs not found!");
                yield break;
            }
        }

        private IEnumerator AreaDenialPattern()
        {
            Debug.Log("Executing Area Denial Pattern.");
            
            if (_projectileSpawner != null && projectilePrefabs.Length > 0)
            {
                string projectileType = projectilePrefabs[Random.Range(0, projectilePrefabs.Length)].name;
                yield return _projectileSpawner.SpawnProjectilesInPattern(
                    PatternType.Random,
                    projectileType,
                    projectileSpawnPoints,
                    5f, // duration.
                    0.5f, // fire rate
                    null
                );
            }
            else
            {
                Debug.LogError("ProjectileSpawner or projectile prefabs not found!");
                yield break;
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