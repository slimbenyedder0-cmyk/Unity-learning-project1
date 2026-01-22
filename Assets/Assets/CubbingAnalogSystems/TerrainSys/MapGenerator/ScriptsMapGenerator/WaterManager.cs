using UnityEngine;
using System.Collections;

/// <summary>
/// Gère le plan d'eau pour le terrain procédural.
/// L'eau suit la position du viewer (caméra/joueur) et reste au niveau défini par MapGenerator.
/// </summary>
public class WaterManager : MonoBehaviour
{
    [Header("Activation globale")]
    [Tooltip("DÉSACTIVE COMPLÈTEMENT le système d'eau (pour tests sans eau)")]
    [SerializeField] private bool enableWaterSystem = true;

    [Header("Configuration de l'eau")]
    [Tooltip("Matériau appliqué au plan d'eau")]
    [SerializeField] private Material waterMaterial;

    [Tooltip("Vitesse de suivi du viewer (plus élevé = plus rapide)")]
    [SerializeField, Range(1f, 20f)] private float followSpeed = 10f;

    [Tooltip("Distance minimale avant de mettre à jour la position (optimisation)")]
    [SerializeField, Range(0.1f, 50f)] private float updateThreshold = 15f;

    [Tooltip("Multiplicateur de taille du plan d'eau (>1 pour éviter de voir les bords)")]
    [SerializeField, Range(1f, 3f)] private float waterScaleMultiplier = 1.5f;

    [Tooltip("Si coché, désactive le collider du plan d'eau")]
    [SerializeField] private bool disableCollider = true;

    [Header("Gizmo")]
    [SerializeField] private bool displayGizmo = true;

    private GameObject waterPlaneObject;
    private Coroutine waterFollowCoroutine;
    private MapGenerator mapGenerator;
    private Transform viewerPosition;
    private bool isWaterActive = false;

    #region Initialisation et Nettoyage

