using UnityEngine;

public class EnemyStateMachine
{
    public EnemyStates CurrentEnemyState { get; set; }

    public void Initialize(EnemyStates startingState)
    {
        CurrentEnemyState = startingState;
        CurrentEnemyState.EnterState();
    }

    public void ChangeState(EnemyStates newState)
    {
        CurrentEnemyState.ExitState();
        CurrentEnemyState = newState;
        CurrentEnemyState.EnterState();
    }
}
