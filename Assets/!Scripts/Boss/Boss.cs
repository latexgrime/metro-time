using System.Collections;
using System.Collections.Generic;
using _Scripts.AmmoDrop;
using _Scripts.Enemy;
using _Scripts.Spawners;
using UnityEngine;

namespace _Scripts.Boss
{
    [RequireComponent(typeof(BossProjectileSpawner))]
    public class Boss : BaseEnemy
    {
        [Header("- Boss Attack Phases")]
        [SerializeField] private float attackDuration = 10f;
        [SerializeField] private Vector2 cooldownDurationRange = new Vector2(3f, 7f);

        [Header("- Boss Specific Settings")]
        [SerializeField] private GameObject[] projectilePrefabs;
        [SerializeField] private Transform[] projectileSpawnPoints;
        [SerializeField] private float globalAttackCooldown = 2f;
        [SerializeField] private int ammoSpawnLimit = 4;

        [Header("- Shield Visual Effects")]
        [SerializeField] private GameObject shieldEffect;
        [SerializeField] private GameObject[] shieldActivateEffects;
        [SerializeField] private GameObject[] shieldDeactivateEffects;
        [SerializeField] private Transform effectSpawnPoint;

        [Header("- Shield Audio Effects")]
        [SerializeField] private AudioClip shieldActivateSound;
        [SerializeField] private AudioClip shieldDeactivateSound;
        [SerializeField] [Range(0f, 1f)] private float shieldSoundVolume = 0.7f;

        // Boss Phases & Attack Control.
        private BossPhase _currentPhase = BossPhase.Cooldown;
        private float _phaseStartTime;
        private float attackTimer;
        private bool isAttacking = false;
        private int _currentAttackPhase = 0;
        private float _currentCooldownDuration;
        
        // Components.
        private BossProjectileSpawner _projectileSpawner;
        private Coroutine _currentAttackCoroutine;
        private AudioSource _audioSource;
        private EnemySpawner _enemySpawner;
        private AmmoSpawner _ammoSpawner;

        protected override void Start()
        {
            base.Start();
            
            // Initialize components.
            ammoDropper = null;
            if (agent != null) agent.enabled = false;
            transform.position = startPosition;
            currentShield = maxShield;
            
            _projectileSpawner = GetComponent<BossProjectileSpawner>();
            if (_projectileSpawner == null)
            {
                _projectileSpawner = gameObject.AddComponent<BossProjectileSpawner>();
            }

            // Set up audio source.
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.spatialBlend = 1f; // 3D sound.
                _audioSource.rolloffMode = AudioRolloffMode.Linear;
                _audioSource.minDistance = 5f;
                _audioSource.maxDistance = 50f;
            }
            
            // If no effect spawn point specified, use this transform.
            if (effectSpawnPoint == null)
            {
                effectSpawnPoint = transform;
            }
            
            _enemySpawner = GetComponent<EnemySpawner>();
            _ammoSpawner = GetComponent<AmmoSpawner>();
            
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
            //HandleDebugKeys();
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
            if (Time.time - _phaseStartTime >= _currentCooldownDuration)
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
            
            // Shield activation effects.
            ActivateShield();
            SpawnEnemies();
        }

        private void StartCooldownPhase()
        {
            _currentPhase = BossPhase.Cooldown;
            _phaseStartTime = Time.time;
            isAttacking = false;
            
            // Generate random cooldown duration.
            _currentCooldownDuration = Random.Range(cooldownDurationRange.x, cooldownDurationRange.y);
            
            // Stop any ongoing attack coroutines.
            if (_currentAttackCoroutine != null)
            {
                StopCoroutine(_currentAttackCoroutine);
                _currentAttackCoroutine = null;
            }
            
            // Shield deactivation effects.
            DeactivateShield();
            SpawnAmmo();        
        }

        private void ActivateShield()
        {
            // Visual effects.
            if (shieldEffect != null)
            {
                shieldEffect.SetActive(true);
            }
            
            if (shieldActivateEffects != null && shieldActivateEffects.Length > 0)
            {
                int effectIndex = Random.Range(0, shieldActivateEffects.Length);
                if (shieldActivateEffects[effectIndex] != null)
                {
                    Instantiate(shieldActivateEffects[effectIndex], effectSpawnPoint.position, effectSpawnPoint.rotation);
                }
            }
            
            // Audio effects.
            if (_audioSource != null && shieldActivateSound != null)
            {
                _audioSource.PlayOneShot(shieldActivateSound, shieldSoundVolume);
            }
        }

