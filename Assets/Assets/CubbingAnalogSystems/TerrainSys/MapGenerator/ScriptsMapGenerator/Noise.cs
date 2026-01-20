using UnityEngine;

/// <summary>
/// Générateur de bruit de Perlin pour la génération procédurale de terrain.
/// Utilise plusieurs octaves pour créer du bruit multi-fréquence détaillé.
/// </summary>
public static class Noise
{
    /// <summary>
    /// Mode de normalisation de la heightmap générée
    /// </summary>
    public enum NormalizeMode
    {
        Local,  // Normalise entre le min/max de cette map spécifique
        Global  // Normalise selon la hauteur max théorique possible
    };

    /// <summary>
    /// Génère une carte de bruit de Perlin 2D avec plusieurs octaves.
    /// </summary>
    /// <param name="mapWidth">Largeur de la carte en pixels</param>
    /// <param name="mapHeight">Hauteur de la carte en pixels</param>
    /// <param name="seed">Graine pour la génération aléatoire (même seed = même résultat)</param>
    /// <param name="scale">Échelle du bruit (plus petit = plus zoomé)</param>
    /// <param name="octaves">Nombre de couches de bruit (plus = plus de détails)</param>
    /// <param name="persistence">Contrôle la diminution de l'amplitude à chaque octave (0-1)</param>
    /// <param name="lacunarity">Contrôle l'augmentation de la fréquence à chaque octave (>1)</param>
    /// <param name="offset">Décalage pour générer différentes sections du bruit</param>
    /// <param name="normalizeMode">Mode de normalisation des valeurs</param>
    /// <returns>Tableau 2D de valeurs normalisées entre 0 et 1</returns>
    public static float[,] GenerateNoiseMap(
        int mapWidth,
        int mapHeight,
        int seed,
        float scale,
        int octaves,
        float persistence,
        float lacunarity,
        Vector2 offset,
        NormalizeMode normalizeMode)
    {
        // Tableau final contenant les valeurs de bruit
        float[,] noiseMap = new float[mapWidth, mapHeight];

        // Générateur de nombres pseudo-aléatoires avec seed
        System.Random prng = new System.Random(seed);

        // Tableau des offsets pour chaque octave (pour variation)
        Vector2[] octaveOffsets = new Vector2[octaves];

        // Calcule la hauteur maximale possible (pour normalisation globale)
        float maxPossibleHeight = 0;
        float amplitude = 1;

        for (int i = 0; i < octaves; i++)
        {
            // Génère un offset aléatoire unique pour cette octave
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            // Accumule l'amplitude max possible
            maxPossibleHeight += amplitude;
            amplitude *= persistence;
        }

        // Évite la division par zéro
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        // Variables pour normalisation locale
        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        // Centre de la map pour un zoom centré
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        // Génère le bruit pour chaque pixel
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // Réinitialise pour chaque pixel
                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                // Accumule le bruit de chaque octave
                for (int i = 0; i < octaves; i++)
                {
                    // Calcule les coordonnées d'échantillonnage
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    // Génère une valeur Perlin entre -1 et 1
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    // Ajoute cette octave à la hauteur totale
                    noiseHeight += perlinValue * amplitude;

                    // Prépare pour la prochaine octave
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                // Suit les valeurs min/max pour normalisation locale
                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                // Stocke la valeur brute
                noiseMap[x, y] = Mathf.Clamp(noiseHeight,0,int.MaxValue);
            }
        }

        // Normalise toutes les valeurs entre 0 et 1
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (normalizeMode == NormalizeMode.Local)
                {
                    // Normalisation locale : entre le min et max de cette map
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    // Normalisation globale : basée sur la hauteur max théorique
                    float normalizedHeight = (noiseMap[x, y] + 1) / (2f * maxPossibleHeight / 1.75f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, 1);
                }
            }
        }

        return noiseMap;
    }
}