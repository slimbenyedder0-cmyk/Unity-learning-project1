using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Gère la génération procédurale de terrain infini autour du joueur.
/// Charge et décharge dynamiquement les chunks de terrain en fonction de la position du viewer.
/// </summary>
public class EndlessTerrain : MonoBehaviour
{
    
    
    const float sqrPlayerMoveThresholdForChunkUpdate = 25f*25f;
    /// <summary>
    /// Distance maximale de vue en unités Unity - détermine jusqu'où les chunks sont visibles
    /// </summary>

    public LODInfo[] detailLevels;
    public static float maxViewDst;

    /// <summary>
    /// Référence au Transform du joueur/caméra pour suivre sa position
    /// </summary>
    public Transform viewer;
    public Vector3 viewerPositionOld;

    [Tooltip("Matériau à appliquer sur tous les chunks de terrain")]
    public Material terrainMaterial;

    /// <summary>
    /// Position actuelle du viewer en 2D (x, z) mise à jour chaque frame
    /// </summary>
    public static Vector2 viewerPosition;

    /// <summary>
    /// Référence statique au MapGenerator (partagée par tous les TerrainChunks)
    /// </summary>
    static MapGenerator mapGenerator;

    /// <summary>
    /// Taille d'un chunk de terrain individuel (en unités)
    /// </summary>
    static int chunkSize;

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
    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    /// <summary>
    /// Référence au WaterManager une fois initialisé
    /// </summary>
    private WaterManager waterManager;

    /// <summary>
    /// Initialise la taille des chunks et le système d'eau
    /// </summary>
    void Start()
    {
        // On l'initialise sur la position actuelle du viewer au démarrage
        if (viewer != null)
        {
            viewerPositionOld = viewer.position;
        }
            maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        // On cherche le MapGenerator dans la scène
        mapGenerator = UnityEngine.Object.FindAnyObjectByType<MapGenerator>();

        if (mapGenerator == null)
        {
            Debug.LogError("[EndlessTerrain] MapGenerator introuvable dans la scène !");
            return;
        }

        // Utilise l'instance récupérée pour avoir la taille
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);

        InitializeWater();
        UpdateVisibleChunks();
    }

    /// <summary>
    /// Initialise le WaterManager avec le viewer et la taille du terrain.
    /// Tous les composants sont sur le même GameObject.
    /// </summary>
    private void InitializeWater()
    {
        // Récupère le WaterManager sur le même GameObject
        waterManager = GetComponent<WaterManager>();

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
        if ((viewerPositionOld - viewer.position).sqrMagnitude > sqrPlayerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewer.position;
            // Met à jour quels chunks doivent être visibles
            UpdateVisibleChunks();
        }
        
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

                    
                }
                // Sinon, crée un nouveau chunk à cette position et l'ajoute au dictionnaire
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels,transform, terrainMaterial));
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
        /// Composants du mesh
        /// </summary>
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;

        /// <summary>
        /// Limites du chunk utilisées pour calculer la distance avec le viewer
        /// </summary>
        Bounds bounds;

        MapGenerator.MapData mapData;
        bool mapDataReceived;
        int previousLODIndex = -1;
        /// <summary>
        /// Constructeur - crée un nouveau chunk de terrain à la position spécifiée
        /// </summary>
        /// <param name="coord">Coordonnées du chunk dans la grille</param>
        /// <param name="size">Taille du chunk en unités</param>
        /// <param name="parent">Transform parent pour organiser la hiérarchie</param>
        /// <param name="material">Matériau à appliquer sur le terrain</param>
        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;
            // Calcule la position mondiale en multipliant les coordonnées par la taille
            position = coord * size;
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            // Crée les bounds pour la détection de distance (centré sur la position, avec la taille du chunk)
            bounds = new Bounds(position, Vector2.one * size);

            // Crée un GameObject vide pour ce chunk
            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;

            // Désactive le chunk par défaut (sera activé si dans la distance de vue)
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }

            // Demande la génération asynchrone des données de map
            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        /// <summary>
        /// Callback appelé quand les MapData sont prêtes.
        /// Déclenche ensuite la génération du mesh.
        /// </summary>
        void OnMapDataReceived(MapGenerator.MapData mapData)
        {
            // Demande la génération asynchrone du mesh
            this.mapData = mapData;
            mapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize); 
            meshRenderer.material.mainTexture = texture;
            UpdateTerrainChunk();
        }

        /// <summary>
        /// Callback appelé quand les MeshData sont prêtes.
        /// Applique le mesh généré au MeshFilter.
        /// </summary>
        

        /// <summary>
        /// Met à jour la visibilité du chunk en fonction de sa distance avec le viewer
        /// </summary>
        public void UpdateTerrainChunk()
        {
           if (mapDataReceived) {
            // Calcule la distance depuis le bord le plus proche du chunk jusqu'au viewer
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            // Le chunk est visible si sa distance est inférieure à la distance de vue max
            bool visible = viewerDstFromNearestEdge <= maxViewDst;
            SetVisible(visible);

                if (visible)
                {
                    int lodIndex = 0;
                    // Détermine le niveau de détail (LOD) à utiliser en fonction de la distance
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break; 
                        }
                    }
                    // Si les données de map sont prêtes, demande le mesh pour le LOD approprié
                    if (lodIndex != previousLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh && mapDataReceived)
                        {
                            lodMesh.RequestedMesh(mapData);
                        }
                    }
                    terrainChunksVisibleLastUpdate.Add(this);
                }
            }
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
    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action UpdateCallback;
        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.UpdateCallback = updateCallback;
        }
        public void OnMeshDataReceived(MeshGenerator.MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;
            UpdateCallback();
        }
        public void RequestedMesh(MapGenerator.MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData,lod, OnMeshDataReceived);
        }
    }
    [System.Serializable]
    public struct LODInfo
    {
        
        public int lod;
        public float visibleDstThreshold;
        
    }
}