        private void DeactivateShield()
        {
            // Visual effects.
            if (shieldEffect != null)
            {
                shieldEffect.SetActive(false);
            }
            
            if (shieldDeactivateEffects != null && shieldDeactivateEffects.Length > 0)
            {
                int effectIndex = Random.Range(0, shieldDeactivateEffects.Length);
                if (shieldDeactivateEffects[effectIndex] != null)
                {
                    Instantiate(shieldDeactivateEffects[effectIndex], effectSpawnPoint.position, effectSpawnPoint.rotation);
                }
            }
            
            // Audio effects.
            if (_audioSource != null && shieldDeactivateSound != null)
            {
                _audioSource.PlayOneShot(shieldDeactivateSound, shieldSoundVolume);
            }
        }

        private void StartAttackCycle()
        {
            isAttacking = true;
            attackTimer = 5f; // Each attack lasts 5 seconds(maybe I'll add it to the inspector later).

            // Stop any previous attack coroutine.
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
            // Not used in the boss - the boss uses specialized attack patterns instead.
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
            if (_projectileSpawner != null && projectilePrefabs.Length > 0)
            {
                string projectileType = projectilePrefabs[0].name;
                yield return _projectileSpawner.SpawnProjectilesInPattern(
                    PatternType.Circular,
                    projectileType,
                    projectileSpawnPoints,
                    5f, // Duration.
                    0.2f, // Fire rate.
                    player
                );
            }
        }

        private IEnumerator ConcentratedBurstPattern()
        {
            if (_projectileSpawner != null && projectilePrefabs.Length > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    string projectileType = projectilePrefabs[Random.Range(0, projectilePrefabs.Length)].name;
                    yield return _projectileSpawner.SpawnProjectilesInPattern(
                        PatternType.Targeted,
                        projectileType,
                        projectileSpawnPoints,
                        1f,
                        0.2f,
                        player
                    );
                    
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        private IEnumerator AreaDenialPattern()
        {
            if (_projectileSpawner != null && projectilePrefabs.Length > 0)
            {
                string projectileType = projectilePrefabs[Random.Range(0, projectilePrefabs.Length)].name;
                yield return _projectileSpawner.SpawnProjectilesInPattern(
                    PatternType.Random,
                    projectileType,
                    projectileSpawnPoints,
                    5f, // duration
                    0.5f, // slower fire rate
                    null
                );
            }
        }
        
        // Spawn methods.
        private void SpawnEnemies()
        {
            if (_enemySpawner != null)
            {
                _enemySpawner.SpawnEnemies();
            }
            else
            {
                Debug.LogError("EnemySpawner is missing.");
            }
        }

        private void SpawnAmmo()
        {
            int ammoCount = FindObjectsOfType<AmmoPickup>().Length;

            if (ammoCount < ammoSpawnLimit)
            {
                if (_ammoSpawner != null)
                {
                    _ammoSpawner.SpawnAmmo();
                }
                else
                {
                    Debug.LogError("AmmoSpawner is missing.");
                }
            }
            else
            {
                Debug.Log("Ammo not spawned - limit reached (5).");
            }
        }


        // DEBUG
        
        private void HandleDebugKeys()
        {
            if (Input.GetKeyDown(KeyCode.I)) TriggerAttackPhase(0); // Bullet Hell.
            if (Input.GetKeyDown(KeyCode.O)) TriggerAttackPhase(1); // Burst.
            if (Input.GetKeyDown(KeyCode.P)) TriggerAttackPhase(2); // Area Denial.
        }
        
        public void TriggerAttackPhase(int attackPhase)
        {
            // To prevent interrupting active attacks.
            if (_currentPhase != BossPhase.Cooldown) return; 

            _currentPhase = BossPhase.Attack;
            isAttacking = false;
            _currentAttackPhase = attackPhase;

            StartAttackCycle();
        }
        
        private enum BossPhase
        {
            Attack,
            Cooldown
        }
    }
}