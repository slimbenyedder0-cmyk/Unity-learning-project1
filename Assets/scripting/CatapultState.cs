using UnityEngine;

public class CatapultState
{
    private CatapultController catapult;
    private CatapultStateMachine stateMachine;
    private Animator animationController;
    private string animationName;

    public CatapultState(CatapultController _catapult, CatapultStateMachine _stateMachine, Animator _animationController, string _animationName)
    {
        catapult = _catapult;
        stateMachine = _stateMachine;
        animationController = _animationController;
        animationName = _animationName;
    }
}
