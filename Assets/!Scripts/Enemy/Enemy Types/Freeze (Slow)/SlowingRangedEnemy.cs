using _Scripts.Status_System;
using UnityEngine;

namespace _Scripts.Enemy.Enemy_Types.Freeze__Slow_
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

        protected override void Start()
        {
            base.Start();
            animator = GetComponentInChildren<Animator>();
        }

        protected override void Update()
        {
            base.Update();
            UpdateAnimator();
            UpdateHoverMotion();
        }

        protected override void Attack()
        {
            if (Time.time >= _nextAttackTime)
            {
                _projectilesShot = 0;
                _lastProjectileTime = Time.time;
            }
            
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
            if (slowProjectilePrefab == null || projectileSpawnPoint == null)
            {
                Debug.LogWarning("Missing projectile prefab or spawn point reference!");
                return;
            }

            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            transform.forward = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
            
            Vector3 spreadDirection = Quaternion.Euler(0, Random.Range(-15f, 15f), 0) * directionToPlayer;

            GameObject projectile = Instantiate(slowProjectilePrefab, 
                projectileSpawnPoint.position, 
                Quaternion.LookRotation(spreadDirection));

            SlowProjectile slowProjectile = projectile.GetComponent<SlowProjectile>();
            if (slowProjectile != null)
            {
                slowProjectile.Initialize(projectileSpeed, projectileLifetime);
            }
            else
            {
                Debug.LogWarning("SlowProjectile component not found on instantiated prefab!");
            }
        }

        protected override void UpdateAnimator()
        {
            if (animator != null)
            {
                animator.SetBool("isDeactivated", isDeactivated);
            }
        }
    }
}