using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public Color[] baseColors;
    [Range(0,1)]
    public float[] baseStartHeights;
    float savedMinHeight;
    float savedMaxHeight;
    public void ApplyToMaterial(Material material)
    {
       material.SetInt("_baseColorCount", baseColors.Length);
       material.SetColorArray("_baseColors", baseColors);
       material.SetFloatArray("_baseStartHeights", baseStartHeights);
        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;
        material.SetFloat("_minHeight", minHeight);
        material.SetFloat("_maxHeight", maxHeight);
    }
}