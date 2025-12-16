using UnityEngine;

public class Skybox : MonoBehaviour
{
    public float sphereScale = 500f;
    void Start()
    {
        Debug.Log("[Skybox] Start() appelé");
        // avant de créer la sphère
        Vector3 originalPos = transform.position;
        Debug.Log("[Skybox] Position originale : " + originalPos);

        // Réinitialisation de la position
        transform.position = Vector3.zero;

        // Création de la sphère
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Debug.Log("[Skybox] Sphère créée");
        sphere.transform.localScale = Vector3.one * sphereScale;
        sphere.transform.parent = transform;
        sphere.transform.localPosition = Vector3.zero;
        sphere.transform.rotation = Quaternion.identity; // Fixe la rotation
        Debug.Log("[Skybox] Sphère mise, à l’échelle, parentée et centrée");

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
        // Inversion des normales
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];
        mesh.normals = normals;

        // Inversion des triangles pour que le culling fonctionne
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = temp;
        }
        mesh.triangles = triangles;
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

        // Debug log vérification faites comme un psycopathe to be continued dans un autre GO. 

    }

    void Update()
    {
        // volontairement vide pour l’instant
    }
}
