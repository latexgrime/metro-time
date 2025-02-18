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

        // Boss Phase States.
        private BossPhase _currentPhase = BossPhase.Cooldown;
        private float _phaseStartTime;
        private float _nextAttackTime;
        private int _currentAttackPattern;

        [Header("- Secondary Shield Settings")]
        [SerializeField] private bool isSecondaryShieldActive = true;

        // Events for UI or other systems to hook into.
        public event System.Action OnAttackPhaseStart;
        public event System.Action OnCooldownPhaseStart;

        protected override void Start()
        {
            base.Start();

            // Remove ammo drop capability.
            ammoDropper = null;

            // Disable movement for stationary boss.
            if (agent != null)
            {
                agent.enabled = false;
            }

            // Ensure the boss is always at a fixed position.
            transform.position = startPosition;

            // Initialize with max shield.
            currentShield = maxShield;

            // Start in cooldown phase.
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

            // Debug keys for attack patterns.
            if (Input.GetKeyDown(KeyCode.I)) DebugTriggerAttackPattern(0); // Rotating Bullet Hell
            if (Input.GetKeyDown(KeyCode.O)) DebugTriggerAttackPattern(1); // Concentrated Burst
            if (Input.GetKeyDown(KeyCode.P)) DebugTriggerAttackPattern(2); // Area Denial
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
                ExecuteAttackPattern();
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
            isSecondaryShieldActive = true;

            EnableEnemySpawners(false); // Disable enemy spawners during attack phase.

            OnAttackPhaseStart?.Invoke();
        }

        private void StartCooldownPhase()
        {
            _currentPhase = BossPhase.Cooldown;
            _phaseStartTime = Time.time;
            isSecondaryShieldActive = false;

            EnableEnemySpawners(true); // Enable enemy spawners during cooldown phase.

            OnCooldownPhaseStart?.Invoke();
        }

        private void EnableEnemySpawners(bool enable)
        {
            Spawner[] spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
            foreach (var spawner in spawners)
            {
                spawner.enabled = enable;
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

        private void ExecuteAttackPattern()
        {
            switch (_currentAttackPattern)
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

            _currentAttackPattern = (_currentAttackPattern + 1) % 3;
            _nextAttackTime = Time.time + globalAttackCooldown;
        }

        // Debugging Methods.
        public void DebugTriggerAttackPattern(int patternIndex)
        {
            if (patternIndex < 0 || patternIndex > 2)
            {
                Debug.LogWarning("Invalid attack pattern index. Choose between 0 and 2.");
                return;
            }

            _currentAttackPattern = patternIndex;
            ExecuteAttackPattern();
        }

        public void DebugForceCooldown()
        {
            StartCooldownPhase();
            Debug.Log("Boss forced into cooldown mode.");
        }

        public void DebugForceAttack()
        {
            StartAttackPhase();
            Debug.Log("Boss forced into attack mode.");
        }

        private void RotatingBulletHellPattern()
        {
            Debug.Log("Executing Rotating Bullet Hell Pattern.");

            int projectileCount = Mathf.Min(maxSimultaneousProjectiles, projectilePrefabs.Length);

            for (int i = 0; i < projectileCount; i++)
            {
                Transform spawnPoint = projectileSpawnPoints[i % projectileSpawnPoints.Length];
                GameObject projectilePrefab = projectilePrefabs[i % projectilePrefabs.Length];

                float angle = (Time.time * rotationSpeed) + (i * (360f / projectileCount));
                Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;

                GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.LookRotation(direction));

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 spreadDirection = Quaternion.Euler(Random.Range(-bulletSpread, bulletSpread), 0, Random.Range(-bulletSpread, bulletSpread)) * direction;
                    rb.linearVelocity = spreadDirection * 10f;
                }
            }
        }

        private void ConcentratedBurstPattern()
        {
            Debug.Log("Executing Concentrated Burst Pattern.");

            int burstCount = 5;
            for (int i = 0; i < burstCount; i++)
            {
                Transform spawnPoint = projectileSpawnPoints[0];
                GameObject projectilePrefab = projectilePrefabs[0];

                Vector3 directionToPlayer = (player.position - spawnPoint.position).normalized;
                Vector3 spreadDirection = Quaternion.Euler(Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread), 0) * directionToPlayer;

                GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.LookRotation(spreadDirection));

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

            int projectileCount = 10;
            for (int i = 0; i < projectileCount; i++)
            {
                Transform spawnPoint = projectileSpawnPoints[Random.Range(0, projectileSpawnPoints.Length)];
                GameObject projectilePrefab = projectilePrefabs[Random.Range(0, projectilePrefabs.Length)];

                Vector3 randomDirection = Random.insideUnitSphere;
                randomDirection.y = 0;
                randomDirection.Normalize();

                GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.LookRotation(randomDirection));

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = randomDirection * 8f;
                }
            }
        }

        private enum BossPhase
        {
            Attack,
            Cooldown
        }
    }
}
