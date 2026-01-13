using JetBrains.Annotations;
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

    /// <summary>
    /// Mode d'affichage actuel sélectionné dans l'inspecteur
    /// </summary>
    public DrawMode drawMode;

    /// <summary>
    /// Taille du chunk de carte en vertices (241 = 240 quads + 1 vertex).
    /// Constant pour assurer la compatibilité avec le système de chunks infinis.
    /// </summary>
    public const int mapChunkSize = 241;

    /// <summary>
    /// Niveau de détail du mesh (0 = détail maximum, 6 = détail minimum).
    /// Permet d'optimiser les performances en réduisant le nombre de vertices.
    /// </summary>
    [Range(0, 6)]
    public int levelOfDetail;

    /// <summary>
    /// Échelle du bruit de Perlin - des valeurs plus petites créent des variations plus douces.
    /// Valeurs typiques : 10-50 pour terrain réaliste.
    /// </summary>
    public float noiseScale = 20f;

    /// <summary>
    /// Si true, régénère automatiquement la carte lors des modifications dans l'inspecteur.
    /// Utile pour le prototypage mais peut ralentir l'éditeur.
    /// </summary>
    public bool autoupdate;

    /// <summary>
    /// Nombre de couches de bruit superposées (octaves).
    /// Plus d'octaves = plus de détails, mais coût de calcul plus élevé.
    /// </summary>
    public int octaves = 4;

    /// <summary>
    /// Contrôle l'amplitude de chaque octave successive (0-1).
    /// Valeurs plus basses = octaves supérieures moins visibles.
    /// </summary>
    [Range(0, 1)]
    public float persistence = 0.5f;

    /// <summary>
    /// Contrôle la fréquence de chaque octave successive.
    /// Valeurs plus élevées = détails plus fins dans les octaves supérieures.
    /// </summary>
    public float lacunarity = 2f;

    /// <summary>
    /// Graine pour la génération procédurale.
    /// Même graine = même terrain généré, utile pour la reproductibilité.
    /// </summary>
    public int seed;

    /// <summary>
    /// Décalage de position dans l'espace de bruit.
    /// Permet de générer différentes portions du terrain infini.
    /// </summary>
    public Vector2 offset;

    /// <summary>
    /// Multiplicateur de hauteur du mesh.
    /// Contrôle l'amplitude verticale du terrain (montagnes/vallées).
    /// </summary>
    public float meshHeightMultiplier;

    /// <summary>
    /// Courbe d'animation pour ajuster la distribution des hauteurs.
    /// Permet de créer des plateaux, falaises ou transitions douces.
    /// </summary>
    public AnimationCurve meshHeightCurve;

    /// <summary>
    /// Tableau des régions de terrain avec leurs hauteurs et couleurs associées.
    /// Détermine l'apparence visuelle selon l'altitude (eau, sable, herbe, roche, neige...).
    /// </summary>
    public TerrainType[] regions;

    /// <summary>
    /// Propriété en lecture seule pour accéder aux régions depuis d'autres scripts
    /// </summary>
    public TerrainType[] Regions => regions;

    /// <summary>
    /// Génère et affiche la carte de terrain selon les paramètres configurés.
    /// Crée d'abord une noise map, puis génère les couleurs et enfin affiche selon le mode choisi.
    /// </summary>
    public void GenerateMap()
    {
        // Génère la carte de bruit de Perlin avec tous les paramètres
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistence, lacunarity, offset);

        // Crée un tableau de couleurs basé sur la hauteur de chaque point
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                // Récupère la hauteur actuelle du point
                float currentHeight = noiseMap[x, y];

                // Parcourt les régions pour trouver celle correspondant à cette hauteur
                for (int i = 0; i < regions.Length; i++)
                {
                    // Assigne la couleur de la première région dont la hauteur seuil est dépassée
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        // Trouve le composant d'affichage dans la scène
        MapDisplay display = Object.FindFirstObjectByType<MapDisplay>();

        // Affiche selon le mode sélectionné
        if (drawMode == DrawMode.NoiseMap)
        {
            // Mode NoiseMap : affiche uniquement les niveaux de gris du bruit
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            // Mode ColorMap : affiche la texture colorée selon les régions
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            // Mode Mesh : génère et affiche le mesh 3D avec texture
            display.DrawMesh(
                MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail),
                TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize)
            );
        }
    }

    /// <summary>
    /// Appelée automatiquement par Unity quand des valeurs sont modifiées dans l'inspecteur.
    /// Valide et corrige les paramètres pour éviter des valeurs invalides.
    /// </summary>
    public void OnValidate()
    {
        // Empêche la lacunarité d'être inférieure à 1 (valeur invalide)
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        // Empêche le nombre d'octaves d'être négatif
        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    /// <summary>
    /// Structure définissant un type de terrain avec son nom, sa hauteur seuil et sa couleur.
    /// Utilisée pour créer différentes zones visuelles (eau, plages, forêts, montagnes...).
    /// </summary>
    [System.Serializable]
    public struct TerrainType
    {
        /// <summary>
        /// Nom descriptif du type de terrain (ex: "Eau", "Sable", "Herbe")
        /// </summary>
        public string name;

        /// <summary>
        /// Hauteur maximale pour ce type de terrain (0-1).
        /// Si la hauteur du terrain est inférieure ou égale, cette région s'applique.
        /// </summary>
        public float height;

        /// <summary>
        /// Cette région est elle associée à de l'eau ?
        /// </summary>
        public bool isWater;

        /// <summary>
        /// Couleur associée à ce type de terrain
        /// </summary>
        public Color color;
    }
}