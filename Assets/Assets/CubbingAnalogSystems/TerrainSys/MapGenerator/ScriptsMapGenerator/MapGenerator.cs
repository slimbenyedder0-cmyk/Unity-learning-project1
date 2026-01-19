using UnityEngine;

/// <summary>
/// Générateur de terrain procédural utilisant le bruit de Perlin.
/// Gère la création de cartes de hauteur, colorées ou de mesh 3D selon le mode choisi.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// Énumération des modes d'affichage disponibles pour la carte générée
    /// </summary>
    public enum DrawMode
    {
        NoiseMap,   // Affiche uniquement la carte de bruit en niveaux de gris
        ColorMap,   // Affiche la carte colorée selon les régions de terrain
        Mesh        // Affiche le mesh 3D avec hauteurs et textures
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
    public int levelOfDetail;

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

    // Propriétés publiques
    public TerrainType[] Regions => regions;

    /// <summary>
    /// Hauteur réelle de l'eau dans le monde (calculée depuis waterLevel et meshHeightMultiplier)
    /// </summary>
    public float actualWaterHeight { get; private set; }

    #region Génération de Map

    /// <summary>
    /// Génère et affiche la carte de terrain selon les paramètres configurés.
    /// </summary>
    public void GenerateMap()
    {
        // Génère la carte de bruit de Perlin avec tous les paramètres
        float[,] noiseMap = Noise.GenerateNoiseMap(
            mapChunkSize, mapChunkSize, seed, noiseScale,
            octaves, persistence, lacunarity, offset
        );

        // Crée un tableau de couleurs basé sur la hauteur de chaque point
        Color[] colorMap = GenerateColorMap(noiseMap);

        // Trouve le composant d'affichage dans la scène
        MapDisplay display = Object.FindFirstObjectByType<MapDisplay>();

        // Affiche selon le mode sélectionné
        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
                break;

            case DrawMode.ColorMap:
                display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
                break;

            case DrawMode.Mesh:
                display.DrawMesh(
                    MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail),
                    TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize)
                );
                break;
        }

        // Calcule la hauteur d'eau réelle pour le WaterManager
        CalculateActualWaterHeight();
    }

    /// <summary>
    /// Génère la carte de couleurs basée sur la heightmap et les régions de terrain.
    /// </summary>
    private Color[] GenerateColorMap(float[,] noiseMap)
    {
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];

                // Trouve la région correspondante à cette hauteur
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color;
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
    private void CalculateActualWaterHeight()
    {
        actualWaterHeight = waterLevel * meshHeightMultiplier;
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

        // Vérifie que waterLevel correspond approximativement à une région de terrain
        ValidateWaterLevel();

        // Recalcule la hauteur d'eau si les paramètres changent
        CalculateActualWaterHeight();
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