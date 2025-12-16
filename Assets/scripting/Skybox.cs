using UnityEngine;

public class Skybox : MonoBehaviour
{
    void Start()
    {
        Debug.Log("[Skybox] Start() appelé");

        // Création de la sphère
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Debug.Log("[Skybox] Sphère créée");

        sphere.transform.localScale = Vector3.one * 1000f;
        sphere.transform.parent = transform;
        Debug.Log("[Skybox] Sphère mise à l’échelle et parentée");

        // Suppression du collider
        Collider col = sphere.GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col);
            Debug.Log("[Skybox] Collider supprimé");
        }
        else
        {
            Debug.LogWarning("[Skybox] Aucun collider trouvé");
        }

        // Récupération du mesh
        MeshFilter meshFilter = sphere.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("[Skybox] MeshFilter introuvable — arrêt");
            return;
        }

        Mesh mesh = meshFilter.mesh;
        print("[Skybox] Mesh récupéré, normales = " + mesh.normals.Length);

        // Inversion des normales
        Vector3[] invertedNormals = mesh.normals;
        for (int i = 0; i < invertedNormals.Length; i++)
            invertedNormals[i] = -invertedNormals[i];

        mesh.normals = invertedNormals;
        Debug.Log("[Skybox] Normales inversées");

        // Renderer et shader
        MeshRenderer renderer = sphere.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            Debug.LogError("[Skybox] MeshRenderer introuvable — arrêt");
            return;
        }

        Shader skyShader = Shader.Find("Unlit/ProceduralSky");
        if (skyShader == null)
        {
            Debug.LogError("[Skybox] Shader introuvable : Unlit/ProceduralSky");
            return;
        }

        Debug.Log("[Skybox] Shader trouvé : " + skyShader.name);

        // Création du matériau
        Material skyMaterial = new Material(skyShader);
        renderer.material = skyMaterial;
        Debug.Log("[Skybox] Matériau créé et assigné");

        print("[Skybox] Initialisation terminée avec succès");
    }

    void Update()
    {
        // volontairement vide pour l’instant
    }
}
