using UnityEngine;

/// <summary>
/// Gère le suivi fluide de la caméra autour du joueur (Le Cube).
/// </summary>
public class CameraFollow : MonoBehaviour
{
    #region Variables - Configuration
    [Header("Cibles")]
    [SerializeField] private Transform target; // Ton "Le Cube"
    
    [Header("Réglages de Suivi")]
    [SerializeField] private float smoothSpeed = 10f; // Vitesse de lissage
    [SerializeField] private Vector3 offset; // Décalage par rapport au cube
    
    [Header("Options")]
    [SerializeField] private bool lookAtTarget = true;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        // Si l'offset n'est pas réglé dans l'inspecteur, on le calcule automatiquement
        if (offset == Vector3.zero && target != null)
        {
            offset = transform.position - target.position;
        }
    }

    /// <summary>
    /// LateUpdate est appelé après toutes les mises à jour de mouvement.
    /// Indispensable pour éviter les tremblements de caméra.
    /// </summary>
    private void LateUpdate()
    {
        if (target == null) return;

        HandleMovement();
        
        if (lookAtTarget)
        {
            transform.LookAt(target);
        }
    }
    #endregion

    #region Logique de Caméra
    private void HandleMovement()
    {
        // Position désirée (où la caméra devrait être)
        Vector3 desiredPosition = target.position + offset;
        
        // On glisse doucement vers cette position (Lerp)
        // Note : On utilise SmoothDamp ou Lerp pour un rendu pro
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
    #endregion

    #region Utilitaires
    /// <summary>
    /// Permet de changer manuellement la cible si besoin.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    #endregion
}