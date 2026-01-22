using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

/// <summary>
/// Générateur de terrain procédural utilisant le bruit de Perlin.
/// Gère la création de cartes de hauteur, colorées ou de mesh 3D selon le mode choisi.
/// Supporte le threading pour optimiser les performances.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    public Noise.NormalizeMode normalizeMode;

    /// <summary>
    /// Énumération des modes d'affichage disponibles pour la carte générée
    /// </summary>
    public enum DrawMode
    {
        NoiseMap,   // Affiche uniquement la carte de bruit en niveaux de gris
        ColorMap,   // Affiche la carte colorée selon les régions de terrain
        Mesh,       // Affiche le mesh 3D avec hauteurs et textures
        FalloffMap  // Affiche la carte de chute (falloff map)
    }

    [Header("Display Settings")]
    public DrawMode drawMode;

    [Header("Water Management")]
    [Tooltip("Référence au WaterManager (généralement sur le même GameObject)")]
    [SerializeField] private WaterManager waterManager;

    [Tooltip("Niveau de l'eau normalisé entre 0 et 1 (correspond aux régions de terrain)")]
    [SerializeField, Range(0f, 1f)] public float waterLevel = 0.3f;

    [Header("Map Settings")]
    public const int mapChunkSize = 241;

    [Range(0, 6)]
    public int editorPreviewLOD;

    public float noiseScale = 20f;
    public bool autoupdate;
    public int octaves = 4;

    [Range(0, 1)]
    public float persistence = 0.5f;

    public float lacunarity = 2f;
    public int seed;
    public Vector2 offset;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public TerrainType[] regions;

    [Header("Falloff Map")]
    [Tooltip("Active la carte de chute pour créer des îles")]
    public bool useFalloffMap;

    /// <summary>
    /// Files d'attente thread-safe pour les callbacks de MapData et MeshData
    /// </summary>
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshGenerator.MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshGenerator.MeshData>>();

    // Propriétés publiques
    public TerrainType[] Regions => regions;

    /// <summary>
    /// Hauteur réelle de l'eau dans le monde (calculée depuis waterLevel et meshHeightMultiplier)
    /// </summary>
    public float actualWaterHeight { get; private set; }

    /// <summary>
    /// Échelle réelle de l'eau
    /// </summary>
    public float actualWaterScale { get; private set; }

    /// <summary>
    /// Carte de falloff précalculée
    /// </summary>
    float[,] falloffMap;

    #region Initialisation

    /// <summary>
    /// Génère la falloff map au démarrage si nécessaire
    /// </summary>
    private void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }

    #endregion

    #region Update - Gestion des Threads

    /// <summary>
    /// Traite les callbacks des threads sur le thread principal Unity.
    /// Unity n'autorise pas les opérations sur GameObjects depuis d'autres threads.
    /// </summary>
    void Update()
    {
        // Traite tous les MapData reçus
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        // Traite tous les MeshData reçus
        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshGenerator.MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    #endregion

    #region Génération de Map (Editor)

    /// <summary>
    /// Génère et affiche la carte dans l'éditeur Unity.
    /// Utilisé par le bouton "Generate" dans l'inspecteur.
    /// </summary>
    public void GenerateMapInEditor()
    {
        // Vérifie que les régions sont configurées (sauf pour FalloffMap)
        if (drawMode != DrawMode.FalloffMap && (regions == null || regions.Length == 0))
        {
            Debug.LogError("[MapGenerator] Aucune région de terrain définie! Ajoutez des régions dans l'inspecteur avant de générer.");
            return;
        }

        MapData mapData = GenerateMapData(Vector2.zero);

        // Trouve le composant d'affichage dans la scène
        MapDisplay display = FindFirstObjectByType<MapDisplay>();

        if (display == null)
        {
            Debug.LogError("[MapGenerator] MapDisplay introuvable dans la scène!");
            return;
        }

        // Affiche selon le mode sélectionné
        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
                break;

            case DrawMode.ColorMap:
                display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
                break;

            case DrawMode.Mesh:
                display.DrawMesh(
                    MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD),
                    TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize)
                );
                break;

            case DrawMode.FalloffMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize)));
                break;
        }
    }

    #endregion

    #region Génération Asynchrone (Threading)

    /// <summary>
    /// Demande la génération de MapData sur un thread séparé.
    /// Le callback sera appelé sur le thread principal Unity dans Update().
    /// </summary>
    /// <param name="centre">Centre du chunk à générer</param>
    /// <param name="callback">Fonction appelée quand les données sont prêtes</param>
    public void RequestMapData(Vector2 centre, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(centre, callback);
        };
        new Thread(threadStart).Start();
    }

    /// <summary>
    /// Génère les MapData sur un thread séparé et enfile le callback.
    /// </summary>
    void MapDataThread(Vector2 centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    /// <summary>
    /// Demande la génération de MeshData sur un thread séparé.
    /// Le callback sera appelé sur le thread principal Unity dans Update().
    /// </summary>
    /// <param name="mapData">Données de la carte à convertir en mesh</param>
    /// <param name="lod">Niveau de détail du mesh</param>
    /// <param name="callback">Fonction appelée quand le mesh est prêt</param>
    public void RequestMeshData(MapData mapData, int lod, Action<MeshGenerator.MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };
        new Thread(threadStart).Start();
    }

    /// <summary>
    /// Génère les MeshData sur un thread séparé et enfile le callback.
    /// </summary>
    void MeshDataThread(MapData mapData, int lod, Action<MeshGenerator.MeshData> callback)
    {
        MeshGenerator.MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshGenerator.MeshData>(callback, meshData));
        }
    }

    #endregion

    #region Génération de Données

    /// <summary>
    /// Génère les données de la carte (heightmap et colormap) pour un centre donné.
    /// Retourne un MapData contenant toutes les informations nécessaires.
    /// </summary>
    /// <param name="centre">Position centrale du chunk à générer</param>
    MapData GenerateMapData(Vector2 centre)
    {
        // Génère la carte de bruit de Perlin avec tous les paramètres
        float[,] noiseMap = Noise.GenerateNoiseMap(
            mapChunkSize,
            mapChunkSize,
            seed,
            noiseScale,
            octaves,
            persistence,
            lacunarity,
            centre + offset,
            normalizeMode
        );

        // Applique la falloff map si activée
        if (useFalloffMap)
        {
            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
            }
        }

        // Crée un tableau de couleurs basé sur la hauteur de chaque point
        Color[] colorMap = GenerateColorMap(noiseMap);

        return new MapData(noiseMap, colorMap);
    }

    /// <summary>
    /// Génère la carte de couleurs basée sur la heightmap et les régions de terrain.
    /// </summary>
    Color[] GenerateColorMap(float[,] noiseMap)
    {
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        // Vérifie que les régions sont définies
        if (regions == null || regions.Length == 0)
        {
            Debug.LogWarning("[MapGenerator] Aucune région définie, utilisation de blanc par défaut");
            // Remplit avec du blanc par défaut
            for (int i = 0; i < colorMap.Length; i++)
            {
                colorMap[i] = Color.white;
            }
            return colorMap;
        }

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];

                // Trouve la région correspondante à cette hauteur
                for (int i = 0; i < regions.Length; i++)
                {
                    // Logique avec >= pour que l'offset fonctionne correctement
                    if (currentHeight >= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return colorMap;
    }

    #endregion

    #region Gestion de l'Eau

    /// <summary>
    /// Calcule la hauteur réelle de l'eau dans le monde (en unités Unity).
    /// Convertit waterLevel (0-1) en hauteur mondiale en utilisant meshHeightMultiplier.
    /// </summary>
    public void CalculateActualWaterData()
    {
        actualWaterHeight = waterLevel * meshHeightMultiplier;
        actualWaterScale = mapChunkSize;
    }

    /// <summary>
    /// Retourne la référence au WaterManager.
    /// </summary>
    public WaterManager GetWaterManager()
    {
        return waterManager;
    }

    #endregion

    #region Validation

    /// <summary>
    /// Validation des paramètres dans l'inspecteur Unity.
    /// </summary>
    public void OnValidate()
    {
        // Valide les paramètres de bruit
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        if (octaves < 0)
        {
            octaves = 0;
        }

        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
        // Vérifie que waterLevel correspond approximativement à une région de terrain
        ValidateWaterLevel();

        // Recalcule la hauteur d'eau si les paramètres changent
        CalculateActualWaterData();
    }

    /// <summary>
    /// Vérifie que le waterLevel correspond à une région de terrain définie.
    /// Affiche un warning si aucune région ne correspond (pour éviter des bugs visuels).
    /// </summary>
    private void ValidateWaterLevel()
    {
        if (regions == null || regions.Length == 0) return;

        bool foundMatchingRegion = false;
        const float tolerance = 0.05f; // Tolérance de 5%

        foreach (var region in regions)
        {
            if (Mathf.Abs(region.height - waterLevel) < tolerance)
            {
                foundMatchingRegion = true;
                break;
            }
        }

        if (!foundMatchingRegion)
        {
            Debug.LogWarning(
                $"[MapGenerator] WaterLevel ({waterLevel:F2}) ne correspond à aucune région de terrain. " +
                "Considérez d'aligner waterLevel avec une région existante pour de meilleurs résultats visuels."
            );
        }
    }

    #endregion

    #region Structures

    /// <summary>
    /// Contient les données générées d'une carte : heightmap et colormap.
    /// Utilisé pour retourner plusieurs valeurs depuis GenerateMapData().
    /// </summary>
    public struct MapData
    {
        public readonly float[,] heightMap;
        public readonly Color[] colorMap;

        public MapData(float[,] heightMap, Color[] colorMap)
        {
            this.heightMap = heightMap;
            this.colorMap = colorMap;
        }
    }

    /// <summary>
    /// Structure thread-safe pour stocker un callback et son paramètre.
    /// Utilisée pour communiquer entre threads de génération et le thread principal Unity.
    /// </summary>
    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }

    /// <summary>
    /// Définit un type de terrain avec un nom, une hauteur seuil et une couleur.
    /// </summary>
    [System.Serializable]
    public struct TerrainType
    {
        [Tooltip("Nom de la région (ex: Eau, Plage, Prairie, Montagne)")]
        public string name;

        [Tooltip("Hauteur maximale de cette région (valeur normalisée 0-1)")]
        public float height;

        [Tooltip("Couleur de cette région sur la carte")]
        public Color color;
    }

    #endregion
}