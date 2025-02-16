using _Scripts.Enemy.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Enemy
{
    public abstract class BaseEnemy : MonoBehaviour, IShieldable
    {
        [Header("Shield Settings")]
        [SerializeField] protected float maxShield = 100f;
        [SerializeField] protected float currentShield;
        [SerializeField] protected float shieldRegenRate = 5f;
        [SerializeField] protected float shieldRegenDelay = 3f;
        
        [Header("Movement Settings")]
        [SerializeField] protected float detectionRange = 15f;
        [SerializeField] protected float attackRange = 10f;
        [SerializeField] protected float moveSpeed = 5f;
        [SerializeField] protected float hoverHeight = 2f; 
        [SerializeField] protected float bobAmount = 0.5f;
        [SerializeField] protected float bobSpeed = 2f;
        
        
        protected Transform player;
        protected NavMeshAgent agent;
        protected Animator  animator;
        protected bool isDeactivated;
        protected float lastDamageTime;
        protected EnemyState currentState = EnemyState.Patrol;
        protected Vector3 startPosition;
        
        protected virtual void Start()
        {
            currentShield = maxShield;
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            startPosition = transform.position;
            
            if (agent != null)
            {
                agent.baseOffset = hoverHeight;
                agent.speed = moveSpeed;
            }
        }

        protected virtual void Update()
        {
            if (isDeactivated) return;

            RegenerateShield();
            UpdateState();
            UpdateBehavior();
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
            if (animator != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                animator.SetBool("isAttacking", distanceToPlayer <= attackRange);
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
            
        }

        protected virtual void ChasePlayer()
        {
            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.SetDestination(player.position);
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
        
        public float GetCurrentShield() => currentShield;
        public float GetMaxShield() => maxShield;
        public bool IsDeactivated() => isDeactivated;

        public virtual void TakeShieldDamage(float damage)
        {
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
            currentShield = maxShield;
            if (agent != null)
            {
                agent.isStopped = false;
            }
            if (animator != null)
            {
                animator.SetTrigger("reactivate");
            }
        }

        protected virtual void Deactivate()
        {
            isDeactivated = true;
            if (agent != null)
            {
                agent.isStopped = true;
            }
            if (animator != null)
            {
                animator.SetTrigger("deactivate");
            }
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