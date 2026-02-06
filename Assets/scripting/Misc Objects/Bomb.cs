using UnityEngine;
/// <summary>
/// Ce script est un exemple de ce que le système de Timer peut faire.
/// </summary>
public class Bomb : MonoBehaviour
{
    int timerId;

    void Start() => timerId = TimerManager.Create(10f); // 10 secondes

    void Update()
    {
        float timeLeft = TimerManager.GetValue(timerId);

        if (timeLeft > 0) 
        {
            // On affiche uniquement si c'est encore en train de tourner
            Debug.Log("Explosion dans : " + TimeFormatter.Format(timeLeft));
        }
        else 
        {
            Debug.Log("BOOM !");
            this.enabled = false; // On arrête le script
        }
    }
}
