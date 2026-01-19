using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Gère la génération procédurale de terrain infini autour du joueur.
/// Charge et décharge dynamiquement les chunks de terrain en fonction de la position du viewer.
/// </summary>
public class EndlessTerrain : MonoBehaviour
{
    /// <summary>
    /// Distance maximale de vue en unités Unity - détermine jusqu'où les chunks sont visibles
    /// </summary>
    public const float maxViewDst = 300;

    /// <summary>
    /// Référence au Transform du joueur/caméra pour suivre sa position
    /// </summary>
    public Transform viewer;

    /// <summary>
    /// Position actuelle du viewer en 2D (x, z) mise à jour chaque frame
    /// </summary>
    public static Vector2 viewerPosition;

    /// <summary>
    /// Référence au MapGenerator (sur le même GameObject)
    /// </summary>
    private MapGenerator mapGenerator;

    /// <summary>
    /// Taille d'un chunk de terrain individuel (en unités)
    /// </summary>
    int chunkSize;

    /// <summary>
    /// Nombre de chunks visibles dans chaque direction depuis le viewer
    /// </summary>
    int chunksVisibleInViewDst;

    /// <summary>
    /// Dictionnaire stockant tous les chunks créés avec leurs coordonnées comme clé.
    /// Permet de retrouver rapidement un chunk existant sans le recréer.
    /// </summary>
    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

    /// <summary>
    /// Liste des chunks qui étaient visibles lors de la dernière frame.
    /// Utilisée pour désactiver efficacement les chunks qui sortent de la vue.
    /// </summary>
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    /// <summary>
    /// Référence au WaterManager une fois initialisé
    /// </summary>
    private WaterManager waterManager;

    /// <summary>
    /// Initialise la taille des chunks et le système d'eau
    /// </summary>
    void Start()
    {
        // Récupère la taille du chunk depuis MapGenerator (-1 car on compte les vertices, pas les quads)
        chunkSize = MapGenerator.mapChunkSize - 1;

        // Calcule combien de chunks rentrent dans la distance de vue
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);

        // Initialise le système d'eau
        InitializeWater();
    }

    /// <summary>
    /// Initialise le WaterManager avec le viewer et la taille du terrain.
    /// Tous les composants sont sur le même GameObject.
    /// </summary>
    private void InitializeWater()
    {
        // Récupère les composants sur le même GameObject
        mapGenerator = GetComponent<MapGenerator>();
        waterManager = GetComponent<WaterManager>();

        // Vérifie que tous les composants sont présents
        if (mapGenerator == null)
        {
            Debug.LogError("[EndlessTerrain] MapGenerator introuvable sur ce GameObject!");
            return;
        }

        if (waterManager == null)
        {
            Debug.LogWarning("[EndlessTerrain] WaterManager introuvable sur ce GameObject!");
            return;
        }

        if (viewer == null)
        {
            Debug.LogError("[EndlessTerrain] Viewer non assigné! L'eau ne pourra pas suivre le joueur.");
            return;
        }

        // Initialise l'eau avec le viewer et la taille du terrain
        waterManager.InitializeWater(viewer, chunkSize);

        Debug.Log("[EndlessTerrain] Système d'eau initialisé avec succès");
    }

    /// <summary>
    /// Met à jour la position du viewer et rafraîchit les chunks visibles chaque frame
    /// </summary>
    void Update()
    {
        // Convertit la position 3D du viewer en 2D (ignore Y car on travaille sur un plan horizontal)
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        // Met à jour quels chunks doivent être visibles
        UpdateVisibleChunks();
    }

    /// <summary>
    /// Détermine quels chunks doivent être actifs en fonction de la position du viewer.
    /// Crée de nouveaux chunks si nécessaire et met à jour leur visibilité.
    /// Optimise les performances en désactivant d'abord les chunks précédemment visibles.
    /// </summary>
    void UpdateVisibleChunks()
    {
        // Désactive tous les chunks qui étaient visibles la frame précédente
        // (certains resteront actifs s'ils sont encore dans la zone de vue)
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }

        // Vide la liste pour la reconstruire avec les nouveaux chunks visibles
        terrainChunksVisibleLastUpdate.Clear();

        // Calcule les coordonnées du chunk où se trouve actuellement le viewer
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        // Parcourt tous les chunks dans la zone visible autour du viewer
        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                // Calcule les coordonnées du chunk à vérifier
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                // Si le chunk existe déjà dans le dictionnaire
                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    // Met à jour sa visibilité en fonction de la distance
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();

                    // Si le chunk est maintenant visible, l'ajoute à la liste pour le tracking
                    if (terrainChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                // Sinon, crée un nouveau chunk à cette position et l'ajoute au dictionnaire
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
                }
            }
        }
    }

    /// <summary>
    /// Représente un chunk individuel de terrain avec sa géométrie et sa logique de visibilité
    /// </summary>
    public class TerrainChunk
    {
        /// <summary>
        /// GameObject Unity contenant le mesh du terrain
        /// </summary>
        GameObject meshObject;

        /// <summary>
        /// Position mondiale du chunk en 2D
        /// </summary>
        Vector2 position;

        /// <summary>
        /// Limites du chunk utilisées pour calculer la distance avec le viewer
        /// </summary>
        Bounds bounds;

        /// <summary>
        /// Constructeur - crée un nouveau chunk de terrain à la position spécifiée
        /// </summary>
        /// <param name="coord">Coordonnées du chunk dans la grille</param>
        /// <param name="size">Taille du chunk en unités</param>
        /// <param name="parent">Transform parent pour organiser la hiérarchie</param>
        public TerrainChunk(Vector2 coord, int size, Transform parent)
        {
            // Calcule la position mondiale en multipliant les coordonnées par la taille
            position = coord * size;
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            // Crée les bounds pour la détection de distance (centré sur la position, avec la taille du chunk)
            bounds = new Bounds(position, Vector2.one * size);

            // Crée un plan primitif comme placeholder (sera remplacé par le vrai terrain généré)
            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;

            // Scale le plan (divisé par 10 car un plane Unity fait 10x10 par défaut)
            meshObject.transform.localScale = Vector3.one * size / 10f;

            // Désactive le chunk par défaut (sera activé si dans la distance de vue)
            SetVisible(false);
        }

        /// <summary>
        /// Met à jour la visibilité du chunk en fonction de sa distance avec le viewer
        /// </summary>
        public void UpdateTerrainChunk()
        {
            // Calcule la distance depuis le bord le plus proche du chunk jusqu'au viewer
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            // Le chunk est visible si sa distance est inférieure à la distance de vue max
            bool visible = viewerDstFromNearestEdge <= maxViewDst;
            SetVisible(visible);
        }

        /// <summary>
        /// Active ou désactive le GameObject du chunk
        /// </summary>
        /// <param name="visible">True pour activer, false pour désactiver</param>
        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        /// <summary>
        /// Vérifie si le chunk est actuellement visible (actif dans la scène)
        /// </summary>
        /// <returns>True si le GameObject du chunk est actif, false sinon</returns>
        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }
}