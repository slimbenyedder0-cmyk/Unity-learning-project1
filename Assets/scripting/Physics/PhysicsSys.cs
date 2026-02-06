using UnityEngine;

public static class PhysicsSys
{
    // --- CONTR�LE DE V�LOCIT� ET MOUVEMENT ---
    #region Velocity Control

    /// <summary> Limite la v�locit� d'un Rigidbody � une valeur maximale sp�cifi�e. </summary>
    public static void ClampVelocity(Rigidbody rb, float maxVelocity)
    {
        if (rb.linearVelocity.sqrMagnitude > maxVelocity * maxVelocity)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
    }

    /// <summary>Cette fonction clamp la vitesse de rotation d'un objet pour �viter les spins incontr�lables</summary>
    public static void ClampAngularVelocity(Rigidbody rb, float maxAngularVelocity)
    {
        if (rb.angularVelocity.magnitude > maxAngularVelocity)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularVelocity;
        }
    }

    /// <summary> R�duit la v�locit� d'un Rigidbody progressivement. Note : � utiliser avec pr�caution dans une classe statique (boucle While). </summary>
    public static void GradualStop(Rigidbody rb, float stopDuration)
    {
        if (stopDuration <= 0) return;
        Vector3 initialVelocity = rb.linearVelocity;
        float elapsedTime = 0f;
        while (elapsedTime < stopDuration)
        {
            float t = elapsedTime / stopDuration;
            rb.linearVelocity = Vector3.Lerp(initialVelocity, Vector3.zero, t);
            elapsedTime += Time.fixedDeltaTime;
        }
        rb.linearVelocity = Vector3.zero;
    }

    /// <summary> Stocke la v�locit� actuelle pour un usage futur. </summary>
    public static void StorePreviousVelocity(Rigidbody rb, out Vector3 previousVelocity)
    {
        previousVelocity = rb.linearVelocity;
    }

    /// <summary> Restaure une v�locit� pr�c�demment stock�e. </summary>
    public static void RestorePreviousVelocity(Rigidbody rb, Vector3 previousVelocity)
    {
        rb.linearVelocity = previousVelocity;
    }

    #endregion

    // --- D�TECTION ET NAVIGATION ---
    #region Detection & Navigation

    /// <summary> V�rifie si un collider touche le sol via un SphereCast (plus pr�cis qu'un simple Raycast). </summary>
    public static bool IsGrounded(Collider col, out RaycastHit hit, float extraDistance = 0.1f)
    {
        float radius = col.bounds.extents.x * 0.9f;
        Vector3 origin = col.bounds.center;
        float dist = col.bounds.extents.y + extraDistance;

        return Physics.SphereCast(origin, radius, Vector3.down, out hit, dist);
    }

    /// <summary> Fait orbiter un Rigidbody autour d'un point pivot. </summary>
    public static void OrbitAroundPoint(Rigidbody rb, Vector3 point, Vector3 axis, float angularSpeed)
    {
        Vector3 direction = rb.position - point;
        Quaternion rotation = Quaternion.AngleAxis(angularSpeed * Time.fixedDeltaTime, axis.normalized);
        Vector3 newDirection = rotation * direction;
        Vector3 newPosition = point + newDirection;
        Vector3 desiredVelocity = (newPosition - rb.position) / Time.fixedDeltaTime;
        rb.linearVelocity = desiredVelocity;
    }

    #endregion

    // --- FORCES ET IMPACTS ---
    #region Forces & Impacts

    /// <summary> Applique une impulsion instantan�e (type boulet de canon). </summary>
    public static void LaunchProjectile(Rigidbody rb, Vector3 launchDirection, float launchForce)
    {
        rb.AddForce(launchDirection.normalized * launchForce, ForceMode.Impulse);
    }

    /// <summary> Applique un boost de v�locit� imm�diat. </summary>
    public static void ApplyBoost(Rigidbody rb, Vector3 boostDirection, float boostAmount)
    {
        rb.linearVelocity += boostDirection.normalized * boostAmount;
    }

    /// <summary> Retire l'effet d'un boost de v�locit�. </summary>
    public static void RemoveBoost(Rigidbody rb, Vector3 boostDirection, float boostAmount)
    {
        rb.linearVelocity -= boostDirection.normalized * boostAmount;
    }

    /// <summary> Simule un rebond sur le sol avec une perte d'�nergie (restitution). </summary>
    public static void BounceOffGround(Rigidbody rb, float restitution)
    {
        if (Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, 1f))
        {
            float velocityAlongNormal = Vector3.Dot(rb.linearVelocity, hit.normal);
            if (velocityAlongNormal < 0)
            {
                rb.linearVelocity -= (1 + restitution) * velocityAlongNormal * hit.normal;
            }
        }
    }

    #endregion

    // --- ENVIRONNEMENT (GRAVIT�, VENT, MAGN�TISME) ---
    #region Environment

    /// <summary> Maintient le Rigidbody en l�vitation � une hauteur pr�cise via un syst�me d'amortissement (Spring). </summary>
    public static void HoverAtHeight(Rigidbody rb, float targetHeight, float hoverForce, float damping)
    {
        if (Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, targetHeight + 1f))
        {
            float heightError = targetHeight - hit.distance;
            Vector3 lift = Vector3.up * heightError * hoverForce;
            Vector3 verticalVelocity = Vector3.Project(rb.linearVelocity, Vector3.up);
            Vector3 dampingForce = -verticalVelocity * damping;
            rb.AddForce(lift + dampingForce);
        }
    }

    /// <summary> Annule l'effet de la gravit� globale de Unity. </summary>
    public static void ApplyAntiGravity(Rigidbody rb)
    {
        rb.AddForce(-Physics.gravity * rb.mass);
    }

    /// <summary> Applique une gravit� personnalis�e dans une direction donn�e. </summary>
    public static void ApplyDirectionalGravity(Rigidbody rb, Vector3 gravityDirection, float gravityStrength)
    {
        rb.AddForce(gravityDirection.normalized * gravityStrength);
    }

    /// <summary> Applique une force constante pour simuler du vent. </summary>
    public static void ApplyWindForce(Rigidbody rb, Vector3 windDirection, float windStrength)
    {
        rb.AddForce(windDirection.normalized * windStrength);
    }

    /// <summary> Simule une force de friction horizontale. </summary>
    public static void ApplyFriction(Rigidbody rb, float frictionCoefficient)
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 frictionForce = -horizontalVelocity.normalized * frictionCoefficient * horizontalVelocity.magnitude;
        rb.AddForce(frictionForce);
    }

    /// <summary> Applique une force d'attraction ou de r�pulsion entre deux objets. </summary>
    public static void ApplyMagneticForce(Rigidbody rb1, Rigidbody rb2, float magneticStrength)
    {
        Vector3 direction = rb2.position - rb1.position;
        float distance = direction.magnitude;
        if (distance > 0)
        {
            Vector3 magneticForce = direction.normalized * (magneticStrength / (distance * distance));
            rb1.AddForce(magneticForce);
            rb2.AddForce(-magneticForce);
        }
    }

    #endregion

    // --- ROTATION ET TRAJECTOIRE ---
    #region Rotation & Math
    /// <summary> R�duit l'inertie lin�aire du Rigidbody sans affecter la gravit�. </summary>
    public static void ApplyInertiaDampening(Rigidbody rb, float dampeningForce, bool ignoreY = true)
    {
        Vector3 velocity = rb.linearVelocity;

        // On peut choisir d'ignorer l'axe Y pour ne pas ralentir la chute libre
        if (ignoreY) velocity.y = 0;

        if (velocity.magnitude > 0.01f)
        {
            // Applique une force oppos�e au mouvement
            rb.AddForce(-velocity * dampeningForce);
        }
    }
    /// <summary> R�duit la vitesse de rotation pour stabiliser l'objet. </summary>
    public static void ApplyAngularDampening(Rigidbody rb, float dampeningForce)
    {
        if (rb.angularVelocity.magnitude > 0.01f)
        {
            // Applique un couple oppos� � la rotation actuelle
            rb.AddTorque(-rb.angularVelocity * dampeningForce);
        }
    }



    /// <summary> Oriente progressivement le Rigidbody vers une cible en utilisant du couple (Torque). </summary>
    public static void TorqueLookAt(Rigidbody rb, Vector3 targetPos, float force)
    {
        Vector3 direction = targetPos - rb.position;
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(rb.rotation);

        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f) angle -= 360f;

        if (!float.IsNaN(axis.x))
        {
            rb.AddTorque(axis.normalized * angle * force);
        }
    }
    /// <summary> Oriente le Rigidbody � l'oppos� d'une cible en utilisant du couple (Torque). </summary>
    public static void TorqueLookAway(Rigidbody rb, Vector3 targetPos, float force)
    {
        // La direction est invers�e : on part de la cible vers l'objet
        Vector3 direction = rb.position - targetPos;

        // S�curit� si l'objet est exactement sur la cible
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Calcul de l'�cart entre la rotation actuelle et la rotation oppos�e
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(rb.rotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);

        // Ajustement de l'angle pour obtenir le chemin le plus court [-180, 180]
        if (angle > 180f) angle -= 360f;

        if (!float.IsNaN(axis.x))
        {
            rb.AddTorque(axis.normalized * angle * force);
        }
    }

    /// <summary> Calcule un point sur une trajectoire balistique selon le temps 't'. </summary>
    public static void GetTrajectoryPoint(Vector3 startPos, Vector3 startVel, float time, out Vector3 point)
    {
        point = startPos + startVel * time + 0.5f * Physics.gravity * (time * time);
    }

    #endregion
}