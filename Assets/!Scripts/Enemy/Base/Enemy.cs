using UnityEngine;

namespace _Scripts.Enemy.Base
{
    public class Enemy : MonoBehaviour, IDamageable, IEnemyMoveable, ITriggerCheckable
    {
        [field: SerializeField] public float MaxHealth { get; set; } = 100f;
        public float CurrentHealth { get; set; }
    
        public Rigidbody RB { get; set; }
        public bool isFacingRight { get; set; } = true;
    
        #region State Machine Variables
    
        public EnemyStateMachine StateMachine { get; set; }
        public EnemyIdleState IdleState { get; set; }
        public EnemyChaseState ChaseState { get; set; }
        public EnemyAttackState AttackState { get; set; }
    
        #endregion

        #region Idle Variables

        public float randomMovementRange = 5f;
        public float randomMovementSpeed = 1f;
    
        #endregion
    
        public bool IsAggroed { get; set; }
        public bool IsWithinStrikingDistance { get; set; }

        public Rigidbody enemyBullet;
    
        private void Awake()
        {
            RB = GetComponent<Rigidbody>();
            StateMachine = new EnemyStateMachine();

            IdleState = new EnemyIdleState(this, StateMachine);
            ChaseState = new EnemyChaseState(this, StateMachine);
            AttackState = new EnemyAttackState(this, StateMachine);

        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            CurrentHealth = MaxHealth;
        
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            StateMachine.CurrentEnemyState.FrameUpdate();
        }

        private void FixedUpdate()
        {
            StateMachine.CurrentEnemyState.PhysicsUpdate();
        }

        #region Health / Die Functions
        public void Damage(float damageAmount)
        {
            CurrentHealth -= damageAmount;

            if (CurrentHealth <= 0f)
            {
                Die();
            }
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    
        #endregion
    
        #region Movement Functions
    
        public void MoveEnemy(Vector3 velocity)
        {
            RB.linearVelocity = velocity;
            CheckForLeftOrRightFacing(velocity);
        }

        public void CheckForLeftOrRightFacing(Vector3 velocity)
        {
            if (isFacingRight && velocity.x < 0f)
            {
                Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
                transform.rotation = Quaternion.Euler(rotator);
                isFacingRight = !isFacingRight;
            }
        
            else if (!isFacingRight && velocity.x > 0f)
            {
                Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
                transform.rotation = Quaternion.Euler(rotator);
                isFacingRight = !isFacingRight;
            }
            
        }
    
        #endregion
    
        #region Animation Triggers

        private void AnimationTriggerEvent(AnimationTriggerType triggerType)
        {
            StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
        }
        public enum AnimationTriggerType
        {
            EnemyDamaged,
            PlayFootstepSound
        }
    
        #endregion
    
        #region Distance Checks
    
        public void SetAggroStatus(bool isAggroed)
        {
            IsAggroed = isAggroed;
        }

        public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
        {
            IsWithinStrikingDistance = isWithinStrikingDistance;
        }
    
        #endregion
    }
}
