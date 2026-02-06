using UnityEngine;

/// <summary>
/// Script de contrôle automatique pour Rigidbody. 
/// Applique le bridage et la lévitation de PhysicsSys de manière isolée.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PhysicsAutoPilot : MonoBehaviour
{
    #region Configuration Giga Importante
    [Header("--- LIMITES PHYSIQUES ---")]
    [SerializeField] private float maxVelocity = 20f;
    [SerializeField] private float maxAngularVelocity = 15f;

    [Header("--- LÉVITATION ---")]
    [SerializeField] private bool useHover = false;
    [SerializeField] private float targetHeight = 1.2f;
    [SerializeField] private float hoverForce = 25f;
    [SerializeField] private float damping = 5f;
    #endregion

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // 1. On bride la vélocité quoi qu'il arrive
        PhysicsSys.ClampVelocity(rb, maxVelocity);
        PhysicsSys.ClampAngularVelocity(rb, maxAngularVelocity);

        // 2. On applique la lévitation seulement si l'option est cochée
        if (useHover)
        {
            PhysicsSys.HoverAtHeight(rb, targetHeight, hoverForce, damping);
        }
    }

    // Petit bonus pour visualiser la target height dans l'éditeur
    private void OnDrawGizmosSelected()
    {
        if (useHover)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * targetHeight);
            Gizmos.DrawWireSphere(transform.position + Vector3.down * targetHeight, 0.2f);
        }
    }
}