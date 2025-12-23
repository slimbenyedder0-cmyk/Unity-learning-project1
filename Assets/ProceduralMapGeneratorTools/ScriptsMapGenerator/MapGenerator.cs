using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth = 256; // Must be greater than 0
    public int mapHeight = 256; // Must be greater than 0
    public float noiseScale = 20f; // Must be greater than 0
    public bool autoupdate;    // Auto update map on parameter change in editor
    public int octaves = 4; // Must be greater than or equal to 0
    [Range(0,1)]
    public float persistence = 0.5f;    // Must be between 0 and 1
    public float lacunarity = 2f; // Must be greater than or equal to 1
    public int seed;    // Seed for random number generator
    public Vector2 offset; // Offset for noise map generation

    // Generates the noise map and displays it using the MapDisplay component
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight,seed, noiseScale, octaves, persistence, lacunarity, offset);

        MapDisplay display = Object.FindFirstObjectByType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }
    // Ensures that parameters have valid values
    public void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
