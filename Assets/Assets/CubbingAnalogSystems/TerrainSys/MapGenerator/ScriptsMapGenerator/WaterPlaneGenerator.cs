using Unity.VisualScripting;
using UnityEngine;

public class WaterPlaneGenerator : MonoBehaviour
{
    /// <summary>
    /// Matériau de l'eau à appliquer au plan
    /// </summary>
    [SerializeField]
    private Material waterMaterial;

    /// <summary>
    /// Génère un plan d'eau avec le matériau configuré
    /// </summary>
    /// <param name="position">Position mondiale du plan d'eau</param>
    /// <param name="scale">Échelle du plan (largeur, hauteur, profondeur)</param>
    public void GenerateWaterPlane(Vector3 position, Vector3 scale,GameObject parent)
    {
        // Crée un plan primitif pour l'eau
        GameObject waterPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        waterPlane.name = "WaterPlane";
        waterPlane.transform.position = position;
        waterPlane.transform.localScale = scale;
        // Whose your daddy? Lol parametrage du parent
        waterPlane.transform.parent = parent.transform;

        // Applique le matériau d'eau si configuré
        if (waterMaterial != null)
        {
            Renderer renderer = waterPlane.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = waterMaterial;
            }
        }
        else
        {
            Debug.LogWarning("Water material not assigned! Please assign it in the inspector.");
        }
    }
    public void EnableWaterPlane(bool enable)
    {
        Transform waterPlane = transform.Find("WaterPlane");
        if (waterPlane != null)
        {
            waterPlane.gameObject.SetActive(enable);
        }
    }
}
