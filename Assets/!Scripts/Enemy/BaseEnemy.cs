using _Scripts.AmmoDrop;
using _Scripts.Enemy.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Enemy
{
    public abstract class BaseEnemy : MonoBehaviour, IShieldable
    {
        [Header("- Distance Settings")]
        [SerializeField] protected float minKeepDistance = 8f; // Minimum distance to maintain from player.
        [SerializeField] protected float maxChaseDistance = 100f; // Max distance.
        
        [Header("- Shield Settings")]
        [SerializeField] protected float maxShield = 100f;
        [SerializeField] protected float currentShield;
        [SerializeField] protected float shieldRegenRate = 5f;
        [SerializeField] protected float shieldRegenDelay = 3f;
        
        [Header("- Movement Settings")]
        [SerializeField] protected float detectionRange = 15f;
        [SerializeField] protected float attackRange = 10f;
        [SerializeField] protected float moveSpeed = 5f;
        [SerializeField] protected float hoverHeight = 2f; 
        [SerializeField] protected float bobAmount = 0.5f;
        [SerializeField] protected float bobSpeed = 2f;
        
        [Header("- Deactivation Physics")]
        public bool isDeactivated;
        [SerializeField] protected float deactivationForce = 5f;
        [SerializeField] protected float deactivationTorque = 2f;
        
        [Header("- Drops")]
        [SerializeField] protected AmmoDropper ammoDropper;
        
        protected bool _animationsEnabled = true;
        protected bool _movementEnabled = true;
        protected float _moveSpeedMultiplier = 1f;
        protected float _animationSpeedMultiplier = 1f;
        protected float lastDamageTime;
        protected bool hasDeactivationPhysicsApplied;
        protected bool isFriendly;
        
        protected Transform player;
        protected NavMeshAgent agent;
        protected Animator animator;
        protected Rigidbody rb;
        protected Collider mainCollider;
        protected EnemyState currentState = EnemyState.Patrol;
        protected Vector3 startPosition;
        
        public bool IsFriendly() => isFriendly;
        public float GetCurrentShield() => currentShield;
        public float GetMaxShield() => maxShield;
        public bool IsDeactivated() => isDeactivated;
        
        protected virtual void Start()
        {
            currentShield = maxShield;
            animator = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            rb = GetComponent<Rigidbody>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            startPosition = transform.position;

            // Configure initial physics.
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }
            
            if (agent != null)
            {
                agent.baseOffset = hoverHeight;
                agent.speed = moveSpeed;
            }
        }

        public void SetAnimationEnabled(bool enabled)
        {
            _animationsEnabled = enabled;
            if (!enabled && animator != null)
            {
                animator.SetFloat("MovementSpeed", 0f);
                animator.SetBool("isAttacking", false);
            }
        }

        public void SetMovementEnabled(bool enabled)
        {
            _movementEnabled = enabled;
            if (!enabled && agent != null)
            {
                agent.isStopped = true;
            }
        }

        public void SetMovementSpeedMultiplier(float multiplier)
        {
            _moveSpeedMultiplier = Mathf.Clamp(multiplier, 0f, 1f);
            if (agent != null)
            {
                agent.speed = moveSpeed * _moveSpeedMultiplier;
            }
        }

        public void SetAnimationSpeed(float multiplier)
        {
            _animationSpeedMultiplier = multiplier;
            if (animator != null)
            {
                animator.speed = multiplier;
            }
        }
        
        protected virtual void Update()
        {
            if (isDeactivated) return;

            if (!isFriendly)
            {
                RegenerateShield();
                UpdateState();
                UpdateBehavior();
            }
        
            // This keeps hovering when friendly.
            UpdateHoverMotion();
        }

        protected virtual void UpdateState()
        {
            if (isDeactivated)
            {
                currentState = EnemyState.Deactivated;
                return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
                currentState = EnemyState.Attack;
            else if (distanceToPlayer <= detectionRange)
                currentState = EnemyState.Chase;
            else
                currentState = EnemyState.Patrol;
        }

        protected virtual void UpdateHoverMotion()
        {
            // Add a bobbing motion to make it look more natural.
            if (agent != null)
            {
                float newHeight = hoverHeight + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
                agent.baseOffset = newHeight;
            }
        }
        
        protected virtual void UpdateAnimator()
        {
            if (animator != null && _animationsEnabled)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
                // Only set animation parameters if we're not disabled.
                if (!isDeactivated)
                {
                    animator.SetBool("isAttacking", distanceToPlayer <= attackRange);
            
                    // Handle movement animations.
                    if (_movementEnabled)
                    {
                        Vector3 velocity = agent != null ? agent.velocity : Vector3.zero;
                        float speed = velocity.magnitude;
                        animator.SetFloat("MovementSpeed", speed * _moveSpeedMultiplier * _animationSpeedMultiplier);
                    }
                    else
                    {
                        animator.SetFloat("MovementSpeed", 0f);
                    }
                }
        
                animator.SetBool("isDeactivated", isDeactivated);
            }
        }
        
        protected virtual void UpdateBehavior()
        {
            switch (currentState)
            {
                case EnemyState.Patrol:
                    Patrol();
                    break;
                case EnemyState.Chase:
                    ChasePlayer();
                    break;
                case EnemyState.Attack:
                    Attack();
                    break;
                case EnemyState.Deactivated:
                    HandleDeactivated();
                    break;
            }
        }

        protected virtual void Patrol()
        {
            // Override in child classes.
        }

        protected virtual void ChasePlayer()
        {
            if (agent == null || !agent.isActiveAndEnabled) return;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // If too close, back away.
            if (distanceToPlayer < minKeepDistance)
            {
                // Calculate a point behind the enemy.
                Vector3 backwardDirection = transform.position - player.position;
                backwardDirection.y = 0;
                backwardDirection = backwardDirection.normalized;

                Vector3 backupDestination = transform.position + backwardDirection * (minKeepDistance - distanceToPlayer + 1f);
                agent.SetDestination(backupDestination);
            }
            // If within chase range, move towards player.
            else if (distanceToPlayer <= maxChaseDistance)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                // Is too far so stop moving.
                agent.ResetPath();
            }
        }

        protected abstract void Attack();

        protected virtual void HandleDeactivated()
        {
            if (agent != null)
                agent.isStopped = true;
        }

        private void RegenerateShield()
        {
            if (Time.time - lastDamageTime >= shieldRegenDelay && currentShield < maxShield)
            {
                currentShield += shieldRegenRate * Time.deltaTime;
                currentShield = Mathf.Min(currentShield, maxShield);
            }
        }

        public virtual void TakeShieldDamage(float damage)
        {
            if (isDeactivated) return;

            lastDamageTime = Time.time;
            currentShield -= damage;

            if (currentShield <= 0)
            {
                currentShield = 0;
                Deactivate();
            }
        }

        public virtual void Reactivate()
        {
            isDeactivated = false;
            hasDeactivationPhysicsApplied = false;
            currentShield = maxShield;
            
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                transform.rotation = Quaternion.identity;
            }
            
            if (agent != null)
            {
                agent.enabled = true;
                agent.Warp(transform.position);
            }
            
            if (animator != null)
            {
                animator.SetTrigger("reactivate");
            }
        }

        protected virtual void Deactivate()
        {
            if (isDeactivated) return;

            isDeactivated = true;
            // Set to friendly when deactivated.
            isFriendly = true;

            // Drop ammo when deactivated.
            if (ammoDropper != null)
            {
                ammoDropper.DropAmmo();
            }

            // Stop the agent but keep it enabled for hover effect.
            if (agent != null)
            {
                agent.isStopped = true;
            }

            // Update animation state.
            if (animator != null)
            {
                animator.SetBool("isFriendly", true);
                animator.SetTrigger("deactivate");
            }

            SendMessage("OnBecameFriendly", SendMessageOptions.DontRequireReceiver);
        }
    }
    
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Deactivated
    }
}