using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class CatapultState
{
    protected StateMachine catapultStateMachine;
    public State(StateMachine csm) => catapultStateMachine = csm;

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
