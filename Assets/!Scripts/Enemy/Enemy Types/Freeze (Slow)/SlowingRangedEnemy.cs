using _Scripts.AmmoDrop;
using _Scripts.Status_System;
using UnityEngine;

namespace _Scripts.Enemy.Enemy_Types.Freeze__Slow_
{
    public class SlowingRangedEnemy : BaseEnemy
    {
        [Header("- Attack Settings")]
        [SerializeField] private GameObject slowProjectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float projectileSpeed = 20f;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private float projectileLifetime = 5f;

        [Header("- Wobble Settings")]
        [SerializeField] private float wobbleAmount = 5f;
        [SerializeField] private float wobbleSpeed = 3f;
        
        private float _nextAttackTime;
        private Vector3 _startRotation;
        
        protected override void Start()
        {
            base.Start();
        
            // This is setting some default distances for dummy proofing.
            if (minKeepDistance <= 0) minKeepDistance = 8f;
            if (maxChaseDistance <= 0) maxChaseDistance = 15f;
            
            // Ignore collisions between enemy projectiles and enemies.
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyProjectile"), LayerMask.NameToLayer("Enemy"), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyProjectile"), LayerMask.NameToLayer("EnemyProjectile"), true);
            
            animator = GetComponentInChildren<Animator>();
            _startRotation = transform.eulerAngles;
            
            if (ammoDropper == null)
            {
                ammoDropper = GetComponent<AmmoDropper>();
            }
        }

        protected override void Update()
        {
            base.Update();
            UpdateAnimator();
            UpdateHoverMotion();
            UpdateWobble();
        }

        private void UpdateWobble()
        {
            if (isDeactivated) return;

            float wobbleX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
            float wobbleZ = Mathf.Cos(Time.time * wobbleSpeed * 0.7f) * wobbleAmount;

            transform.eulerAngles = new Vector3(
                _startRotation.x + wobbleX,
                transform.eulerAngles.y, 
                _startRotation.z + wobbleZ
            );
        }

        protected override void Attack()
        {
            if (Time.time >= _nextAttackTime)
            {
                var directionToPlayer = (player.position - transform.position).normalized;
                transform.forward = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);

                if (animator != null) animator.SetTrigger("shoot");

                var projectile = Instantiate(slowProjectilePrefab,
                    projectileSpawnPoint.position,
                    Quaternion.LookRotation(directionToPlayer));

                var slowProjectile = projectile.GetComponent<SlowProjectile>();
                if (slowProjectile != null) 
                {
                    slowProjectile.Initialize(projectileSpeed, projectileLifetime);
                    slowProjectile.PlayShotSound();
                }
                
                _nextAttackTime = Time.time + attackCooldown;
            }
        }

        protected override void UpdateAnimator()
        {
            if (animator == null) return;

            // Check if the enemy is moving forward.
            if (agent != null)
            {
                Vector3 velocity = agent.velocity;
                // Project velocity onto the enemy's forward direction.
                float forwardMovement = Vector3.Dot(velocity.normalized, transform.forward);

                // Set forward movement animation parameter.
                animator.SetFloat("ForwardMovement", forwardMovement);
            }

            animator.SetBool("isDeactivated", isDeactivated);
        }
    }
}