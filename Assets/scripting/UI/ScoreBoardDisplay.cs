using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ScoreBoardDisplay : MonoBehaviour
{
    #region Variables
    [Header("Configuration")]
    [SerializeField] private string prefix = "Score: ";
    
    // Indispensable pour ton script 'spiraleapoints.cs'
    [HideInInspector] public int valeurtotale; 
    
    private TMP_Text textComponent;
    private int lastValeurVisualisee = -1; 
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            // Correction du nom de l'event : OnScoreChanged
            ScoreManager.Instance.OnScoreChanged.AddListener(RefreshDisplay);
            
            // Initialisation avec la valeur actuelle
            RefreshDisplay(ScoreManager.Instance.GetCurrentScore());
        }
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged.RemoveListener(RefreshDisplay);
        }
    }
    #endregion

    #region Méthode de Mise à jour
    /// <summary>
    /// Reçoit le score depuis le ScoreManager et met à jour l'affichage.
    /// </summary>
    public void RefreshDisplay(int nouvelleValeur)
    {
        valeurtotale = nouvelleValeur; // On met à jour la variable que 'spiraleapoints' surveille

        if (valeurtotale != lastValeurVisualisee)
        {
            if (textComponent != null)
            {
                textComponent.text = $"{prefix}{valeurtotale}";
            }
            lastValeurVisualisee = valeurtotale;
        }
    }
    #endregion
}