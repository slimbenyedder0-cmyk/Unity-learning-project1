using UnityEngine;
using UnityEditor;

// Custom editor for the MapGenerator component
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;
        if (DrawDefaultInspector())
        { 
            if (mapGen.autoupdate)
            {
                mapGen.GenerateMapInEditor();
            }
        }
    
        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMapInEditor();
        }
    }
   
}
