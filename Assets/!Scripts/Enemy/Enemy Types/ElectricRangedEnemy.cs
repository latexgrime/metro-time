using UnityEngine;

namespace _Scripts.Enemy.Enemy_Types
{
    public class ElectricRangedEnemy : BaseEnemy
    {
        [Header("- Attack Settings")] [SerializeField]
        private GameObject electricProjectilePrefab;

        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float projectileSpeed = 20f;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private float projectileLifetime = 5f;

        private float _nextAttackTime;

        protected override void Start()
        {
            base.Start();
            animator = GetComponentInChildren<Animator>();
        }

        protected override void Update()
        {
            base.Update();
            UpdateAnimator();
        }

        protected override void Attack()
        {
            if (Time.time >= _nextAttackTime)
            {
                var directionToPlayer = (player.position - transform.position).normalized;
                transform.forward = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);

                // Trigger attack animation.
                if (animator != null) animator.SetTrigger("shoot");

                // Shoot projectile.
                var projectile = Instantiate(electricProjectilePrefab,
                    projectileSpawnPoint.position,
                    Quaternion.LookRotation(directionToPlayer));

                var electricProjectile = projectile.GetComponent<ElectricProjectile>();
                if (electricProjectile != null) electricProjectile.Initialize(projectileSpeed, projectileLifetime);

                _nextAttackTime = Time.time + attackCooldown;
            }
        }

        protected override void UpdateAnimator()
        {
            if (animator != null) animator.SetBool("isDeactivated", isDeactivated);
        }
    }
}