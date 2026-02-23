using UnityEngine;

public abstract class State
{
    // référence à la machine à états pour permettre la transition entre les états
    protected StateMachine stateMachine;
    // constructeur pour initialiser la référence à la machine à états
    public State(StateMachine sm)
    {
        stateMachine = sm;
    }
    // méthodes virtuelles pour les actions à effectuer lors de l'entrée, de la mise à jour et de la sortie de l'état
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
