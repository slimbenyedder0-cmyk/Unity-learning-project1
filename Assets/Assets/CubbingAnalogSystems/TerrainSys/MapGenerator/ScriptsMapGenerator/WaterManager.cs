using UnityEngine;
using System.Collections;

/// <summary>
/// Gère le plan d'eau pour le terrain procédural.
/// L'eau suit la position du viewer (caméra/joueur) et reste au niveau défini par MapGenerator.
/// </summary>
public class WaterManager : MonoBehaviour
{
    [Header("Configuration de l'eau")]
    [Tooltip("Matériau appliqué au plan d'eau")]
    [SerializeField] private Material waterMaterial;

    [Tooltip("Vitesse de suivi du viewer (plus élevé = plus rapide)")]
    [SerializeField, Range(1f, 20f)] private float followSpeed = 5f;

    [Tooltip("Si coché, désactive le collider du plan d'eau")]
    [SerializeField] private bool disableCollider = true;

    [Header("Références (auto-assignées)")]
    [Tooltip("Preview visible dans l'éditeur uniquement")]
    [SerializeField] private GameObject waterPlanePreview;

    // Références internes
    private GameObject waterPlaneObject;        // Le plan d'eau généré en jeu
    private Coroutine waterFollowCoroutine;     // Coroutine de suivi du viewer
    private MapGenerator mapGenerator;          // Référence au MapGenerator
    private Transform viewerPosition;           // Position du viewer (depuis EndlessTerrain)

    // État
    private bool isWaterActive = false;

    #region Initialisation et Nettoyage

    void Awake()
    {
        // Désactive la preview en mode jeu (elle sert uniquement dans l'éditeur)
        if (waterPlanePreview != null)
        {
            waterPlanePreview.SetActive(false);
        }

        // Récupère la référence au MapGenerator
        mapGenerator = GetComponent<MapGenerator>();
        if (mapGenerator == null)
        {
            Debug.LogError("[WaterManager] MapGenerator introuvable sur ce GameObject!");
        }
    }

    void OnDisable()
    {
        StopWaterFollowing();
    }

    void OnDestroy()
    {
        DestroyWaterPlane();
    }

    #endregion

    #region Méthodes Publiques

    /// <summary>
    /// Initialise et active le plan d'eau.
    /// Appelé par MapGenerator ou EndlessTerrain après l'initialisation.
    /// </summary>
    /// <param name="viewer">Transform du viewer à suivre (depuis EndlessTerrain)</param>
    /// <param name="terrainSize">Taille du terrain pour dimensionner le plan d'eau</param>
    public void InitializeWater(Transform viewer, int terrainSize)
    {
        if (mapGenerator == null)
        {
            Debug.LogError("[WaterManager] MapGenerator non assigné, impossible d'initialiser l'eau!");
            return;
        }

        // Assigne le viewer
        viewerPosition = viewer;

        // Calcule la position initiale de l'eau
        float waterHeight = mapGenerator.actualWaterHeight;
        Vector3 waterPosition = new Vector3(
            viewerPosition.position.x,
            waterHeight,
            viewerPosition.position.z
        );

        // Calcule l'échelle (un Plane Unity fait 10x10 unités par défaut)
        Vector3 waterScale = new Vector3(
            terrainSize / 10f,
            1f,
            terrainSize / 10f
        );

        // Génère ou met à jour le plan d'eau
        if (waterPlaneObject == null)
        {
            GenerateWaterPlane(waterPosition, waterScale);
        }
        else
        {
            UpdateWaterTransform(waterPosition, waterScale);
            waterPlaneObject.SetActive(true);
        }

        // Lance le suivi du viewer
        StartWaterFollowing();
        isWaterActive = true;

        Debug.Log($"[WaterManager] Eau initialisée à la hauteur {waterHeight} et suit le viewer");
    }

    /// <summary>
    /// Désactive complètement le plan d'eau.
    /// </summary>
    public void DisableWater()
    {
        StopWaterFollowing();

        if (waterPlaneObject != null)
        {
            waterPlaneObject.SetActive(false);
        }

        isWaterActive = false;
    }

