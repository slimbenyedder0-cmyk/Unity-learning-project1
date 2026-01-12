using JetBrains.Annotations;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh };
    public DrawMode drawMode;
    public const int mapChunkSize = 241;
    [Range (0,6)]
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
    public  AnimationCurve meshHeightCurve ;
    public TerrainType[] regions;
    public TerrainType[] Regions => regions;
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistence, lacunarity, offset);
        
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
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

        MapDisplay display = Object.FindFirstObjectByType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap) { 
        display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
           
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap,mapChunkSize,mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier,meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
    }

    public void OnValidate()
    {
        
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }
}