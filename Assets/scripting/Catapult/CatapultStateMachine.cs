using UnityEngine;

public class CatapultStateMachine
{
    public CatapultState _CurrentState;

    public void ChangeState(CatapultState newState)
    {
        _CurrentState = newState;
    }

}
