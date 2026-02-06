using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Gère la logique du score de manière isolée.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Données")]
    [SerializeField] private int currentScore = 0;

    [Header("Événements")]
    [Tooltip("Envoie le nouveau score à chaque modification.")]
    public UnityEvent<int> OnScoreChanged;

    private void Awake()
    {
        // Setup du Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Ajoute des points et prévient les abonnés.
    /// </summary>
    public void AddScore(int points)
    {
        currentScore += points;
        
        // On lance l'événement avec la nouvelle valeur
        OnScoreChanged?.Invoke(currentScore);
        
        Debug.Log($"<color=green>SCORE :</color> {currentScore} (+{points})");
    }

    public int GetCurrentScore() => currentScore;
}