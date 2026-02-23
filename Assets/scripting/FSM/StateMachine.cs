using UnityEngine;

public class StateMachine : MonoBehaviour
{
    // On peut ajouter une référence à l'état actuel pour faciliter la gestion des transitions
    protected State currentState;

    /// <summary>
    /// Cette méthode permet de changer d'état. Elle s'assure que les méthodes Exit() et Enter() sont appelées correctement pour gérer les transitions entre les états.
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();
    }

    // Update est appelé une fois par frame. Il est protégé et virtuel pour permettre aux classes dérivées de l'implémenter selon leurs besoins spécifiques.
    protected virtual void Update()
    {
        currentState?.Update(); // Appelle la méthode Update() de l'état actuel (si elle existe), ce qui permet à chaque état de gérer son propre comportement de mise à jour.
    }
}
