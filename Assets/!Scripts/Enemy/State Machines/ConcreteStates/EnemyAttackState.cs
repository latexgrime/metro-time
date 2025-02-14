using _Scripts.Enemy.Base;
using UnityEngine;

public class EnemyAttackState : EnemyState
{

    private Transform playerTransform;
    private float timer;
    private float timeBetweenShots = 2f;

    private float exitTimer;
    private float timeTillExit = 3f;
    private float distanceToCountExit = 3f;
    private float bulletSpeed = 10f;
    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        playerTransform = GameObject.FindGameObjectWithTag("PlayerTarget").transform;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        
        enemy.MoveEnemy(Vector3.zero);

        if (timer > timeBetweenShots)
        {
            timer = 0f;
                
            Vector3 dir = (playerTransform.position - enemy.transform.position).normalized;

            Rigidbody bullet = GameObject.Instantiate(enemy.enemyBullet, enemy.transform.position, Quaternion.identity);
            bullet.linearVelocity = dir * bulletSpeed;
        }

        if (Vector3.Distance(playerTransform.position, enemy.transform.position) > distanceToCountExit)
        {
            exitTimer += Time.deltaTime;
            
            if (exitTimer > timeTillExit)
            {
                enemyStateMachine.ChangeState(enemy.ChaseState);
            }
        }
        else
        {
            exitTimer = 0f;
        }
        timer += Time.deltaTime;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }
}
