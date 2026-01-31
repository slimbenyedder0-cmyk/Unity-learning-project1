using UnityEngine;

public class CatapultState
{
    public CatapultController catapult;
    public CatapultStateMachine stateMachine;
    public Animator animationController;
    public string animationName;

    public CatapultState(CatapultController _catapult, CatapultStateMachine _stateMachine, Animator _animationController, string _animationName)
    {
        catapult = _catapult;
        stateMachine = _stateMachine;
        animationController = _animationController;
        animationName = _animationName;
    }
}
