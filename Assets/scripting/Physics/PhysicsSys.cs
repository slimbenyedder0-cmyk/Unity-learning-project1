using UnityEngine;

/// <summary>
/// Système physique global optimisé. 
/// TOUTES les méthodes vérifient si le Rigidbody est null avant exécution.
/// </summary>
public static class PhysicsSys
{
    // --- CONTRÔLE DE VÉLOCITÉ ET MOUVEMENT ---
    #region Velocity Control

    public static void ClampVelocity(Rigidbody rb, float maxVelocity)
    {
        if (rb == null || maxVelocity <= 0) return;

        // Utilisation de sqrMagnitude pour les performances (évite le calcul de racine carrée)
        if (rb.linearVelocity.sqrMagnitude > maxVelocity * maxVelocity)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
    }

    public static void ClampAngularVelocity(Rigidbody rb, float maxAngularVelocity)
    {
        if (rb == null || maxAngularVelocity <= 0) return;

        if (rb.angularVelocity.magnitude > maxAngularVelocity)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularVelocity;
        }
    }

    /// <summary> 
    /// VERSION SÉCURISÉE : Réduit la vélocité linéairement. 
    /// ATTENTION : Pour un arrêt progressif fluide, préférez ApplyInertiaDampening dans FixedUpdate.
    /// </summary>
    public static void GradualStop(Rigidbody rb, float dampingAmount)
    {
        if (rb == null) return;
        // On évite la boucle While qui freeze Unity. On applique un freinage par frame.
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, dampingAmount * Time.fixedDeltaTime);
    }

    public static void StorePreviousVelocity(Rigidbody rb, out Vector3 previousVelocity)
    {
        previousVelocity = (rb != null) ? rb.linearVelocity : Vector3.zero;
    }

    public static void RestorePreviousVelocity(Rigidbody rb, Vector3 previousVelocity)
    {
        if (rb != null) rb.linearVelocity = previousVelocity;
    }

    #endregion

    // --- DÉTECTION ET NAVIGATION ---
    #region Detection & Navigation

    public static bool IsGrounded(Collider col, out RaycastHit hit, float extraDistance = 0.1f)
    {
        hit = new RaycastHit();
        if (col == null) return false;

        float radius = col.bounds.extents.x * 0.9f;
        Vector3 origin = col.bounds.center;
        float dist = col.bounds.extents.y + extraDistance;

        return Physics.SphereCast(origin, radius, Vector3.down, out hit, dist);
    }

    public static void OrbitAroundPoint(Rigidbody rb, Vector3 point, Vector3 axis, float angularSpeed)
    {
        if (rb == null || Time.fixedDeltaTime <= 0) return;

        Vector3 direction = rb.position - point;
        Quaternion rotation = Quaternion.AngleAxis(angularSpeed * Time.fixedDeltaTime, axis.normalized);
        Vector3 newDirection = rotation * direction;
        Vector3 newPosition = point + newDirection;
        
        // Calcul manuel de la vélocité nécessaire pour atteindre la position cible
        rb.linearVelocity = (newPosition - rb.position) / Time.fixedDeltaTime;
    }

    #endregion

    // --- FORCES ET IMPACTS ---
    #region Forces & Impacts

    public static void LaunchProjectile(Rigidbody rb, Vector3 launchDirection, float launchForce)
    {
        if (rb == null || launchDirection == Vector3.zero) return;
        rb.AddForce(launchDirection.normalized * launchForce, ForceMode.Impulse);
    }

    public static void ApplyBoost(Rigidbody rb, Vector3 boostDirection, float boostAmount)
    {
        if (rb == null || boostDirection == Vector3.zero) return;
        rb.linearVelocity += boostDirection.normalized * boostAmount;
    }

    #endregion

    // --- ENVIRONNEMENT (GRAVITÉ, VENT, LÉVITATION) ---
    #region Environment

    public static void HoverAtHeight(Rigidbody rb, float targetHeight, float hoverForce, float damping)
    {
        if (rb == null) return;

        // Raycast un peu plus long que la targetHeight pour anticiper le sol
        if (Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, targetHeight * 2f))
        {
            float heightError = targetHeight - hit.distance;
            
            // Force de ressort (Spring)
            Vector3 lift = Vector3.up * heightError * hoverForce;
            
            // Amortissement (Damping) pour éviter l'effet "yo-yo"
            Vector3 verticalVelocity = Vector3.Project(rb.linearVelocity, Vector3.up);
            Vector3 dampingForce = -verticalVelocity * damping;
            
            rb.AddForce(lift + dampingForce, ForceMode.Acceleration);
        }
    }

    public static void ApplyAntiGravity(Rigidbody rb)
    {
        if (rb == null) return;
        rb.AddForce(-Physics.gravity * rb.mass);
    }

    public static void ApplyMagneticForce(Rigidbody rb1, Rigidbody rb2, float magneticStrength)
    {
        if (rb1 == null || rb2 == null) return;

        Vector3 direction = rb2.position - rb1.position;
        float sqrDistance = direction.sqrMagnitude; // sqrMagnitude est plus rapide que magnitude

        if (sqrDistance > 0.01f)
        {
            Vector3 magneticForce = direction.normalized * (magneticStrength / sqrDistance);
            rb1.AddForce(magneticForce);
            rb2.AddForce(-magneticForce);
        }
    }

    #endregion

    // --- ROTATION ET TRAJECTOIRE ---
    #region Rotation & Math

    public static void ApplyInertiaDampening(Rigidbody rb, float dampeningForce, bool ignoreY = true)
    {
        if (rb == null) return;
        Vector3 velocity = rb.linearVelocity;
        if (ignoreY) velocity.y = 0;

        if (velocity.sqrMagnitude > 0.001f)
        {
            rb.AddForce(-velocity * dampeningForce);
        }
    }

    public static void TorqueLookAt(Rigidbody rb, Vector3 targetPos, float force)
    {
        if (rb == null) return;
        Vector3 direction = targetPos - rb.position;
        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(rb.rotation);

        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f) angle -= 360f;

        if (!float.IsNaN(axis.x) && !float.IsInfinity(axis.x))
        {
            rb.AddTorque(axis.normalized * angle * force);
        }
    }

    #endregion
}