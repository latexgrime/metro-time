using _Scripts.Enemy.Base;
using UnityEngine;

public class EnemyStates
{
    protected Enemy enemy;
    protected EnemyStateMachine enemyStateMachine;

    public EnemyStates(Enemy enemy, EnemyStateMachine enemyStateMachine)
    {
        this.enemy = enemy;
        this.enemyStateMachine = enemyStateMachine;
    }
    
    public virtual void EnterState() {}
    
    public virtual void ExitState() {}
    
    public virtual void FrameUpdate() {}
    
    public virtual void PhysicsUpdate() {}
    
    public virtual void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType) {}
    
}
