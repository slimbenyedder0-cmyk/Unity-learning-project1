using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gère le radar du joueur : détecte les spirales et active leur aspiration.
/// </summary>
public class SearchProcess : MonoBehaviour
{
    #region Variables
    [Header("Radar")]
    [Tooltip("Liste de tous les objets collectables enregistrés au Start.")]
    public List<GameObject> Ofinterestlist = new List<GameObject>();
    #endregion

    /// <summary>
    /// Se déclenche quand un objet entre dans le cercle (Trigger) du radar.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 1. On vérifie si l'objet qui entre possède le script SpiralePoints
        if (other.TryGetComponent<SpiralePoints>(out SpiralePoints spirale))
        {
            // 2. On vérifie s'il fait partie de la liste des objets qui nous intéressent
            if (Ofinterestlist.Contains(other.gameObject))
            {
                // 3. Si la spirale ne bouge pas encore, on active son aspiration
                // Note : J'ai remplacé 'ismoving' par une propriété propre dans SpiralePoints
                spirale.ActiverAspiration(this.transform.parent.gameObject);
                
                Debug.Log($"<color=cyan>Radar :</color> {other.name} détecté et activé !");
            }
        }
        else
        {
            // 4. Si ce n'est pas une spirale, on demande à la physique d'ignorer la collision
            // pour ne pas que le radar "pousse" les objets ou les quilles.
            Physics.IgnoreCollision(other, GetComponent<Collider>());
        }
    }
}