    void Awake()
    {
        // HARD DISABLE : si désactivé, ne rien initialiser du tout
        if (!enableWaterSystem)
        {
            Debug.Log("[WaterManager] Système d'eau DÉSACTIVÉ - Aucune initialisation");
            enabled = false;
            return;
        }

        mapGenerator = GetComponent<MapGenerator>();

        if (mapGenerator == null)
        {
            Debug.LogError("[WaterManager] MapGenerator introuvable sur ce GameObject!");
            enabled = false;
            return;
        }

        // Calcule les données d'eau mais N'INITIALISE PAS encore le plan
        // L'initialisation sera faite par EndlessTerrain quand le viewer sera prêt
        mapGenerator.CalculateActualWaterData();
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

    #region API Publique

    public void InitializeWater(Transform viewer, int waterScale)
    {
        // HARD DISABLE : si désactivé, ignore complètement l'appel
        if (!enableWaterSystem)
        {
            return;
        }

        if (mapGenerator == null)
        {
            Debug.LogError("[WaterManager] MapGenerator non initialisé!");
            return;
        }

        viewerPosition = viewer;

        if (viewerPosition == null)
        {
            Debug.LogWarning("[WaterManager] Viewer null - impossible d'initialiser l'eau");
            return;
        }

        // Calcul de la position et échelle avec multiplicateur pour éviter les bords
        Vector3 waterPosition = new Vector3(
            viewerPosition.position.x,
            mapGenerator.actualWaterHeight,
            viewerPosition.position.z
        );

        // Application du multiplicateur pour agrandir le plan d'eau
        float scaledSize = waterScale * 5f * waterScaleMultiplier;
        Vector3 scale = new Vector3(scaledSize, 1f, scaledSize);

        // Génération ou mise à jour du plan
        if (waterPlaneObject == null)
        {
            GenerateWaterPlane(waterPosition, scale);
        }
        else
        {
            UpdateWaterTransform(waterPosition, scale);
            waterPlaneObject.SetActive(true);
        }

        StartWaterFollowing();
        isWaterActive = true;

        Debug.Log($"[WaterManager] Eau initialisée à hauteur {mapGenerator.actualWaterHeight} (taille: {scaledSize})");
    }

    public void DisableWater()
    {
        // HARD DISABLE : nettoie et désactive le component entier
        if (!enableWaterSystem)
        {
            enabled = false;
            return;
        }

        StopWaterFollowing();

        if (waterPlaneObject != null)
        {
            waterPlaneObject.SetActive(false);
        }

        isWaterActive = false;
    }

    public void SetWaterMaterial(Material newMaterial)
    {
        // HARD DISABLE : ignore les changements de matériau
        if (!enableWaterSystem)
        {
            return;
        }

        if (newMaterial == null)
        {
            Debug.LogWarning("[WaterManager] Tentative d'assigner un matériau null");
            return;
        }

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

    #region Génération du Plan d'Eau

    private void GenerateWaterPlane(Vector3 position, Vector3 scale)
    {
        if (waterPlaneObject != null)
        {
            Destroy(waterPlaneObject);
        }

        // Création du plan primitif
        waterPlaneObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        waterPlaneObject.name = "WaterPlane_Dynamic";
        waterPlaneObject.transform.SetParent(transform);
        waterPlaneObject.transform.position = position;
        waterPlaneObject.transform.localScale = scale;

        ConfigureWaterRenderer();
        ConfigureWaterCollider();

        Debug.Log("[WaterManager] Plan d'eau généré");
    }

    private void ConfigureWaterRenderer()
    {
        Renderer renderer = waterPlaneObject.GetComponent<Renderer>();
        if (renderer == null) return;

        if (waterMaterial != null)
        {
            renderer.material = waterMaterial;
        }
        else
        {
            Debug.LogWarning("[WaterManager] Aucun matériau d'eau assigné!");
        }

        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = true;
    }

    private void ConfigureWaterCollider()
    {
        Collider collider = waterPlaneObject.GetComponent<Collider>();
        if (collider != null)
        {
            if (disableCollider)
            {
                // Désactive complètement pour éviter les collisions
                collider.enabled = false;
            }
            else
            {
                // Mode Trigger si activé pour éviter les blocages physiques
                collider.isTrigger = true;
                Debug.LogWarning("[WaterManager] Collider d'eau activé en mode Trigger.");
            }
        }
    }

    private void UpdateWaterTransform(Vector3 position, Vector3 scale)
    {
        if (waterPlaneObject == null) return;

        waterPlaneObject.transform.position = position;
        waterPlaneObject.transform.localScale = scale;
    }

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

    private void StartWaterFollowing()
    {
        StopWaterFollowing();

        if (waterPlaneObject != null && viewerPosition != null)
        {
            waterFollowCoroutine = StartCoroutine(FollowViewerCoroutine());
        }
    }

    private void StopWaterFollowing()
    {
        if (waterFollowCoroutine != null)
        {
            StopCoroutine(waterFollowCoroutine);
            waterFollowCoroutine = null;
        }
    }

    private IEnumerator FollowViewerCoroutine()
    {
        // Cache la dernière hauteur d'eau pour éviter les recalculs
        float cachedWaterHeight = mapGenerator.actualWaterHeight;
        Vector3 lastPosition = waterPlaneObject.transform.position;

        while (waterPlaneObject != null && viewerPosition != null && mapGenerator != null)
        {
            // Position cible : suit X/Z du viewer, garde Y fixe
            Vector3 targetPosition = new Vector3(
                viewerPosition.position.x,
                cachedWaterHeight,
                viewerPosition.position.z
            );

            // Calcul distance horizontale (ignore Y pour optimisation)
            float sqrDistance = (waterPlaneObject.transform.position.x - targetPosition.x) *
                                (waterPlaneObject.transform.position.x - targetPosition.x) +
                                (waterPlaneObject.transform.position.z - targetPosition.z) *
                                (waterPlaneObject.transform.position.z - targetPosition.z);

            // Mise à jour uniquement si déplacement significatif (évite calculs inutiles)
            if (sqrDistance > updateThreshold * updateThreshold)
            {
                // Interpolation smooth pour mouvement fluide
                waterPlaneObject.transform.position = Vector3.Lerp(
                    waterPlaneObject.transform.position,
                    targetPosition,
                    followSpeed * Time.deltaTime
                );
            }

            // Yield une frame sur deux pour réduire encore la charge CPU
            yield return null;
            yield return null;
        }
    }

    #endregion

    #region Gizmos

    void OnDrawGizmos()
    {
        // HARD DISABLE : pas de gizmo si système désactivé
        if (!enableWaterSystem || !displayGizmo || waterPlaneObject == null || !isWaterActive)
            return;

        Gizmos.color = new Color(0.2f, 0.5f, 0.8f, 0.3f);

        Vector3 size = waterPlaneObject.transform.localScale * 10f;
        size.y = 0.1f;

        Gizmos.DrawCube(waterPlaneObject.transform.position, size);
    }

    #endregion
}