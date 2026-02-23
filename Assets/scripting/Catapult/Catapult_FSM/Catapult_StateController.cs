using UnityEngine;

public class Catapult_StateController : StateMachine
{
    [Header("Paramètres de changement d'état/d'états")]
    // Ici, on mettera les références aux différents états que le catapult peut avoir, par exemple : saut, chute etc.


    [Header("Références aux différents animators et audioSources")]
    // Ici, on mettra les références aux différents animators et audioSources que le catapult peut utiliser pour ses différentes animations et sons.
    

    [Header("Les états utilisés par la Catapulte")]
    // Ici, on mettra les références aux différents états que le catapult peut avoir, par exemple : saut, chute etc.
    public Catapult_IdleState idleState;

    //Dans Awake, on initialise les différents états et on définit l'état de départ de la catapulte.
    private void Awake()
    {
        ChangeState(idleState);
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
}