    /// <summary>
    /// Change le matériau de l'eau à la volée.
    /// </summary>
    public void SetWaterMaterial(Material newMaterial)
    {
        waterMaterial = newMaterial;

        if (waterPlaneObject != null)
        {
            Renderer renderer = waterPlaneObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = waterMaterial;
            }
        }
    }

    #endregion

    #region Génération et Gestion du Plan d'Eau

    /// <summary>
    /// Crée le GameObject du plan d'eau avec tous ses composants.
    /// </summary>
    private void GenerateWaterPlane(Vector3 position, Vector3 scale)
    {
        // Nettoie l'ancien plan s'il existe
        if (waterPlaneObject != null)
        {
            Destroy(waterPlaneObject);
        }

        // Crée un plan primitif
        waterPlaneObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        waterPlaneObject.name = "WaterPlane_Dynamic";
        waterPlaneObject.transform.SetParent(transform);
        waterPlaneObject.transform.position = position;
        waterPlaneObject.transform.localScale = scale;

        // Configure le rendu
        ConfigureWaterRenderer();

        // Configure le collider
        ConfigureWaterCollider();

        Debug.Log("[WaterManager] Plan d'eau généré");
    }

    /// <summary>
    /// Configure le Renderer du plan d'eau (matériau et ombres).
    /// </summary>
    private void ConfigureWaterRenderer()
    {
        Renderer renderer = waterPlaneObject.GetComponent<Renderer>();
        if (renderer == null) return;

        // Applique le matériau
        if (waterMaterial != null)
        {
            renderer.material = waterMaterial;
        }
        else
        {
            Debug.LogWarning("[WaterManager] Aucun matériau d'eau assigné!");
        }

        // Configure les ombres
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = true;
    }

    /// <summary>
    /// Configure le Collider du plan d'eau.
    /// </summary>
    private void ConfigureWaterCollider()
    {
        Collider collider = waterPlaneObject.GetComponent<Collider>();
        if (collider != null && disableCollider)
        {
            collider.enabled = false;
        }
    }

    /// <summary>
    /// Met à jour la position et l'échelle du plan d'eau existant.
    /// </summary>
    private void UpdateWaterTransform(Vector3 position, Vector3 scale)
    {
        if (waterPlaneObject == null) return;

        waterPlaneObject.transform.position = position;
        waterPlaneObject.transform.localScale = scale;
    }

    /// <summary>
    /// Détruit complètement le plan d'eau et arrête toutes les coroutines.
    /// </summary>
    private void DestroyWaterPlane()
    {
        StopWaterFollowing();

        if (waterPlaneObject != null)
        {
            Destroy(waterPlaneObject);
            waterPlaneObject = null;
        }

        isWaterActive = false;
    }

    #endregion

    #region Suivi du Viewer

    /// <summary>
    /// Démarre la coroutine qui fait suivre l'eau au viewer.
    /// </summary>
    private void StartWaterFollowing()
    {
        // Arrête la coroutine précédente si elle existe
        StopWaterFollowing();

        if (waterPlaneObject != null && viewerPosition != null)
        {
            waterFollowCoroutine = StartCoroutine(FollowViewerCoroutine());
        }
    }

    /// <summary>
    /// Arrête la coroutine de suivi.
    /// </summary>
    private void StopWaterFollowing()
    {
        if (waterFollowCoroutine != null)
        {
            StopCoroutine(waterFollowCoroutine);
            waterFollowCoroutine = null;
        }
    }

    /// <summary>
    /// Coroutine : fait suivre le plan d'eau au viewer (position X/Z uniquement).
    /// La hauteur Y reste fixe selon MapGenerator.actualWaterHeight.
    /// </summary>
    private IEnumerator FollowViewerCoroutine()
    {
        while (waterPlaneObject != null && viewerPosition != null && mapGenerator != null)
        {
            // Position cible : suit le viewer en X/Z, garde la hauteur d'eau
            Vector3 targetPosition = new Vector3(
                viewerPosition.position.x,
                mapGenerator.actualWaterHeight,  // Hauteur fixe depuis MapGenerator
                viewerPosition.position.z
            );

            // Interpolation smooth vers la position cible
            waterPlaneObject.transform.position = Vector3.Lerp(
                waterPlaneObject.transform.position,
                targetPosition,
                followSpeed * Time.deltaTime
            );

            yield return null;
        }
    }

    #endregion

    #region Gizmos

    /// <summary>
    /// Affiche un aperçu visuel du plan d'eau dans l'éditeur Scene.
    /// </summary>
    void OnDrawGizmos()
    {
        if (waterPlaneObject != null && isWaterActive)
        {
            Gizmos.color = new Color(0.2f, 0.5f, 0.8f, 0.3f);

            // Taille du gizmo (Plane = 10x10 unités)
            Vector3 size = waterPlaneObject.transform.localScale * 10f;
            size.y = 0.1f;

            Gizmos.DrawCube(waterPlaneObject.transform.position, size);
        }
    }

    #endregion
}